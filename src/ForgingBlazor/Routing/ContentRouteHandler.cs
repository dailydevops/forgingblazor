namespace NetEvolve.ForgingBlazor.Routing;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

/// <summary>
/// Internal class implementing Blazor <see cref="IComponent"/> as dynamic router resolving content and rendering configured component.
/// </summary>
internal sealed class ContentRouteHandler : IComponent
{
    private RenderHandle _renderHandle;

    /// <summary>
    /// Gets or sets the route definition to render.
    /// </summary>
    [Parameter]
    public RouteDefinition? RouteDefinition { get; set; }

    /// <summary>
    /// Gets or sets the request path that resolved to this route.
    /// </summary>
    [Parameter]
    public string? RequestPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the navigation was intercepted (SPA navigation) or a full page load.
    /// </summary>
    [Parameter]
    public bool NavigationIntercepted { get; set; }

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
        if (RouteDefinition == null)
        {
            // No route definition available
            builder.AddContent(0, "No route definition available.");
            return;
        }

        // Render the configured component
        builder.OpenComponent(0, RouteDefinition.ComponentType);

        // TODO: Pass resolved content and other parameters to the component
        // - RequestPath: Current request path that resolved to this route
        // - NavigationIntercepted: Indicates if this was a SPA navigation (true) or full page load (false)
        //   Can be used for cache/state management decisions

        builder.CloseComponent();
    }
}
