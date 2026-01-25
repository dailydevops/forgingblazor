namespace NetEvolve.ForgingBlazor.Storage.FileSystem;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// File system implementation of asset storage provider.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
internal sealed class FileSystemAssetStorageProvider : IAssetStorageProvider, IDisposable
{
    private readonly FileSystemStorageOptions _options;
    private FileSystemWatcher? _watcher;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemAssetStorageProvider"/> class.
    /// </summary>
    /// <param name="options">The file system storage options.</param>
    internal FileSystemAssetStorageProvider(FileSystemStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;

        if (_options.EnableWatch && !string.IsNullOrEmpty(_options.BasePath))
        {
            InitializeWatcher();
        }
    }

    /// <inheritdoc />
    public Task<Stream> GetAssetAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var fullPath = GetFullPath(path);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Asset file not found: {path}", fullPath);
        }

        var fileStream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true
        );

        return Task.FromResult<Stream>(fileStream);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> GetAssetsAsync(string folder, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(folder);

        var fullPath = GetFullPath(folder);

        if (!Directory.Exists(fullPath))
        {
            return Task.FromResult<IReadOnlyList<string>>([]);
        }

        var files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories);
        var relativePaths = new List<string>();

        foreach (var file in files)
        {
            var relativePath = Path.GetRelativePath(fullPath, file);
            relativePaths.Add(relativePath.Replace(Path.DirectorySeparatorChar, '/'));
        }

        return Task.FromResult<IReadOnlyList<string>>(relativePaths);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var fullPath = GetFullPath(path);
        return Task.FromResult(File.Exists(fullPath));
    }

    /// <inheritdoc />
    public async Task SaveAssetAsync(string path, Stream content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(content);

        var fullPath = GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        var fileStream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true
        );
        await using (fileStream.ConfigureAwait(false))
        {
            await content.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public Task DeleteAssetAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var fullPath = GetFullPath(path);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _watcher?.Dispose();
        _disposed = true;
    }

    private string GetFullPath(string relativePath)
    {
        if (string.IsNullOrEmpty(_options.BasePath))
        {
            throw new InvalidOperationException(
                "Base path is not configured. Call WithBasePath() on the storage options."
            );
        }

        var normalizedRelativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(_options.BasePath, normalizedRelativePath);
    }

    private void InitializeWatcher()
    {
        if (string.IsNullOrEmpty(_options.BasePath))
        {
            return;
        }

        if (!Directory.Exists(_options.BasePath))
        {
            _ = Directory.CreateDirectory(_options.BasePath);
        }

        _watcher = new FileSystemWatcher(_options.BasePath)
        {
            NotifyFilter =
                NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
            Filter = "*.*",
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
        };

        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Deleted += OnFileChanged;
        _watcher.Renamed += OnFileRenamed;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // Cache invalidation will be implemented in future phases
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        // Cache invalidation will be implemented in future phases
    }
}
