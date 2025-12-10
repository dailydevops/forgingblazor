namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a page property that provides a brief summary or description of the page content.
/// </summary>
/// <remarks>
/// This interface is typically used for page metadata, SEO descriptions, and preview text in listings or cards.
/// </remarks>
public interface IPagePropertySummary
{
    /// <summary>
    /// Gets or sets an optional summary or brief description of the page content.
    /// </summary>
    /// <value>
    /// A string containing the page summary, or <see langword="null"/> if no summary is specified.
    /// This value is commonly used for meta descriptions, RSS feeds, and page previews.
    /// </value>
    string? Summary { get; set; }
}

/// <summary>
/// Defines sitemap-related properties that control how a page is included in XML sitemaps.
/// </summary>
/// <remarks>
/// This interface allows pages to configure their visibility and priority in generated sitemap files,
/// which affects how search engines discover and index content.
/// </remarks>
public interface IPagePropertySitemap
{
    /// <summary>
    /// Gets or sets a value indicating whether this page should be excluded from the sitemap.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to exclude the page from sitemap generation; <see langword="false"/> to include it.
    /// The default value is typically <see langword="false"/>, meaning pages are included by default.
    /// </value>
    bool ExcludeFromSitemap { get; set; }

    /// <summary>
    /// Gets or sets the priority hint for this page relative to other pages on the site.
    /// </summary>
    /// <value>
    /// An optional integer value typically between 0 and 10, where higher values suggest higher priority,
    /// or <see langword="null"/> to use the default priority. This is a hint to search engines and does not
    /// guarantee any specific crawling behavior.
    /// </value>
    int? Priority { get; set; }
}
