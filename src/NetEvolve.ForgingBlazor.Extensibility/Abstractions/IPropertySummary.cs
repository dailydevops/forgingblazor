namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a page property that provides a brief summary or description of the page content.
/// </summary>
/// <remarks>
/// This interface is typically used for page metadata, SEO descriptions, and preview text in listings or cards.
/// </remarks>
public interface IPropertySummary
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
