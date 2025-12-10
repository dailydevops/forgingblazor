namespace NetEvolve.ForgingBlazor.Core.Models;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Represents a concrete page implementation for the ForgingBlazor framework.
/// </summary>
/// <remarks>
/// This sealed record provides the standard page type with all fundamental page properties inherited from <see cref="PageBase"/>,
/// including slug, title, and optional link title. Use this type for general-purpose pages
/// where no additional custom properties or blog-specific metadata (such as publication date, author, or tags) are required.
/// For blog posts, use <see cref="BlogPost"/> instead.
/// </remarks>
public sealed record Page : PageBase { }
