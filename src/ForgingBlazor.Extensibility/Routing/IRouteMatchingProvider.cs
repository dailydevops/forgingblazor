namespace NetEvolve.ForgingBlazor.Routing;

public interface IRouteMatchingProvider
{
    IRouteMatching Compile(string route);
}
