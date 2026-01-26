namespace NetEvolve.ForgingBlazor.Content;

using System;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
/// Cache service for content descriptors with culture-aware cache keys.
/// </summary>
/// <remarks>
/// This service wraps <see cref="IMemoryCache"/> and provides methods for
/// caching and invalidating content based on culture and path information.
/// </remarks>
internal sealed class ContentCacheService
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _defaultCacheOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentCacheService"/> class.
    /// </summary>
    /// <param name="cache">The memory cache instance.</param>
    internal ContentCacheService(IMemoryCache cache)
    {
        ArgumentNullException.ThrowIfNull(cache);
        _cache = cache;

        _defaultCacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromHours(4))
            .SetPriority(CacheItemPriority.Normal);
    }

    /// <summary>
    /// Gets or creates a cached content descriptor.
    /// </summary>
    /// <typeparam name="TDescriptor">The type of content descriptor.</typeparam>
    /// <param name="segmentPath">The segment path.</param>
    /// <param name="slug">The content slug.</param>
    /// <param name="culture">The culture information.</param>
    /// <param name="factory">Factory function to create the content if not cached.</param>
    /// <returns>The cached or newly created content descriptor.</returns>
    internal TDescriptor? GetOrCreate<TDescriptor>(
        string segmentPath,
        string slug,
        CultureInfo culture,
        Func<TDescriptor?> factory
    )
        where TDescriptor : ContentDescriptor
    {
        ArgumentNullException.ThrowIfNull(segmentPath);
        ArgumentNullException.ThrowIfNull(slug);
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(factory);

        var cacheKey = BuildCacheKey(segmentPath, slug, culture);
        return _cache.GetOrCreate(
            cacheKey,
            entry =>
            {
                _ = entry.SetOptions(_defaultCacheOptions);
                return factory();
            }
        );
    }

    /// <summary>
    /// Invalidates all cache entries for a specific segment path.
    /// </summary>
    /// <param name="segmentPath">The segment path to invalidate.</param>
    /// <remarks>
    /// Note: IMemoryCache doesn't support pattern-based removal.
    /// A full implementation would require tracking keys or using a distributed cache.
    /// For now, we'll need to remove specific entries as they're identified.
    /// </remarks>
    internal static void InvalidateSegment(string segmentPath) => ArgumentNullException.ThrowIfNull(segmentPath);

    /// <summary>
    /// Invalidates a specific content entry.
    /// </summary>
    /// <param name="segmentPath">The segment path.</param>
    /// <param name="slug">The content slug.</param>
    /// <param name="culture">The culture information.</param>
    internal void Invalidate(string segmentPath, string slug, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(segmentPath);
        ArgumentNullException.ThrowIfNull(slug);
        ArgumentNullException.ThrowIfNull(culture);

        var cacheKey = BuildCacheKey(segmentPath, slug, culture);
        _cache.Remove(cacheKey);
    }

    /// <summary>
    /// Clears all cache entries.
    /// </summary>
    /// <remarks>
    /// Note: IMemoryCache doesn't have a Clear method.
    /// This would require tracking all keys or disposing and recreating the cache.
    /// For development, restart the application to clear the cache.
    /// </remarks>
    internal static void Clear() { }

    private static string BuildCacheKey(string segmentPath, string slug, CultureInfo culture)
    {
        var normalizedPath = segmentPath.Replace('\\', '/').Trim('/');
        var normalizedCulture = culture.Name.ToUpperInvariant();
        return $"content:{normalizedPath}:{slug}:{normalizedCulture}";
    }
}
