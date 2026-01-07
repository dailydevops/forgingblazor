namespace NetEvolve.ForgingBlazor.Routing;

public interface IRouteMatching
{
    bool IsMatch(string relativeRoute, Dictionary<string, object> routeValues);
}
