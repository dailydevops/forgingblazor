namespace NetEvolve.ForgingBlazor.Pagination;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides URL parsing services for pagination segments.
/// </summary>
internal static class PaginationUrlParser
{
    /// <summary>
    /// Parses a URL segment to extract the page number based on pagination settings.
    /// </summary>
    /// <param name="segment">The URL segment to parse (e.g., "2" or "page-2").</param>
    /// <param name="settings">The pagination settings defining the URL format.</param>
    /// <param name="pageNumber">When this method returns, contains the parsed page number if successful; otherwise, 0.</param>
    /// <returns><c>true</c> if the segment was successfully parsed; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="segment"/> or <paramref name="settings"/> is null.</exception>
    public static bool TryParse(string segment, PaginationSettings settings, out int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(segment);
        ArgumentNullException.ThrowIfNull(settings);

        return PaginationService.TryParsePageNumber(segment, settings, out pageNumber);
    }

    /// <summary>
    /// Parses a numeric pagination segment (e.g., "2" for page 2).
    /// </summary>
    /// <param name="segment">The URL segment to parse.</param>
    /// <param name="pageNumber">When this method returns, contains the parsed page number if successful; otherwise, 0.</param>
    /// <returns><c>true</c> if the segment was successfully parsed as a numeric page number; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="segment"/> is null.</exception>
    public static bool TryParseNumeric(string segment, out int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(segment);

        pageNumber = 0;

        if (string.IsNullOrWhiteSpace(segment))
        {
            return false;
        }

        // Reject if segment starts with minus sign (negative number)
        if (segment.StartsWith('-'))
        {
            return false;
        }

        return int.TryParse(segment, out pageNumber) && pageNumber > 0;
    }

    /// <summary>
    /// Parses a prefixed pagination segment (e.g., "page-2" for page 2).
    /// </summary>
    /// <param name="segment">The URL segment to parse.</param>
    /// <param name="prefix">The prefix to expect (e.g., "page").</param>
    /// <param name="pageNumber">When this method returns, contains the parsed page number if successful; otherwise, 0.</param>
    /// <returns><c>true</c> if the segment was successfully parsed as a prefixed page number; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="segment"/> or <paramref name="prefix"/> is null.</exception>
    public static bool TryParsePrefixed(string segment, string prefix, out int pageNumber)
    {
        ArgumentNullException.ThrowIfNull(segment);
        ArgumentNullException.ThrowIfNull(prefix);

        pageNumber = 0;

        if (string.IsNullOrWhiteSpace(segment) || string.IsNullOrWhiteSpace(prefix))
        {
            return false;
        }

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
