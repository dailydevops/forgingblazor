namespace NetEvolve.ForgingBlazor.Routing.Constraints;

using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NetEvolve.ForgingBlazor.Pagination;

/// <summary>
/// Route constraint for validating pagination URL segments.
/// </summary>
/// <remarks>
/// This constraint validates pagination segments in two formats:
/// - Numeric: "2", "3", etc.
/// - Prefixed: "page-2", "page-3", etc.
/// </remarks>
internal sealed class PaginationRouteConstraint : IRouteConstraint
{
    private readonly PaginationSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationRouteConstraint"/> class.
    /// </summary>
    /// <param name="settings">The pagination settings defining the URL format.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="settings"/> is null.</exception>
    public PaginationRouteConstraint(PaginationSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _settings = settings;
    }

    /// <summary>
    /// Determines whether the URL parameter contains a valid pagination segment.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="route">The route.</param>
    /// <param name="routeKey">The route key.</param>
    /// <param name="values">The route values.</param>
    /// <param name="routeDirection">The route direction.</param>
    /// <returns><c>true</c> if the parameter is a valid pagination segment; otherwise, <c>false</c>.</returns>
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        if (!values.TryGetValue(routeKey, out var value) || value is null)
        {
            return false;
        }

        var segment = Convert.ToString(value, CultureInfo.InvariantCulture);

        if (string.IsNullOrWhiteSpace(segment))
        {
            return false;
        }

        return PaginationUrlParser.TryParse(segment, _settings, out _);
    }
}
