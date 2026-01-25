namespace NetEvolve.ForgingBlazor.Routing.Configurations;

using System;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Routing;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Implements the segment configuration fluent API.
/// </summary>
internal sealed class SegmentConfiguration : ISegmentConfiguration
{
    private readonly RoutingBuilderState _state;
    private readonly SegmentConfigurationBuilderState _segment;

    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentConfiguration"/> class.
    /// </summary>
    /// <param name="state">The shared routing builder state.</param>
    /// <param name="segment">The segment configuration state.</param>
    internal SegmentConfiguration(RoutingBuilderState state, SegmentConfigurationBuilderState segment)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(segment);

        _state = state;
        _segment = segment;
    }

    /// <inheritdoc />
    public ISegmentConfiguration WithPagination(Action<IPaginationConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var paginationState = _segment.EnsurePagination();
        var configuration = new PaginationConfiguration(paginationState);
        configure(configuration);
        return this;
    }

    /// <inheritdoc />
    public ISegmentConfiguration WithContentType<T>()
        where T : ContentDescriptor
    {
        _segment.ContentType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public ISegmentConfiguration WithIndexComponent<T>()
        where T : IComponent
    {
        _segment.IndexComponentType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public ISegmentConfiguration WithIndexLayout<T>()
        where T : LayoutComponentBase
    {
        _segment.IndexLayoutType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public ISegmentConfiguration WithPageComponent<T>()
        where T : IComponent
    {
        _segment.PageComponentType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public ISegmentConfiguration WithPageLayout<T>()
        where T : LayoutComponentBase
    {
        _segment.PageLayoutType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public ISegmentConfiguration WithMetadata(Action<IMetadataConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new MetadataConfiguration(_segment.Metadata);
        configure(configuration);
        return this;
    }

    /// <inheritdoc />
    public ISegmentConfiguration MapSegment(string name, Action<ISegmentConfiguration> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(configure);

        var (fullPath, segments) = RoutingPathUtility.NormalizeSegmentPath(name, _segment.PathSegments);
        var nestedState = _state.GetOrAddSegment(fullPath, segments);
        var nestedConfiguration = new SegmentConfiguration(_state, nestedState);
        configure(nestedConfiguration);
        return nestedConfiguration;
    }
}
