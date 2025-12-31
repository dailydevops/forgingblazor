namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a page property that assigns descriptive tags or keywords to content for classification and discovery.
/// </summary>
/// <remarks>
/// Tags provide a flexible, non-hierarchical way to classify content using keywords.
/// Unlike categories, tags are typically more granular and can be freely added to describe specific aspects or topics of the content.
/// </remarks>
public interface IPropertyTags
{
    /// <summary>
    /// Gets or sets the collection of tags assigned to this page.
    /// </summary>
    /// <value>
    /// A read-only set of tag names or keywords. Using a set ensures each tag appears only once and provides efficient membership testing.
    /// Tags are commonly used for content discovery, related content recommendations, and SEO keywords.
    /// </value>
    IList<string>? Tags { get; }
}
