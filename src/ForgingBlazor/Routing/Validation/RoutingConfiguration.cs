namespace NetEvolve.ForgingBlazor.Routing.Validation;

using System;

/// <summary>
/// Represents routing configuration options.
/// </summary>
internal sealed class RoutingConfiguration
{
    /// <summary>
    /// Gets or sets the supported cultures.
    /// </summary>
    public IReadOnlyList<System.Globalization.CultureInfo>? SupportedCultures { get; set; }

    /// <summary>
    /// Gets or sets the default culture.
    /// </summary>
    public System.Globalization.CultureInfo? DefaultCulture { get; set; }

    /// <summary>
    /// Gets or sets the default component type.
    /// </summary>
    public Type? DefaultComponentType { get; set; }

    /// <summary>
    /// Gets or sets the default layout type.
    /// </summary>
    public Type? DefaultLayoutType { get; set; }
}
