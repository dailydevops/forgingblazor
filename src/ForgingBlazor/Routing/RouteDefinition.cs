namespace NetEvolve.ForgingBlazor.Routing;

using System;
using NetEvolve.ForgingBlazor.Pagination;

/// <summary>
/// Represents a route definition containing path pattern, component type, layout type, content type, pagination settings, and parent reference.
/// </summary>
/// <param name="PathPattern">The path pattern for this route (e.g., "/posts", "/about").</param>
/// <param name="ComponentType">The Blazor component type to render for this route.</param>
/// <param name="LayoutType">The layout component type for this route, or null to use default.</param>
/// <param name="ContentType">The content descriptor type for this route, or null if not content-based.</param>
/// <param name="PaginationSettings">The pagination settings for this route, or null if not paginated.</param>
/// <param name="Parent">The parent route definition, or null if this is a root route.</param>
internal sealed record RouteDefinition(
    string PathPattern,
    Type ComponentType,
    Type? LayoutType,
    Type? ContentType,
    PaginationSettings? PaginationSettings,
    RouteDefinition? Parent
);
