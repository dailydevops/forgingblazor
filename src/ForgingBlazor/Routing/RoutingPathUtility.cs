namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Collections.Generic;
using System.Linq;
using global::NetEvolve.ForgingBlazor;

/// <summary>
/// Provides helper methods to normalize and validate routing paths.
/// </summary>
internal static class RoutingPathUtility
{
    private const StringSplitOptions SplitOptions =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;

    /// <summary>
    /// Normalizes a segment path and optionally composes it with parent segments.
    /// </summary>
    /// <param name="path">The segment path to normalize.</param>
    /// <param name="parentSegments">Optional parent path segments to prepend.</param>
    /// <returns>The normalized full path and its individual segments.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is invalid.</exception>
    internal static (string FullPath, string[] Segments) NormalizeSegmentPath(
        string path,
        IReadOnlyList<string>? parentSegments
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var segments = path.Split('/', SplitOptions);
        if (segments.Length == 0)
        {
            throw new ArgumentException("The segment path must contain at least one valid segment.", nameof(path));
        }

        var invalidSegment = segments.FirstOrDefault(segment => !Check.IsValidPathSegment(segment));
        if (invalidSegment is not null)
        {
            throw new ArgumentException(
                $"The segment '{invalidSegment}' is not valid. Segments must contain only letters, digits, hyphens, or underscores and be between {Defaults.SegmentLengthMinimum} and {Defaults.SegmentLengthMaximum} characters.",
                nameof(path)
            );
        }

        if (parentSegments is null || parentSegments.Count == 0)
        {
            var normalized = segments.ToArray();
            return (string.Join('/', normalized), normalized);
        }

        var combined = new string[parentSegments.Count + segments.Length];
        for (var index = 0; index < parentSegments.Count; index++)
        {
            combined[index] = parentSegments[index];
        }

        for (var index = 0; index < segments.Length; index++)
        {
            combined[parentSegments.Count + index] = segments[index];
        }

        return (string.Join('/', combined), combined);
    }

    /// <summary>
    /// Normalizes a page path and validates its slug.
    /// </summary>
    /// <param name="path">The page path containing optional segments and the slug.</param>
    /// <returns>The normalized full path, its segments, and the slug.</returns>
    /// <exception cref="ArgumentException">Thrown when the path or slug are invalid.</exception>
    internal static (string FullPath, string[] Segments, string Slug) NormalizePagePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var segments = path.Split('/', SplitOptions);
        if (segments.Length == 0)
        {
            throw new ArgumentException("The page path must contain a slug or segment/slug combination.", nameof(path));
        }

        if (segments.Length > 1)
        {
            var invalidParentSegment = segments
                .Take(segments.Length - 1)
                .FirstOrDefault(segment => !Check.IsValidPathSegment(segment));

            if (invalidParentSegment is not null)
            {
                throw new ArgumentException(
                    $"The segment '{invalidParentSegment}' is not valid. Segments must contain only letters, digits, hyphens, or underscores and be between {Defaults.SegmentLengthMinimum} and {Defaults.SegmentLengthMaximum} characters.",
                    nameof(path)
                );
            }
        }

        var slug = Check.ValidateSlug(segments[^1], nameof(path));

        var normalizedSegments = segments.ToArray();
        normalizedSegments[^1] = slug;
        return (string.Join('/', normalizedSegments), normalizedSegments, slug);
    }
}
