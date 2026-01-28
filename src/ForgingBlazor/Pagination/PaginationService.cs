namespace NetEvolve.ForgingBlazor.Pagination;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides pagination services for calculating page ranges, validating page numbers, and generating page URLs.
/// </summary>
internal static class PaginationService
{
    /// <summary>
    /// Creates a paginated result from a collection of items.
    /// </summary>
    /// <typeparam name="T">The type of items to paginate.</typeparam>
    /// <param name="allItems">The complete collection of items to paginate.</param>
    /// <param name="currentPage">The current page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A <see cref="PaginatedResult{T}"/> containing the items for the current page and pagination metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="allItems"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="currentPage"/> is less than 1 or <paramref name="pageSize"/> is less than 1.</exception>
    public static PaginatedResult<T> CreatePaginatedResult<T>(IReadOnlyList<T> allItems, int currentPage, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(allItems);

        if (currentPage < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(currentPage), currentPage, "Page number must be at least 1.");
        }

        if (pageSize < Defaults.PageSizeMinimum)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                pageSize,
                $"Page size must be at least {Defaults.PageSizeMinimum}."
            );
        }

        var totalItems = allItems.Count;
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Empty collection results in 1 total page but 0 items
        if (totalItems == 0)
        {
            return new PaginatedResult<T>(
                Array.Empty<T>(),
                CurrentPage: 1,
                TotalPages: 1,
                TotalItems: 0,
                HasPrevious: false,
                HasNext: false
            );
        }

        var skip = (currentPage - 1) * pageSize;
        var items = new List<T>();

        for (var i = skip; i < Math.Min(skip + pageSize, totalItems); i++)
        {
            items.Add(allItems[i]);
        }

        return new PaginatedResult<T>(
            items,
            CurrentPage: currentPage,
            TotalPages: totalPages,
            TotalItems: totalItems,
            HasPrevious: currentPage > 1,
            HasNext: currentPage < totalPages
        );
    }

    /// <summary>
    /// Validates whether a page number is within valid range for the total number of items.
    /// </summary>
    /// <param name="pageNumber">The page number to validate (1-based).</param>
    /// <param name="totalItems">The total number of items.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns><c>true</c> if the page number is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidPageNumber(int pageNumber, int totalItems, int pageSize)
    {
        if (pageNumber < 1)
        {
            return false;
        }

        if (totalItems == 0)
        {
            return pageNumber == 1;
        }

        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        return pageNumber <= totalPages;
    }

    /// <summary>
    /// Generates a page URL based on the pagination settings and page number.
    /// </summary>
    /// <param name="basePath">The base path of the segment (e.g., "/posts").</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="settings">The pagination settings.</param>
    /// <returns>The generated page URL.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="basePath"/> or <paramref name="settings"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pageNumber"/> is less than 1.</exception>
    [return: NotNullIfNotNull(nameof(basePath))]
    public static string GeneratePageUrl(string basePath, int pageNumber, PaginationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(basePath);
        ArgumentNullException.ThrowIfNull(settings);

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number must be at least 1.");
        }

        // Page 1 is canonical at the base path
        if (pageNumber == 1)
        {
            return basePath;
        }

        return settings.UrlFormat switch
        {
            PaginationUrlFormat.Numeric => $"{basePath}/{pageNumber}",
            PaginationUrlFormat.Prefixed => $"{basePath}/{settings.Prefix ?? "page"}-{pageNumber}",
            _ => throw new ArgumentException(
                $"Unsupported pagination URL format: {settings.UrlFormat}",
                nameof(settings)
            ),
        };
    }

    /// <summary>
    /// Extracts the page number from a URL segment based on the pagination format.
    /// </summary>
    /// <param name="segment">The URL segment to parse (e.g., "2" or "page-2").</param>
    /// <param name="settings">The pagination settings.</param>
    /// <param name="pageNumber">When this method returns, contains the page number if parsing succeeded; otherwise, 0.</param>
    /// <returns><c>true</c> if the segment was successfully parsed; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="segment"/> or <paramref name="settings"/> is null.</exception>
    public static bool TryParsePageNumber(string segment, PaginationSettings settings, out int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(segment);
        ArgumentNullException.ThrowIfNull(settings);

        pageNumber = 0;

        if (string.IsNullOrWhiteSpace(segment))
        {
            return false;
        }

        return settings.UrlFormat switch
        {
            PaginationUrlFormat.Numeric => TryParseNumericPageNumber(segment, out pageNumber),
            PaginationUrlFormat.Prefixed => TryParsePrefixedPageNumber(
                segment,
                settings.Prefix ?? "page",
                out pageNumber
            ),
            _ => false,
        };
    }

    private static bool TryParseNumericPageNumber(string segment, out int pageNumber)
    {
        pageNumber = 0;

        // Reject if segment starts with minus sign (negative number)
        if (segment.StartsWith('-'))
        {
            return false;
        }

        if (!int.TryParse(segment, out var parsed))
        {
            return false;
        }

        if (parsed <= 0)
        {
            return false;
        }

        pageNumber = parsed;
        return true;
    }

    private static bool TryParsePrefixedPageNumber(string segment, string prefix, out int pageNumber)
    {
        pageNumber = 0;

        var expectedPrefix = $"{prefix}-";
        if (
            !segment.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase)
            || segment.Length <= expectedPrefix.Length
        )
        {
            return false;
        }

        var numberPart = segment.Substring(expectedPrefix.Length);

        // Reject if number part starts with minus sign (negative number)
        if (numberPart.StartsWith('-'))
        {
            return false;
        }

        if (!int.TryParse(numberPart, out var parsed))
        {
            return false;
        }

        if (parsed <= 0)
        {
            return false;
        }

        pageNumber = parsed;
        return true;
    }
}
