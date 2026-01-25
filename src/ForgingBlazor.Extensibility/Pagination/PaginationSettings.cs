namespace NetEvolve.ForgingBlazor.Pagination;

/// <summary>
/// Represents pagination settings for a route or segment.
/// </summary>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="UrlFormat">The URL format for pagination.</param>
/// <param name="Prefix">The prefix for prefixed pagination format (e.g., "page" for "/page-2"), or null for numeric format.</param>
public sealed record PaginationSettings(int PageSize, PaginationUrlFormat UrlFormat, string? Prefix);
