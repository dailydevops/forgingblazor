namespace NetEvolve.ForgingBlazor.Extensibility.Models;

using System.Collections.Generic;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides the base implementation for blog post pages with common blogging metadata.
/// </summary>
/// <remarks>
/// This abstract record extends <see cref="PageBase"/> and implements common blogging interfaces
/// to provide publication date, author attribution, and tagging functionality.
/// All blog post types should inherit from this class to ensure consistent blog-specific properties.
/// </remarks>
public abstract record BlogPostBase : PageBase, IPagePropertyPublishedOn, IPagePropertyAuthor, IPagePropertyTags
{
    /// <inheritdoc />
    public DateTimeOffset? PublishedOn { get; set; }

    /// <inheritdoc />
    public string? Author { get; set; }

    /// <inheritdoc />
    public IReadOnlySet<string>? Tags { get; set; }
}
