namespace NetEvolve.ForgingBlazor.Routing.Constraints;

using System;
using System.Globalization;
using global::NetEvolve.ForgingBlazor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

/// <summary>
/// Enforces the slug route constraint defined by the dynamic content routing system.
/// </summary>
public sealed class SlugRouteConstraint : IRouteConstraint
{
    /// <inheritdoc />
    public bool Match(
        HttpContext? httpContext,
        IRouter? route,
        string routeKey,
        RouteValueDictionary values,
        RouteDirection routeDirection
    )
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentException.ThrowIfNullOrWhiteSpace(routeKey);

        if (!values.TryGetValue(routeKey, out var value) || value is null)
        {
            return false;
        }

        if (value is string slug)
        {
            return Check.IsValidSlug(slug);
        }

        if (value is ReadOnlyMemory<char> memory)
        {
            return Check.IsValidSlug(memory.ToString());
        }

        if (value is IFormCollection formCollection && formCollection.TryGetValue(routeKey, out var formValue))
        {
            return Check.IsValidSlug(formValue.ToString());
        }

        var converted = Convert.ToString(value, CultureInfo.InvariantCulture);
        return converted is not null && Check.IsValidSlug(converted);
    }
}
