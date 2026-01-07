namespace NetEvolve.ForgingBlazor.Routing;

using Microsoft.AspNetCore.Components;

internal interface IRouteMiddleware
{
    void Execute(Action next, RouteData pageContext);
}
