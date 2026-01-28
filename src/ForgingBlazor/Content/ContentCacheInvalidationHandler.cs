namespace NetEvolve.ForgingBlazor.Content;

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handles cache invalidation based on file system changes.
/// </summary>
/// <remarks>
/// This handler subscribes to file system watcher events and invalidates
/// the corresponding cache entries when content files are modified.
/// </remarks>
internal sealed class ContentCacheInvalidationHandler
{
    private readonly ContentCacheService _cacheService;
    private readonly ILogger<ContentCacheInvalidationHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentCacheInvalidationHandler"/> class.
    /// </summary>
    /// <param name="cacheService">The content cache service.</param>
    /// <param name="logger">The logger instance.</param>
    internal ContentCacheInvalidationHandler(
        ContentCacheService cacheService,
        ILogger<ContentCacheInvalidationHandler> logger
    )
    {
        ArgumentNullException.ThrowIfNull(cacheService);
        ArgumentNullException.ThrowIfNull(logger);

        _cacheService = cacheService;
        _logger = logger;
    }

    /// <summary>
    /// Handles a file change event by invalidating the corresponding cache entry.
    /// </summary>
    /// <param name="fullPath">The full path to the changed file.</param>
    internal void HandleFileChange(string fullPath)
    {
        ArgumentNullException.ThrowIfNull(fullPath);

        try
        {
            if (!fullPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var (segmentPath, slug, culture) = ParseFilePath(fullPath);

            if (segmentPath is null || slug is null || culture is null)
            {
                _logger.LogWarning("Could not parse file path for cache invalidation: {FilePath}", fullPath);
                return;
            }

            _logger.LogDebug(
                "Invalidating cache for segment: {SegmentPath}, slug: {Slug}, culture: {Culture}",
                segmentPath,
                slug,
                culture.Name
            );

            _cacheService.Invalidate(segmentPath, slug, culture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling file change for cache invalidation: {FilePath}", fullPath);
        }
    }

    private (string? segmentPath, string? slug, CultureInfo? culture) ParseFilePath(string fullPath)
    {
        try
        {
            var fileName = Path.GetFileNameWithoutExtension(fullPath);
            var directory = Path.GetDirectoryName(fullPath);

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(directory))
            {
                return (null, null, null);
            }

            var slug = fileName;

            // Extract culture from directory name (e.g., "EN-US", "DE-DE")
            var cultureFolder = new DirectoryInfo(directory).Name;
            var culture = TryResolveCulture(cultureFolder);

            if (culture is null)
            {
                return (null, null, null);
            }

            // Get segment path by removing culture folder and base path
            var parentDirectory = Directory.GetParent(directory);
            if (parentDirectory is null)
            {
                return (null, null, null);
            }

            var segmentPath = GetSegmentPath(parentDirectory.FullName);

            return (segmentPath, slug, culture);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error parsing file path: {FilePath}", fullPath);
            return (null, null, null);
        }
    }

    private static CultureInfo? TryResolveCulture(string cultureFolder)
    {
        try
        {
            // Culture folders are uppercase (e.g., "EN-US", "DE-DE")
            return CultureInfo.GetCultureInfo(cultureFolder);
        }
        catch
        {
            return null;
        }
    }

    private static string GetSegmentPath(string directoryPath)
    {
        // This is a simplified implementation
        // A full implementation would need to know the base content path
        // and calculate the relative segment path from it
        var parts = directoryPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return string.Join("/", parts.TakeLast(2));
    }
}
