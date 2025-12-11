namespace NetEvolve.ForgingBlazor.Models;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Represents a concrete blog post implementation for the ForgingBlazor framework.
/// </summary>
/// <remarks>
/// This sealed record provides the standard blog post type with all common blogging properties inherited from <see cref="BlogPostBase"/>,
/// including slug, title, publication date, author, and tags. Use this type for typical blog post scenarios
/// where no additional custom properties are required.
/// </remarks>
public sealed record BlogPost : BlogPostBase { }
