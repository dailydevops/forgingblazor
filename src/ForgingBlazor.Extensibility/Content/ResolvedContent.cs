#pragma warning disable CA1056 // CanonicalUrl is intentionally a string per contract requirements
namespace NetEvolve.ForgingBlazor;

using System.Collections.Generic;
using System.Globalization;

/// <summary>
/// Represents a content item resolved for a specific culture and route context.
/// </summary>
/// <typeparam name="TDescriptor">A content descriptor type derived from <see cref="ContentDescriptor"/>.</typeparam>
public class ResolvedContent<TDescriptor>
    where TDescriptor : ContentDescriptor
{
    /// <summary>
    /// The descriptor containing the content metadata and body.
    /// </summary>
    public required TDescriptor Descriptor { get; init; }

    /// <summary>
    /// The culture used for resolution.
    /// </summary>
    public required CultureInfo Culture { get; init; }

    /// <summary>
    /// The canonical URL for this content instance in the resolved culture.
    /// </summary>
    public required string CanonicalUrl { get; init; }

    /// <summary>
    /// Route values associated with the resolution context (e.g., segment, pagination, parameters).
    /// </summary>
    public required IReadOnlyDictionary<string, object?> RouteValues { get; init; }
}
