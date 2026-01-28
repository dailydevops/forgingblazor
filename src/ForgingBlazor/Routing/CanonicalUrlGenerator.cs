namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Globalization;

/// <summary>
/// Internal class for generating canonical URLs based on <see cref="CultureCanonical"/> setting and route definition.
/// </summary>
internal sealed class CanonicalUrlGenerator
{
    /// <summary>
    /// Generates a canonical URL for a route.
    /// </summary>
    /// <param name="pathPattern">The path pattern for the route.</param>
    /// <param name="culture">The culture for the route.</param>
    /// <param name="canonicalFormat">The canonical format setting.</param>
    /// <param name="defaultCulture">The default culture.</param>
    /// <returns>The canonical URL.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pathPattern"/>, <paramref name="culture"/>, or <paramref name="defaultCulture"/> is null.</exception>
    public static string Generate(
        string pathPattern,
        CultureInfo culture,
        CultureCanonical canonicalFormat,
        CultureInfo defaultCulture
    )
    {
        ArgumentNullException.ThrowIfNull(pathPattern);
        ArgumentNullException.ThrowIfNull(culture);
        ArgumentNullException.ThrowIfNull(defaultCulture);

        // Normalize path pattern
        var normalizedPath = NormalizePath(pathPattern);

        // Determine if culture prefix should be included
        var includeCulturePrefix = ShouldIncludeCulturePrefix(culture, canonicalFormat, defaultCulture);

        if (includeCulturePrefix)
        {
            var result = $"/{culture.Name}{normalizedPath}";
            // Remove trailing slash for non-root paths
            if (result.Length > 1 && result.EndsWith('/'))
            {
                result = result.TrimEnd('/');
            }
            return result;
        }

        return normalizedPath;
    }

    /// <summary>
    /// Determines if a culture prefix should be included in the canonical URL.
    /// </summary>
    /// <param name="culture">The current culture.</param>
    /// <param name="canonicalFormat">The canonical format setting.</param>
    /// <param name="defaultCulture">The default culture.</param>
    /// <returns><see langword="true"/> if a culture prefix should be included; otherwise, <see langword="false"/>.</returns>
    private static bool ShouldIncludeCulturePrefix(
        CultureInfo culture,
        CultureCanonical canonicalFormat,
        CultureInfo defaultCulture
    ) =>
        canonicalFormat switch
        {
            CultureCanonical.WithPrefix => true,
            CultureCanonical.WithoutPrefix => !culture.Equals(defaultCulture)
                && !culture.TwoLetterISOLanguageName.Equals(
                    defaultCulture.TwoLetterISOLanguageName,
                    StringComparison.OrdinalIgnoreCase
                ),
            _ => false,
        };

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
            path = $"/{path}";
        }

        // Remove trailing slashes (except for root)
        if (path.Length > 1 && path.EndsWith('/'))
        {
            path = path.TrimEnd('/');
        }

        return path;
    }
}
