namespace NetEvolve.ForgingBlazor.Routing;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

public interface IRouteProvider
{
    bool TryGetRoute<TPage>([NotNullWhen(true)] out string? route)
        where TPage : class, IComponent => TryGetRoute(typeof(TPage), out route);

    bool TryGetRoute(Type pageType, [NotNullWhen(true)] out string? route);

    bool TryGetRouteData<TPage>([NotNullWhen(true)] out Route? routeData)
        where TPage : class, IComponent => TryGetRouteData(typeof(TPage), out routeData);

    bool TryGetRouteData(Type pageType, [NotNullWhen(true)] out Route? routeData);

    bool TryMatch(string localPath, out RouteData routeData);
}
