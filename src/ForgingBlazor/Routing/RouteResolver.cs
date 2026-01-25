namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Internal class for matching incoming request paths to <see cref="RouteDefinition"/> entries,
/// considering culture prefix and pagination patterns.
/// </summary>
internal sealed class RouteResolver
{
    private readonly RouteRegistry _registry;

    /// <summary>
    /// Initializes a new instance of the <see cref="RouteResolver"/> class.
    /// </summary>
    /// <param name="registry">The route registry containing all registered routes.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is null.</exception>
    public RouteResolver(RouteRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        _registry = registry;
    }

    /// <summary>
    /// Attempts to resolve a route definition from an incoming request path.
    /// </summary>
    /// <param name="requestPath">The incoming request path (e.g., "/posts/my-article").</param>
    /// <param name="definition">When this method returns, contains the matched route definition if found; otherwise, null.</param>
    /// <returns><see langword="true"/> if a route was matched; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="requestPath"/> is null.</exception>
    public bool TryResolve(string requestPath, [NotNullWhen(true)] out RouteDefinition? definition)
    {
        ArgumentNullException.ThrowIfNull(requestPath);

        // Normalize the request path
        var normalizedPath = NormalizePath(requestPath);

        // Try exact match first
        if (_registry.TryGetRoute(normalizedPath, out definition))
        {
            return true;
        }

        // Try to match against all registered routes (for partial matches, nested routes, etc.)
        var allRoutes = _registry.GetAll();
        foreach (var route in allRoutes)
        {
            if (IsMatch(normalizedPath, route.PathPattern))
            {
                definition = route;
                return true;
            }
        }

        definition = null;
        return false;
    }

    /// <summary>
    /// Normalizes a path by ensuring it starts with a forward slash and removing trailing slashes.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path.</returns>
    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "/";
        }

        // Ensure leading slash
        if (!path.StartsWith('/'))
        {
            path = "/" + path;
        }

        // Remove trailing slashes (except for root)
        if (path.Length > 1 && path.EndsWith('/'))
        {
            path = path.TrimEnd('/');
        }

        return path;
    }

    /// <summary>
    /// Checks if a request path matches a route pattern.
    /// </summary>
    /// <param name="requestPath">The normalized request path.</param>
    /// <param name="pattern">The route pattern to match against.</param>
    /// <returns><see langword="true"/> if the path matches the pattern; otherwise, <see langword="false"/>.</returns>
    private static bool IsMatch(string requestPath, string pattern) =>
        // For now, simple string comparison
        // TODO: Implement proper pattern matching with culture prefix and pagination patterns
        requestPath.Equals(pattern, StringComparison.OrdinalIgnoreCase)
        || requestPath.StartsWith(pattern + "/", StringComparison.OrdinalIgnoreCase);
}
