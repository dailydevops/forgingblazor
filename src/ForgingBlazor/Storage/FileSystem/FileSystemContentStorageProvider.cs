namespace NetEvolve.ForgingBlazor.Storage.FileSystem;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using NetEvolve.ForgingBlazor.Content.Parsing;

/// <summary>
/// File system implementation of content storage provider.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
internal sealed class FileSystemContentStorageProvider : IContentStorageProvider, IDisposable
{
    private readonly FileSystemStorageOptions _options;
    private FileSystemWatcher? _watcher;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemContentStorageProvider"/> class.
    /// </summary>
    /// <param name="options">The file system storage options.</param>
    internal FileSystemContentStorageProvider(FileSystemStorageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;

        if (_options.EnableWatch && !string.IsNullOrEmpty(_options.BasePath))
        {
            InitializeWatcher();
        }
    }

    /// <inheritdoc />
    public async Task<TDescriptor?> GetContentAsync<TDescriptor>(
        string segmentPath,
        string slug,
        CultureInfo culture,
        CancellationToken cancellationToken = default
    )
        where TDescriptor : ContentDescriptor, new()
    {
        ArgumentNullException.ThrowIfNull(segmentPath);
        ArgumentNullException.ThrowIfNull(slug);
        ArgumentNullException.ThrowIfNull(culture);

        var relativePath = BuildContentPath(segmentPath, slug, culture);
        var fullPath = GetFullPath(relativePath);

        if (!File.Exists(fullPath))
        {
            return null;
        }

        var markdownContent = await File.ReadAllTextAsync(fullPath, cancellationToken).ConfigureAwait(false);

        return ContentParser.Parse<TDescriptor>(markdownContent);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TDescriptor>> GetContentsAsync<TDescriptor>(
        string segmentPath,
        CultureInfo culture,
        CancellationToken cancellationToken = default
    )
        where TDescriptor : ContentDescriptor, new()
    {
        ArgumentNullException.ThrowIfNull(segmentPath);
        ArgumentNullException.ThrowIfNull(culture);

        var fullPath = GetFullPath(segmentPath);

        if (!Directory.Exists(fullPath))
        {
            return [];
        }

        var cultureFolder = GetCultureFolder(culture);
        var searchPath = Path.Combine(fullPath, cultureFolder);

        if (!Directory.Exists(searchPath))
        {
            return [];
        }

        var files = Directory.GetFiles(searchPath, "*.md", SearchOption.TopDirectoryOnly);
        var contents = new List<TDescriptor>();

        foreach (var file in files)
        {
            var markdownContent = await File.ReadAllTextAsync(file, cancellationToken).ConfigureAwait(false);
            var descriptor = ContentParser.Parse<TDescriptor>(markdownContent);
            contents.Add(descriptor);
        }

        return contents;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var fullPath = GetFullPath(path);
        return Task.FromResult(File.Exists(fullPath));
    }

    /// <inheritdoc />
    public async Task SaveContentAsync(string path, string markdown, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(markdown);

        var fullPath = GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(fullPath, markdown, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task DeleteContentAsync(string path, CancellationToken cancellationToken = default)
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

    private static string BuildContentPath(string segmentPath, string slug, CultureInfo culture)
    {
        var cultureFolder = GetCultureFolder(culture);
        var fileName = $"{slug}.md";
        return Path.Combine(segmentPath.TrimStart('/'), cultureFolder, fileName);
    }

    private static string GetCultureFolder(CultureInfo culture) => culture.Name.ToUpperInvariant();

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
            Filter = "*.md",
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
