namespace NetEvolve.ForgingBlazor.Components;

using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

/// <summary>
/// Routes incoming requests to Blazor components or dynamic content, with content routing taking precedence
/// over standard component routing.
/// </summary>
public sealed partial class ForgingRouter : IComponent, IHandleAfterRender, IDisposable
{
    private RenderHandle _renderHandle;
    private bool _navigationInterceptionEnabled;
    private string? _location;
    private RenderFragment? _renderFragment;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private INavigationInterception NavigationInterception { get; set; } = default!;

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
        _location = NavigationManager.Uri;
        var locationPath = NavigationManager.ToBaseRelativePath(_location);

        if (Navigating is not null)
        {
            _renderFragment = Navigating;
            _renderHandle.Render(_renderFragment);
        }

        if (OnNavigateAsync.HasDelegate)
        {
            // TODO: Implement proper NavigationContext handling
            // For now, we skip the navigate event to avoid compilation errors
            // This will be implemented when full routing integration is complete
        }

        // TODO: Implement content route resolution here
        // For now, always render NotFound
        _renderFragment = NotFound;
        _renderHandle.Render(_renderFragment!);
    }
}
