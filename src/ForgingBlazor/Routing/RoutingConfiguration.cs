namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using global::NetEvolve.ForgingBlazor;

/// <summary>
/// Represents the immutable result of the routing FluentAPI configuration.
/// </summary>
internal sealed class RoutingConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoutingConfiguration"/> class.
    /// </summary>
    /// <param name="root">The root configuration snapshot.</param>
    /// <param name="segments">The segment configuration snapshots keyed by their normalized path.</param>
    /// <param name="pages">The page configuration snapshots keyed by their normalized path.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/>, <paramref name="segments"/>, or <paramref name="pages"/> are <see langword="null"/>.</exception>
    internal RoutingConfiguration(
        RootConfigurationSnapshot root,
        IReadOnlyDictionary<string, SegmentConfigurationSnapshot> segments,
        IReadOnlyDictionary<string, PageConfigurationSnapshot> pages
    )
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(segments);
        ArgumentNullException.ThrowIfNull(pages);

        Root = root;
        Segments = segments.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
        Pages = pages.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the root configuration snapshot.
    /// </summary>
    internal RootConfigurationSnapshot Root { get; }

    /// <summary>
    /// Gets the configured segments keyed by their normalized path.
    /// </summary>
    internal IReadOnlyDictionary<string, SegmentConfigurationSnapshot> Segments { get; }

    /// <summary>
    /// Gets the configured pages keyed by their normalized path.
    /// </summary>
    internal IReadOnlyDictionary<string, PageConfigurationSnapshot> Pages { get; }
}

/// <summary>
/// Represents the root-level routing configuration snapshot.
/// </summary>
/// <param name="DefaultContentType">The default content descriptor type.</param>
/// <param name="DefaultComponentType">The default component type for index rendering.</param>
/// <param name="DefaultLayoutType">The default layout component type.</param>
/// <param name="HomePageContentType">The content descriptor type for the home page.</param>
/// <param name="Culture">The culture configuration snapshot.</param>
internal sealed record RootConfigurationSnapshot(
    Type? DefaultContentType,
    Type? DefaultComponentType,
    Type? DefaultLayoutType,
    Type? HomePageContentType,
    CultureConfigurationSnapshot Culture
);

/// <summary>
/// Represents the culture configuration snapshot.
/// </summary>
/// <param name="DefaultCulture">The default culture.</param>
/// <param name="Canonical">The canonical format to use for the default culture.</param>
/// <param name="SupportedCultures">The supported cultures.</param>
internal sealed record CultureConfigurationSnapshot(
    CultureInfo? DefaultCulture,
    CultureCanonical Canonical,
    IReadOnlyList<CultureInfo> SupportedCultures
);

/// <summary>
/// Represents a content segment configuration snapshot.
/// </summary>
/// <param name="FullPath">The normalized full path of the segment.</param>
/// <param name="PathSegments">The individual path segments.</param>
/// <param name="ContentType">The content descriptor type associated with the segment.</param>
/// <param name="IndexComponentType">The component used to render the segment index page.</param>
/// <param name="IndexLayoutType">The layout used for the segment index page.</param>
/// <param name="PageComponentType">The component used to render child content pages.</param>
/// <param name="PageLayoutType">The layout used to render child content pages.</param>
/// <param name="Pagination">The pagination settings snapshot.</param>
/// <param name="Metadata">The metadata configuration snapshot.</param>
internal sealed record SegmentConfigurationSnapshot(
    string FullPath,
    IReadOnlyList<string> PathSegments,
    Type? ContentType,
    Type? IndexComponentType,
    Type? IndexLayoutType,
    Type? PageComponentType,
    Type? PageLayoutType,
    PaginationConfigurationSnapshot? Pagination,
    MetadataConfigurationSnapshot Metadata
);

/// <summary>
/// Represents a page configuration snapshot.
/// </summary>
/// <param name="FullPath">The normalized full path of the page.</param>
/// <param name="PathSegments">The path segments including the slug as the final element.</param>
/// <param name="Slug">The slug of the page.</param>
/// <param name="ContentType">The content descriptor type for the page.</param>
/// <param name="ComponentType">The component used to render the page.</param>
/// <param name="LayoutType">The layout used to render the page.</param>
/// <param name="Metadata">The metadata configuration snapshot.</param>
internal sealed record PageConfigurationSnapshot(
    string FullPath,
    IReadOnlyList<string> PathSegments,
    string Slug,
    Type? ContentType,
    Type? ComponentType,
    Type? LayoutType,
    MetadataConfigurationSnapshot Metadata
);

/// <summary>
/// Represents pagination settings for a segment.
/// </summary>
/// <param name="PageSize">The number of items per page.</param>
/// <param name="Format">The pagination URL format.</param>
/// <param name="Prefix">The optional prefix used when <paramref name="Format"/> is <see cref="PaginationUrlFormat.Prefixed"/>.</param>
internal sealed record PaginationConfigurationSnapshot(int PageSize, PaginationUrlFormat Format, string? Prefix);

/// <summary>
/// Represents metadata configuration entries.
/// </summary>
/// <param name="Fields">The configured metadata fields keyed by field name.</param>
internal sealed record MetadataConfigurationSnapshot(IReadOnlyDictionary<string, MetadataFieldSnapshot> Fields);

/// <summary>
/// Represents an individual metadata field configuration.
/// </summary>
/// <param name="Name">The metadata field name.</param>
/// <param name="FieldType">The metadata field type.</param>
/// <param name="DefaultValue">The default value for the metadata field.</param>
internal sealed record MetadataFieldSnapshot(string Name, Type FieldType, object? DefaultValue);
