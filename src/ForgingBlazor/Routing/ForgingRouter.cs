namespace NetEvolve.ForgingBlazor.Routing;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;

public sealed class ForgingRouter : IComponent, IHandleAfterRender, IDisposable
{
    private bool _disposedValue;
    private bool _isFirstRender = true;
    private string? _location;
    private RenderHandle _renderHandle;

    [Parameter]
    public RenderFragment<RouteData> Found { get; set; }

    [Parameter]
    public RenderFragment NotFound { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; }

    [Inject]
    private IRouteProvider RouteProvider { get; set; }

    [Inject]
    private INavigationInterception NavigationInterception { get; set; }

    [Inject]
    private IServiceProvider ServiceProvider { get; set; }

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
        _location = NavigationManager.Uri;

        NavigationManager.LocationChanged += OnLocationChanged;
    }

    public Task OnAfterRenderAsync()
    {
        if (!_isFirstRender)
        {
            return Task.CompletedTask;
        }

        _isFirstRender = false;
        return NavigationInterception.EnableNavigationInterceptionAsync();
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (Found is null)
        {
            throw new InvalidOperationException($"The '{nameof(Found)}' parameter is required.");
        }

        if (NotFound is null)
        {
            throw new InvalidOperationException($"The '{nameof(NotFound)}' parameter is required.");
        }

        Refresh();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                NavigationManager.LocationChanged -= OnLocationChanged;
            }

            _disposedValue = true;
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _location = e.Location;
        Refresh();
    }

    private void Refresh()
    {
        var relativePath = NavigationManager.ToBaseRelativePath(_location!);
        var relativeUri = new Uri(relativePath, UriKind.Relative);

        if (!RouteProvider.TryMatch(relativeUri.LocalPath, out var routeData))
        {
            _renderHandle.Render(NotFound);
            return;
        }

        var middlewares = ServiceProvider.GetServices<IRouteMiddleware>();
        var renderPage = true;

        foreach (var routerMiddleware in middlewares)
        {
            var executeNext = false;

            routerMiddleware.Execute(() => executeNext = true, routeData);

            if (!executeNext)
            {
                renderPage = false;
                break;
            }
        }

        if (renderPage)
        {
            _renderHandle.Render(Found(routeData));
        }
    }
}
