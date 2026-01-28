namespace NetEvolve.ForgingBlazor.Components;

using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using NetEvolve.ForgingBlazor.Routing;

/// <summary>
/// Routes incoming requests to Blazor components or dynamic content, with content routing taking precedence
/// over standard component routing.
/// </summary>
public sealed class ForgingRouter : IComponent, IHandleAfterRender, IDisposable
{
    private RenderHandle _renderHandle;
    private bool _navigationInterceptionEnabled;
    private RenderFragment? _renderFragment;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private INavigationInterception NavigationInterception { get; set; } = default!;

    [Inject]
    private RouteRegistry RouteRegistry { get; set; } = default!;

    [Inject]
    private RouteResolver RouteResolver { get; set; } = default!;

    /// <summary>
    /// Gets or sets the assembly that should be searched for components matching the URI.
    /// </summary>
    [Parameter]
    public Assembly? AppAssembly { get; set; }

    /// <summary>
    /// Gets or sets a collection of additional assemblies that should be searched for components
    /// that can match URIs.
    /// </summary>
    [Parameter]
    public IEnumerable<Assembly>? AdditionalAssemblies { get; set; }

    /// <summary>
    /// Gets or sets the content to display when a match is found for the requested route.
    /// </summary>
    [Parameter]
    public RenderFragment<RouteData>? Found { get; set; }

    /// <summary>
    /// Gets or sets the content to display when no match is found for the requested route.
    /// </summary>
    [Parameter]
    public RenderFragment? NotFound { get; set; }

    /// <summary>
    /// Gets or sets the content to display when navigation is in progress.
    /// </summary>
    [Parameter]
    public RenderFragment? Navigating { get; set; }

    /// <summary>
    /// Gets or sets a handler that should be called before navigating to a new page.
    /// </summary>
    [Parameter]
    public EventCallback<NavigationContext> OnNavigateAsync { get; set; }

    /// <inheritdoc />
    public void Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

    private void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<ForgingRouter>>(0);
        builder.AddComponentParameter(1, "Value", this);
        builder.AddComponentParameter(2, "ChildContent", _renderFragment);
        builder.CloseComponent();
    }

    /// <inheritdoc />
    public async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (AppAssembly is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ForgingRouter)} requires a value for the {nameof(AppAssembly)} parameter."
            );
        }

        if (Found is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ForgingRouter)} requires a value for the {nameof(Found)} parameter."
            );
        }

        if (NotFound is null)
        {
            throw new InvalidOperationException(
                $"{nameof(ForgingRouter)} requires a value for the {nameof(NotFound)} parameter."
            );
        }

        // Subscribe to location changes
        NavigationManager.LocationChanged -= OnLocationChanged;
        NavigationManager.LocationChanged += OnLocationChanged;

        await RefreshAsync(isNavigationIntercepted: false).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task OnAfterRenderAsync()
    {
        if (!_navigationInterceptionEnabled)
        {
            _navigationInterceptionEnabled = true;
            return NavigationInterception.EnableNavigationInterceptionAsync();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose() => NavigationManager.LocationChanged -= OnLocationChanged;

    private void OnLocationChanged(object? sender, LocationChangedEventArgs args) =>
        _ = RefreshAsync(isNavigationIntercepted: args.IsNavigationIntercepted);

    private async Task RefreshAsync(bool isNavigationIntercepted)
    {
        var location = NavigationManager.Uri;
        var locationPath = NavigationManager.ToBaseRelativePath(location);

        if (Navigating is not null)
        {
            _renderFragment = Navigating;
            _renderHandle.Render(BuildRenderTree);
        }

        // Note: OnNavigateAsync callback is handled by the Router component
        // We don't invoke it directly here to avoid NavigationContext construction
        // isNavigationIntercepted indicates whether this was a SPA navigation (true) or a full page load (false)

        // Try to resolve a content route first
        if (RouteResolver.TryResolve(locationPath, out var routeDefinition))
        {
            // Render content route via ContentRouteHandler
            // isNavigationIntercepted is available for cache/state management decisions
            _renderFragment = builder =>
            {
                builder.OpenComponent<ContentRouteHandler>(0);
                builder.AddComponentParameter(1, "RouteDefinition", routeDefinition);
                builder.AddComponentParameter(2, "RequestPath", locationPath);
                builder.AddComponentParameter(3, "NavigationIntercepted", isNavigationIntercepted);
                builder.CloseComponent();
            };
        }
        else
        {
            // Fall back to standard component routing using Router
            _renderFragment = builder =>
            {
                builder.OpenComponent<Router>(0);
                builder.AddComponentParameter(1, "AppAssembly", AppAssembly);
                builder.AddComponentParameter(2, "AdditionalAssemblies", AdditionalAssemblies);
                builder.AddComponentParameter(3, "Found", Found);
                builder.AddComponentParameter(4, "NotFound", NotFound);
                builder.AddComponentParameter(5, "Navigating", Navigating);
                builder.AddComponentParameter(6, "OnNavigateAsync", OnNavigateAsync);
                builder.CloseComponent();
            };
        }

        _renderHandle.Render(BuildRenderTree);
    }
}
