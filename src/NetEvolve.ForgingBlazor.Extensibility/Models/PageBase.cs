namespace NetEvolve.ForgingBlazor.Extensibility.Models;

using YamlDotNet.Serialization;

/// <summary>
/// Provides the base implementation for all page types in the ForgingBlazor framework.
/// </summary>
/// <remarks>
/// This abstract record defines the fundamental properties that all pages must have, including slug, title, and optional link title.
/// All page types should inherit from this class to ensure consistent core functionality and metadata across the application.
/// </remarks>
public abstract record PageBase
{
    /// <summary>
    /// Gets or sets the unique URL-friendly identifier for the page.
    /// </summary>
    /// <value>
    /// A URL-safe string that uniquely identifies the page. Must contain only lowercase letters, numbers, and hyphens.
    /// This value is used in URL routing and must be unique across pages of the same type.
    /// </value>
    /// <remarks>
    /// The slug should be lowercase, use hyphens to separate words, and contain no spaces or special characters.
    /// Example: "my-first-blog-post" or "getting-started-guide".
    /// </remarks>
    [YamlMember(
        Alias = "slug",
        Description = "The unique identifier for the page used in URLs. Must be URL-friendly and contain no spaces or special characters. This value should be lowercase and use hyphens to separate words."
    )]
    public required string Slug { get; set; }

    /// <summary>
    /// Gets or sets the primary display title of the page.
    /// </summary>
    /// <value>
    /// The main heading text that appears at the top of the page and in page metadata.
    /// This value is also used for SEO title tags and social media sharing.
    /// </value>
    /// <remarks>
    /// The title should be descriptive and concise for optimal readability and SEO performance.
    /// It appears as the main heading (typically H1) on the page.
    /// </remarks>
    [YamlMember(
        Alias = "title",
        Description = "The primary display title of the page. This appears as the main heading and in page metadata. Should be descriptive and concise for optimal readability."
    )]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets an optional shortened title used in navigation and breadcrumbs.
    /// </summary>
    /// <value>
    /// A shortened or alternative version of the title for use in constrained UI spaces,
    /// or <see langword="null"/> to use the main <see cref="Title"/> as fallback.
    /// </value>
    /// <remarks>
    /// This property is useful when the main title is too long for navigation menus, breadcrumbs, or other UI elements
    /// where space is limited. When not specified, the main title is used throughout the application.
    /// </remarks>
    [YamlMember(
        Alias = "linkTitle",
        Description = "An optional shortened or alternative title used specifically for navigation links and breadcrumbs. When not specified, the main title is used as fallback. Useful for long titles that need truncation in UI elements."
    )]
    public string? LinkTitle { get; set; }
}
