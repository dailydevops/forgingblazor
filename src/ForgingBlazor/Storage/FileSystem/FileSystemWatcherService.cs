namespace NetEvolve.ForgingBlazor.Storage.FileSystem;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Hosted service that monitors file system changes for content files.
/// </summary>
/// <remarks>
/// This service wraps <see cref="FileSystemWatcher"/> and implements debouncing
/// to avoid excessive events during rapid file changes.
/// </remarks>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
internal sealed class FileSystemWatcherService : IHostedService, IDisposable
{
    private readonly ILogger<FileSystemWatcherService> _logger;
    private readonly FileSystemStorageOptions _options;
    private readonly List<Action<string>> _changeHandlers = [];
    private FileSystemWatcher? _watcher;
    private Timer? _debounceTimer;
    private readonly HashSet<string> _pendingChanges = [];
    private readonly object _lock = new();
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcherService"/> class.
    /// </summary>
    /// <param name="options">The file system storage options.</param>
    /// <param name="logger">The logger instance.</param>
    internal FileSystemWatcherService(FileSystemStorageOptions options, ILogger<FileSystemWatcherService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// Registers a handler to be called when file changes are detected.
    /// </summary>
    /// <param name="handler">The handler to invoke with changed file paths.</param>
    internal void OnFileChanged(Action<string> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _changeHandlers.Add(handler);
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.EnableWatch || string.IsNullOrEmpty(_options.BasePath))
        {
            _logger.LogInformation("File system watching is disabled or base path not configured");
            return Task.CompletedTask;
        }

        if (!Directory.Exists(_options.BasePath))
        {
            _logger.LogWarning("Base path does not exist: {BasePath}", _options.BasePath);
            return Task.CompletedTask;
        }

        _logger.LogInformation("Starting file system watcher for: {BasePath}", _options.BasePath);

        _watcher = new FileSystemWatcher(_options.BasePath)
        {
            NotifyFilter =
                NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
            Filter = "*.md",
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
        };

        _watcher.Changed += OnWatcherEvent;
        _watcher.Created += OnWatcherEvent;
        _watcher.Deleted += OnWatcherEvent;
        _watcher.Renamed += OnWatcherRenamed;

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping file system watcher");

        if (_watcher is not null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnWatcherEvent;
            _watcher.Created -= OnWatcherEvent;
            _watcher.Deleted -= OnWatcherEvent;
            _watcher.Renamed -= OnWatcherRenamed;
        }

        if (_debounceTimer is not null)
        {
            await _debounceTimer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _watcher?.Dispose();
        _debounceTimer?.Dispose();
        _disposed = true;
    }

    private void OnWatcherEvent(object sender, FileSystemEventArgs e)
    {
        lock (_lock)
        {
            _ = _pendingChanges.Add(e.FullPath);
            ResetDebounceTimer();
        }
    }

    private void OnWatcherRenamed(object sender, RenamedEventArgs e)
    {
        lock (_lock)
        {
            _ = _pendingChanges.Add(e.OldFullPath);
            _ = _pendingChanges.Add(e.FullPath);
            ResetDebounceTimer();
        }
    }

    private void ResetDebounceTimer()
    {
        _debounceTimer?.Dispose();
        _debounceTimer = new Timer(
            ProcessPendingChanges,
            state: null,
            dueTime: TimeSpan.FromMilliseconds(300),
            period: Timeout.InfiniteTimeSpan
        );
    }

    private void ProcessPendingChanges(object? state)
    {
        string[] changedFiles;

        lock (_lock)
        {
            changedFiles = [.. _pendingChanges];
            _pendingChanges.Clear();
        }

        foreach (var file in changedFiles)
        {
            _logger.LogDebug("Processing file change: {FilePath}", file);

            foreach (var handler in _changeHandlers)
            {
                try
                {
                    handler(file);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file change handler for: {FilePath}", file);
                }
            }
        }
    }
}
