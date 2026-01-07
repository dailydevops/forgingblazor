namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

internal sealed class RouteProvider : IRouteProvider
{
    private readonly IReadOnlyCollection<Route> _routes;

    public RouteProvider(SegmentBuilder rootBuilder)
    {
        ArgumentNullException.ThrowIfNull(rootBuilder);

        _routes = rootBuilder.Build();
    }

    public bool TryGetRoute(Type pageType, out string? route) => throw new NotImplementedException();

    public bool TryGetRouteData(Type pageType, out Route? routeData) => throw new NotImplementedException();

    public bool TryMatch(string localPath, out RouteData routeData) => throw new NotImplementedException();
}
