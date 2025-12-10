namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a page property that organizes content into one or more categories.
/// </summary>
/// <remarks>
/// Categories provide a hierarchical or high-level classification system for organizing content.
/// Unlike tags, categories typically represent broader content groupings and are often used for navigation structure.
/// </remarks>
public interface IPagePropertyCategories
{
    /// <summary>
    /// Gets or sets the collection of categories assigned to this page.
    /// </summary>
    /// <value>
    /// A read-only set of category names. Using a set ensures each category appears only once and provides efficient membership testing.
    /// Categories are typically used for content organization, navigation menus, and filtering content by topic or section.
    /// </value>
    IReadOnlySet<string> Categories { get; set; }
}
