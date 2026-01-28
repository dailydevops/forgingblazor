namespace NetEvolve.ForgingBlazor.Pagination;

using System.Collections.Generic;

/// <summary>
/// Represents a paginated result containing a subset of items and pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the paginated result.</typeparam>
/// <param name="Items">The items for the current page.</param>
/// <param name="CurrentPage">The current page number (1-based).</param>
/// <param name="TotalPages">The total number of pages.</param>
/// <param name="TotalItems">The total number of items across all pages.</param>
/// <param name="HasPrevious">Indicates whether a previous page exists.</param>
/// <param name="HasNext">Indicates whether a next page exists.</param>
public sealed record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int CurrentPage,
    int TotalPages,
    int TotalItems,
    bool HasPrevious,
    bool HasNext
);
