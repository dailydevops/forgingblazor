namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NetEvolve.ForgingBlazor;

/// <summary>
/// Provides mutable builder state used while configuring routing via the FluentAPI.
/// </summary>
internal sealed class RoutingBuilderState
{
    private readonly Dictionary<string, SegmentConfigurationBuilderState> _segments = new(
        StringComparer.OrdinalIgnoreCase
    );

    private readonly Dictionary<string, PageConfigurationBuilderState> _pages = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the root configuration builder state.
    /// </summary>
    internal RootConfigurationBuilderState Root { get; } = new();

    /// <summary>
    /// Gets the configured segments keyed by their normalized full path.
    /// </summary>
    internal IReadOnlyDictionary<string, SegmentConfigurationBuilderState> Segments => _segments;

    /// <summary>
    /// Gets the configured pages keyed by their normalized full path.
    /// </summary>
    internal IReadOnlyDictionary<string, PageConfigurationBuilderState> Pages => _pages;

    /// <summary>
    /// Gets an existing segment or creates a new segment for the specified normalized path.
    /// </summary>
    /// <param name="fullPath">The normalized full path.</param>
    /// <param name="pathSegments">The path segments that compose the full path.</param>
    /// <returns>The <see cref="SegmentConfigurationBuilderState"/> for the path.</returns>
    internal SegmentConfigurationBuilderState GetOrAddSegment(string fullPath, IReadOnlyList<string> pathSegments)
    {
        if (_segments.TryGetValue(fullPath, out var existing))
        {
            return existing;
        }

        var state = new SegmentConfigurationBuilderState(fullPath, pathSegments);
        _segments.Add(fullPath, state);
        return state;
    }

    /// <summary>
    /// Adds a new page for the specified normalized path.
    /// </summary>
    /// <param name="fullPath">The normalized full path.</param>
    /// <param name="pathSegments">The path segments including the slug.</param>
    /// <param name="slug">The slug of the page.</param>
    /// <returns>The <see cref="PageConfigurationBuilderState"/> created for the path.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a page with the same path was already configured.</exception>
    internal PageConfigurationBuilderState AddPage(string fullPath, IReadOnlyList<string> pathSegments, string slug)
    {
        if (_pages.ContainsKey(fullPath))
        {
            throw new InvalidOperationException(
                $"The page '{fullPath}' has already been configured. Duplicate page paths are not allowed."
            );
        }

        var state = new PageConfigurationBuilderState(fullPath, pathSegments, slug);
        _pages.Add(fullPath, state);
        return state;
    }

    /// <summary>
    /// Creates an immutable <see cref="RoutingConfiguration"/> snapshot from the builder state.
    /// </summary>
    /// <returns>The immutable routing configuration.</returns>
    internal RoutingConfiguration BuildConfiguration()
    {
        var segments = _segments.ToDictionary(
            static kvp => kvp.Key,
            kvp => kvp.Value.ToSnapshot(),
            StringComparer.OrdinalIgnoreCase
        );

        var pages = _pages.ToDictionary(
            static kvp => kvp.Key,
            kvp => kvp.Value.ToSnapshot(),
            StringComparer.OrdinalIgnoreCase
        );

        return new RoutingConfiguration(Root.ToSnapshot(), segments, pages);
    }
}

/// <summary>
/// Represents mutable root configuration state.
/// </summary>
internal sealed class RootConfigurationBuilderState
{
    /// <summary>
    /// Gets or sets the default content descriptor type.
    /// </summary>
    internal Type? DefaultContentType { get; set; }

    /// <summary>
    /// Gets or sets the default component type for index rendering.
    /// </summary>
    internal Type? DefaultComponentType { get; set; }

    /// <summary>
    /// Gets or sets the default layout component type.
    /// </summary>
    internal Type? DefaultLayoutType { get; set; }

    /// <summary>
    /// Gets or sets the content descriptor type used for the home page.
    /// </summary>
    internal Type? HomePageContentType { get; set; }

    /// <summary>
    /// Gets the culture configuration state.
    /// </summary>
    internal CultureConfigurationBuilderState Culture { get; } = new();

    /// <summary>
    /// Creates an immutable snapshot of the current state.
    /// </summary>
    /// <returns>The root configuration snapshot.</returns>
    internal RootConfigurationSnapshot ToSnapshot() =>
        new(DefaultContentType, DefaultComponentType, DefaultLayoutType, HomePageContentType, Culture.ToSnapshot());
}

/// <summary>
/// Represents mutable culture configuration state.
/// </summary>
internal sealed class CultureConfigurationBuilderState
{
    private readonly Dictionary<string, CultureInfo> _supportedCultures = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the default culture.
    /// </summary>
    internal CultureInfo? DefaultCulture { get; private set; }

    /// <summary>
    /// Gets or sets the canonical format for the default culture. Defaults to <see cref="CultureCanonical.WithoutPrefix"/>.
    /// </summary>
    internal CultureCanonical Canonical { get; set; } = CultureCanonical.WithoutPrefix;

    /// <summary>
    /// Sets the default culture and ensures it is part of the supported cultures collection.
    /// </summary>
    /// <param name="culture">The culture to set as default.</param>
    internal void SetDefaultCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        DefaultCulture = culture;
        _supportedCultures[culture.Name] = culture;
    }

    /// <summary>
    /// Adds a supported culture.
    /// </summary>
    /// <param name="culture">The supported culture.</param>
    internal void AddSupportedCulture(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        _supportedCultures[culture.Name] = culture;
    }

    /// <summary>
    /// Creates an immutable snapshot of the culture configuration.
    /// </summary>
    /// <returns>The culture configuration snapshot.</returns>
    internal CultureConfigurationSnapshot ToSnapshot()
    {
        var supported = _supportedCultures
            .Values.OrderBy(static culture => culture.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new CultureConfigurationSnapshot(DefaultCulture, Canonical, supported);
    }
}

/// <summary>
/// Represents mutable segment configuration state.
/// </summary>
internal sealed class SegmentConfigurationBuilderState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentConfigurationBuilderState"/> class.
    /// </summary>
    /// <param name="fullPath">The normalized full path.</param>
    /// <param name="pathSegments">The path segments composing the full path.</param>
    internal SegmentConfigurationBuilderState(string fullPath, IReadOnlyList<string> pathSegments)
    {
        ArgumentNullException.ThrowIfNull(fullPath);
        ArgumentNullException.ThrowIfNull(pathSegments);

        FullPath = fullPath;
        PathSegments = pathSegments;
        Metadata = new MetadataConfigurationBuilderState();
    }

    /// <summary>
    /// Gets the normalized full path of the segment.
    /// </summary>
    internal string FullPath { get; }

    /// <summary>
    /// Gets the individual path segments.
    /// </summary>
    internal IReadOnlyList<string> PathSegments { get; }

    /// <summary>
    /// Gets or sets the content descriptor type.
    /// </summary>
    internal Type? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the index component type.
    /// </summary>
    internal Type? IndexComponentType { get; set; }

    /// <summary>
    /// Gets or sets the index layout component type.
    /// </summary>
    internal Type? IndexLayoutType { get; set; }

    /// <summary>
    /// Gets or sets the page component type.
    /// </summary>
    internal Type? PageComponentType { get; set; }

    /// <summary>
    /// Gets or sets the page layout component type.
    /// </summary>
    internal Type? PageLayoutType { get; set; }

    /// <summary>
    /// Gets the pagination configuration state if configured.
    /// </summary>
    internal PaginationConfigurationBuilderState? Pagination { get; private set; }

    /// <summary>
    /// Gets the metadata configuration state.
    /// </summary>
    internal MetadataConfigurationBuilderState Metadata { get; }

    /// <summary>
    /// Ensures the pagination state exists and returns it.
    /// </summary>
    /// <returns>The pagination configuration state.</returns>
    internal PaginationConfigurationBuilderState EnsurePagination() =>
        Pagination ??= new PaginationConfigurationBuilderState();

    /// <summary>
    /// Creates an immutable snapshot of the segment configuration.
    /// </summary>
    /// <returns>The segment configuration snapshot.</returns>
    internal SegmentConfigurationSnapshot ToSnapshot() =>
        new(
            FullPath,
            PathSegments,
            ContentType,
            IndexComponentType,
            IndexLayoutType,
            PageComponentType,
            PageLayoutType,
            Pagination?.ToSnapshot(),
            Metadata.ToSnapshot()
        );
}

/// <summary>
/// Represents mutable page configuration state.
/// </summary>
internal sealed class PageConfigurationBuilderState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PageConfigurationBuilderState"/> class.
    /// </summary>
    /// <param name="fullPath">The normalized full path.</param>
    /// <param name="pathSegments">The path segments composing the full path.</param>
    /// <param name="slug">The slug for the page.</param>
    internal PageConfigurationBuilderState(string fullPath, IReadOnlyList<string> pathSegments, string slug)
    {
        ArgumentNullException.ThrowIfNull(fullPath);
        ArgumentNullException.ThrowIfNull(pathSegments);
        ArgumentNullException.ThrowIfNull(slug);

        FullPath = fullPath;
        PathSegments = pathSegments;
        Slug = slug;
        Metadata = new MetadataConfigurationBuilderState();
    }

    /// <summary>
    /// Gets the normalized full path of the page.
    /// </summary>
    internal string FullPath { get; }

    /// <summary>
    /// Gets the path segments including the slug.
    /// </summary>
    internal IReadOnlyList<string> PathSegments { get; }

    /// <summary>
    /// Gets the slug.
    /// </summary>
    internal string Slug { get; }

    /// <summary>
    /// Gets or sets the content descriptor type.
    /// </summary>
    internal Type? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the component type.
    /// </summary>
    internal Type? ComponentType { get; set; }

    /// <summary>
    /// Gets or sets the layout type.
    /// </summary>
    internal Type? LayoutType { get; set; }

    /// <summary>
    /// Gets the metadata configuration state.
    /// </summary>
    internal MetadataConfigurationBuilderState Metadata { get; }

    /// <summary>
    /// Creates an immutable snapshot of the page configuration.
    /// </summary>
    /// <returns>The page configuration snapshot.</returns>
    internal PageConfigurationSnapshot ToSnapshot() =>
        new(FullPath, PathSegments, Slug, ContentType, ComponentType, LayoutType, Metadata.ToSnapshot());
}

/// <summary>
/// Represents mutable pagination configuration state.
/// </summary>
internal sealed class PaginationConfigurationBuilderState
{
    /// <summary>
    /// Gets or sets the page size. Defaults to <see cref="Defaults.PageSizeDefault"/>.
    /// </summary>
    internal int PageSize { get; set; } = Defaults.PageSizeDefault;

    /// <summary>
    /// Gets or sets the pagination URL format. Defaults to <see cref="PaginationUrlFormat.Numeric"/>.
    /// </summary>
    internal PaginationUrlFormat Format { get; set; } = PaginationUrlFormat.Numeric;

    /// <summary>
    /// Gets or sets the optional prefix used for prefixed pagination format.
    /// </summary>
    internal string? Prefix { get; set; }

    /// <summary>
    /// Creates an immutable snapshot of the pagination configuration.
    /// </summary>
    /// <returns>The pagination configuration snapshot.</returns>
    internal PaginationConfigurationSnapshot ToSnapshot() => new(PageSize, Format, Prefix);
}

/// <summary>
/// Represents mutable metadata configuration state.
/// </summary>
internal sealed class MetadataConfigurationBuilderState
{
    private readonly Dictionary<string, MetadataFieldBuilderState> _fields = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Adds a metadata field definition.
    /// </summary>
    /// <param name="fieldName">The metadata field name.</param>
    /// <param name="fieldType">The metadata field type.</param>
    /// <param name="defaultValue">The default value (optional).</param>
    /// <exception cref="InvalidOperationException">Thrown when the field name was already configured.</exception>
    internal void AddField(string fieldName, Type fieldType, object? defaultValue)
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(fieldType);

        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("The metadata field name must not be empty.", nameof(fieldName));
        }

        if (!_fields.TryAdd(fieldName, new MetadataFieldBuilderState(fieldName, fieldType, defaultValue)))
        {
            throw new InvalidOperationException(
                $"The metadata field '{fieldName}' has already been configured. Duplicate field names are not allowed."
            );
        }
    }

    /// <summary>
    /// Creates an immutable snapshot of the metadata configuration.
    /// </summary>
    /// <returns>The metadata configuration snapshot.</returns>
    internal MetadataConfigurationSnapshot ToSnapshot()
    {
        var fields = _fields.ToDictionary(
            static kvp => kvp.Key,
            kvp => kvp.Value.ToSnapshot(),
            StringComparer.OrdinalIgnoreCase
        );

        return new MetadataConfigurationSnapshot(fields);
    }
}

/// <summary>
/// Represents mutable metadata field state.
/// </summary>
/// <param name="Name">The metadata field name.</param>
/// <param name="FieldType">The metadata field type.</param>
/// <param name="DefaultValue">The default value.</param>
internal sealed record MetadataFieldBuilderState(string Name, Type FieldType, object? DefaultValue)
{
    /// <summary>
    /// Creates an immutable snapshot of the metadata field configuration.
    /// </summary>
    /// <returns>The metadata field snapshot.</returns>
    internal MetadataFieldSnapshot ToSnapshot() => new(Name, FieldType, DefaultValue);
}
