namespace NetEvolve.ForgingBlazor.Routing.Configurations;

using System;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Routing;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Implements the root-level routing configuration fluent API.
/// </summary>
internal sealed class RootConfiguration : IRootConfiguration
{
    private readonly RoutingBuilderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootConfiguration"/> class.
    /// </summary>
    /// <param name="state">The shared routing builder state.</param>
    internal RootConfiguration(RoutingBuilderState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    /// <inheritdoc />
    public ICultureConfiguration WithCulture(Action<ICultureConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new CultureConfiguration(_state.Root.Culture);
        configure(configuration);
        return configuration;
    }

    /// <inheritdoc />
    public IRootConfiguration WithDefaultContentType<T>()
        where T : ContentDescriptor
    {
        _state.Root.DefaultContentType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public IRootConfiguration WithDefaultComponent<T>()
        where T : IComponent
    {
        _state.Root.DefaultComponentType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public IRootConfiguration WithDefaultLayout<T>()
        where T : LayoutComponentBase
    {
        _state.Root.DefaultLayoutType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public IRootConfiguration WithHomePage<T>()
        where T : ContentDescriptor
    {
        _state.Root.HomePageContentType = typeof(T);
        return this;
    }
}
