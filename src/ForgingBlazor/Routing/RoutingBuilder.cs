namespace NetEvolve.ForgingBlazor.Routing;

using System;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Routing.Configurations;

/// <summary>
/// Implements the <see cref="IRoutingBuilder"/> fluent API.
/// </summary>
internal sealed class RoutingBuilder : IRoutingBuilder
{
    private readonly RoutingBuilderState _state = new();

    /// <summary>
    /// Gets the underlying mutable state. Intended for testing.
    /// </summary>
    internal RoutingBuilderState State => _state;

    /// <inheritdoc />
    public IRootConfiguration ConfigureRoot(Action<IRootConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new RootConfiguration(_state);
        configure(configuration);
        return configuration;
    }

    /// <inheritdoc />
    public ISegmentConfiguration MapSegment(string name, Action<ISegmentConfiguration> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(configure);

        var (fullPath, segments) = RoutingPathUtility.NormalizeSegmentPath(name, null);
        var segmentState = _state.GetOrAddSegment(fullPath, segments);
        var configuration = new SegmentConfiguration(_state, segmentState);
        configure(configuration);
        return configuration;
    }

    /// <inheritdoc />
    public IPageConfiguration MapPage(string slug, Action<IPageConfiguration> configure)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentNullException.ThrowIfNull(configure);

        var (fullPath, segments, normalizedSlug) = RoutingPathUtility.NormalizePagePath(slug);
        var pageState = _state.AddPage(fullPath, segments, normalizedSlug);
        var configuration = new PageConfiguration(pageState);
        configure(configuration);
        return configuration;
    }

    /// <summary>
    /// Builds an immutable <see cref="RoutingConfiguration"/> snapshot based on the current state.
    /// </summary>
    /// <returns>The immutable routing configuration.</returns>
    internal RoutingConfiguration Build() => _state.BuildConfiguration();
}
