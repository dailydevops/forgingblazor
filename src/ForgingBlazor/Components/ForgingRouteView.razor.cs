namespace NetEvolve.ForgingBlazor.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Displays the page component associated with the current route's <see cref="RouteData"/>,
/// with support for dynamic content resolution and <see cref="ResolvedContent{TDescriptor}"/> parameter injection.
/// </summary>
public sealed partial class ForgingRouteView : ComponentBase
{
    private Type? _componentType;
    private Dictionary<string, object?>? _parameters;
    private object? _resolvedContent;

    /// <summary>
    /// Gets or sets the route data.
    /// </summary>
    [Parameter]
    public RouteData? RouteData { get; set; }

    /// <summary>
    /// Gets or sets the type of a layout to use if the page does not declare any layout.
    /// If null, no default layout is used.
    /// </summary>
    [Parameter]
    public Type? DefaultLayout { get; set; }

    /// <summary>
    /// Gets or sets the content to display when no match is found for the requested route.
    /// </summary>
    [Parameter]
    public RenderFragment? NotFoundContent { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (RouteData is null)
        {
            _componentType = null;
            _parameters = null;
            _resolvedContent = null;
            return;
        }

        // Check if this is a content route with ResolvedContent
        if (RouteData.RouteValues.TryGetValue("__ResolvedContent", out var contentValue))
        {
            _resolvedContent = contentValue;
            _componentType = RouteData.PageType;

            // Build parameters including the resolved content
            var parameters = new Dictionary<string, object?> { ["ResolvedContent"] = _resolvedContent };

            // Add any additional route values as parameters
            foreach (var kvp in RouteData.RouteValues.Where(kv => kv.Key != "__ResolvedContent"))
            {
                parameters[kvp.Key] = kvp.Value;
            }

            _parameters = parameters;
        }
        else
        {
            // Standard component routing - let RouteView handle it
            _componentType = null;
            _parameters = null;
            _resolvedContent = null;
        }
    }
}
