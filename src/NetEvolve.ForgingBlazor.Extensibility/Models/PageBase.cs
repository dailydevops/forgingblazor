namespace NetEvolve.ForgingBlazor.Extensibility.Models;

using System;
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

    /// <summary>
    /// Gets or sets the absolute URL of the page including the base URL.
    /// </summary>
    /// <value>
    /// The fully qualified URL of the page, including the protocol, domain, and path.
    /// This value is automatically set during page processing and includes the complete URL structure
    /// (e.g., "https://example.com/blog/my-first-post").
    /// </value>
    /// <remarks>
    /// This property is excluded from YAML serialization and is populated at runtime.
    /// It provides the complete URL for use in metadata, canonical links, and social media sharing tags.
    /// The absolute URL is constructed by combining the site's base URL with the page's relative path.
    /// </remarks>
    [YamlIgnore]
    public string AbsoluteLink { get; internal set; } = default!;

    /// <summary>
    /// Gets or sets the relative URL path of the page from the site root.
    /// </summary>
    /// <value>
    /// The site-relative path of the page starting with a forward slash.
    /// This value is automatically set during page processing and represents the path portion of the URL
    /// without the protocol or domain (e.g., "/blog/my-first-post").
    /// </value>
    /// <remarks>
    /// This property is excluded from YAML serialization and is populated at runtime.
    /// It provides the relative path for use in internal navigation, routing, and link generation.
    /// The relative URL is derived from the page type and slug, following the site's routing conventions.
    /// </remarks>
    [YamlIgnore]
    public string RelativeLink { get; internal set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether this page is an index page (_index.md).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this is an index page; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// This property is excluded from YAML serialization and is set at runtime during content collection.
    /// Index pages are special pages that represent directory-level content, typically named _index.md.
    /// </remarks>
    [YamlIgnore]
    public bool IsIndexPage { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether this page is the home page (root _index.md).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this is the home page; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// This property is excluded from YAML serialization and is set at runtime during content collection.
    /// The home page is the _index.md file located in the root of the content segment directory.
    /// </remarks>
    [YamlIgnore]
    public bool IsHomePage { get; internal set; }

    /// <summary>
    /// Gets or sets the markdown content of the page (excluding YAML front matter).
    /// </summary>
    /// <value>
    /// A <see cref="string"/> containing the markdown content, or <see langword="null"/> if no content is present.
    /// </value>
    /// <remarks>
    /// This property is excluded from YAML serialization and is populated at runtime during content collection.
    /// It contains the page's markdown content that appears after the YAML front matter delimiter.
    /// </remarks>
    [YamlIgnore]
    public string? Content { get; internal set; }

    /// <summary>
    /// Gets or sets the date and time when the page was created.
    /// </summary>
    /// <value>
    /// The creation date and time of the page, or <see langword="null"/> if not set.
    /// </value>
    /// <remarks>
    /// This property is excluded from YAML serialization and is populated at runtime during content collection.
    /// </remarks>
    [YamlIgnore]
    public DateTimeOffset? CreationDate { get; internal set; }

    /// <summary>
    /// Gets or sets the date and time when the page was last modified.
    /// </summary>
    /// <value>
    /// The last modified date and time of the page, or <see langword="null"/> if not set.
    /// </value>
    /// <remarks>
    /// This property is excluded from YAML serialization and is populated at runtime during content collection.
    /// </remarks>
    [YamlIgnore]
    public DateTimeOffset? LastModifiedDate { get; internal set; }
}
