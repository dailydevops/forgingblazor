namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a page property that identifies the author or creator of the page content.
/// </summary>
/// <remarks>
/// This interface is commonly used for blog posts, articles, and other content types where authorship attribution is important.
/// </remarks>
public interface IPagePropertyAuthor
{
    /// <summary>
    /// Gets or sets the name or identifier of the page's author.
    /// </summary>
    /// <value>
    /// A string containing the author's name or identifier, or <see langword="null"/> if no author is specified.
    /// This value is typically displayed in page metadata, bylines, and author attribution sections.
    /// </value>
    string? Author { get; set; }
}
