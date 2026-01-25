namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

/// <summary>
/// Internal singleton registry storing all registered routes from FluentAPI configuration with lookup by path.
/// </summary>
internal sealed class RouteRegistry
{
    private readonly ConcurrentDictionary<string, RouteDefinition> _routes = new();

    /// <summary>
    /// Registers a route definition.
    /// </summary>
    /// <param name="pathPattern">The path pattern for the route.</param>
    /// <param name="definition">The route definition to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pathPattern"/> or <paramref name="definition"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="pathPattern"/> is empty or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a route with the same path pattern is already registered.</exception>
    public void Register(string pathPattern, RouteDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(pathPattern);
        ArgumentNullException.ThrowIfNull(definition);

        if (string.IsNullOrWhiteSpace(pathPattern))
        {
            throw new ArgumentException("Path pattern cannot be empty or whitespace.", nameof(pathPattern));
        }

        if (!_routes.TryAdd(pathPattern, definition))
        {
            throw new InvalidOperationException($"Route with path pattern '{pathPattern}' is already registered.");
        }
    }

    /// <summary>
    /// Attempts to get a route definition by path pattern.
    /// </summary>
    /// <param name="pathPattern">The path pattern to look up.</param>
    /// <param name="definition">When this method returns, contains the route definition if found; otherwise, null.</param>
    /// <returns><see langword="true"/> if a route definition was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetRoute(string pathPattern, [NotNullWhen(true)] out RouteDefinition? definition)
    {
        ArgumentNullException.ThrowIfNull(pathPattern);

        return _routes.TryGetValue(pathPattern, out definition);
    }

    /// <summary>
    /// Gets all registered route definitions.
    /// </summary>
    /// <returns>A read-only collection of all registered route definitions.</returns>
    public IReadOnlyCollection<RouteDefinition> GetAll() => _routes.Values.ToList();

    /// <summary>
    /// Clears all registered routes.
    /// </summary>
    internal void Clear() => _routes.Clear();
}
