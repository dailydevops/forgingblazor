namespace NetEvolve.ForgingBlazor.Content;

using System.Globalization;
using NetEvolve.ForgingBlazor.Routing.Culture;

/// <summary>
/// Locates content files using culture fallback chain.
/// </summary>
internal sealed class CultureContentLocator
{
    private readonly CultureFallbackChain _fallbackChain;
    private readonly IContentStorageProvider _storageProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureContentLocator"/> class.
    /// </summary>
    /// <param name="fallbackChain">The culture fallback chain.</param>
    /// <param name="storageProvider">The content storage provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    public CultureContentLocator(CultureFallbackChain fallbackChain, IContentStorageProvider storageProvider)
    {
        ArgumentNullException.ThrowIfNull(fallbackChain);
        ArgumentNullException.ThrowIfNull(storageProvider);

        _fallbackChain = fallbackChain;
        _storageProvider = storageProvider;
    }

    /// <summary>
    /// Locates a content file using the culture fallback chain.
    /// </summary>
    /// <typeparam name="TContent">The content descriptor type.</typeparam>
    /// <param name="segmentPath">The segment path (e.g., "posts").</param>
    /// <param name="slug">The content slug.</param>
    /// <param name="culture">The target culture.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The content descriptor if found; otherwise, <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <remarks>
    /// Attempts to locate content files in the following order (for de-DE):
    /// 1. posts/my-article (with culture de-DE)
    /// 2. posts/my-article (with culture de)
    /// 3. posts/my-article (with culture en-US)
    /// 4. posts/my-article (with culture en)
    /// 5. posts/my-article (with invariant culture)
    /// </remarks>
    public async Task<TContent?> LocateContentAsync<TContent>(
        string segmentPath,
        string slug,
        CultureInfo culture,
        CancellationToken cancellationToken
    )
        where TContent : ContentDescriptor, new()
    {
        ArgumentNullException.ThrowIfNull(segmentPath);
        ArgumentNullException.ThrowIfNull(slug);
        ArgumentNullException.ThrowIfNull(culture);

        var fallbackCultures = _fallbackChain.GetFallbackChain(culture);

        foreach (var fallbackCulture in fallbackCultures)
        {
            var targetCulture = fallbackCulture ?? CultureInfo.InvariantCulture;

            var content = await _storageProvider
                .GetContentAsync<TContent>(segmentPath, slug, targetCulture, cancellationToken)
                .ConfigureAwait(false);

            if (content is not null)
            {
                return content;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all possible lookup paths for a content file.
    /// </summary>
    /// <param name="lookupPath">The base lookup path.</param>
    /// <returns>An ordered list of file paths to try.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lookupPath"/> is <c>null</c>.</exception>
    public IReadOnlyList<string> GetLookupPaths(ContentLookupPath lookupPath)
    {
        ArgumentNullException.ThrowIfNull(lookupPath);

        var suffixes = _fallbackChain.GetCultureSuffixes(lookupPath.Culture);
        var paths = new List<string>(suffixes.Count);

        foreach (var suffix in suffixes)
        {
            paths.Add(lookupPath.GeneratePath(suffix));
        }

        return paths.AsReadOnly();
    }
}
