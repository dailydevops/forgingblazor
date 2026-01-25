namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Internal class implementing Blazor <see cref="IComponent"/> as dynamic router resolving content and rendering configured component.
/// </summary>
internal sealed class ContentRouteHandler : IComponent
{
    private RenderHandle _renderHandle;
    private RouteDefinition? _routeDefinition;

    /// <summary>
    /// Gets or sets the route definition to render.
    /// </summary>
    [Parameter]
    public RouteDefinition? RouteDefinition
    {
        get => _routeDefinition;
        set => _routeDefinition = value;
    }

    /// <inheritdoc />
    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    /// <inheritdoc />
    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        _renderHandle.Render(BuildRenderTree);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Builds the render tree for the component.
    /// </summary>
    /// <param name="builder">The render tree builder.</param>
    private void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (_routeDefinition == null)
        {
            // No route definition available
            builder.AddContent(0, "No route definition available.");
            return;
        }

        // Render the configured component
        builder.OpenComponent(0, _routeDefinition.ComponentType);

        // TODO: Pass resolved content and other parameters to the component

        builder.CloseComponent();
    }
}
