namespace NetEvolve.ForgingBlazor.Routing.Validation;

using System;
using Microsoft.Extensions.Options;

/// <summary>
/// Validates routing configuration at startup implementing <see cref="IValidateOptions{TOptions}"/>.
/// Validates: at least one culture configured, default component/layout defined.
/// </summary>
internal sealed class RoutingConfigurationValidation : IValidateOptions<RoutingConfiguration>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, RoutingConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Validate at least one culture is configured
        if (options.SupportedCultures is null || options.SupportedCultures.Count == 0)
        {
            return ValidateOptionsResult.Fail("At least one culture must be configured.");
        }

        // Validate default culture is set
        if (options.DefaultCulture is null)
        {
            return ValidateOptionsResult.Fail("Default culture must be configured.");
        }

        // Validate default component is defined
        if (options.DefaultComponentType is null)
        {
            return ValidateOptionsResult.Fail("Default component type must be defined.");
        }

        // Validate default layout is defined
        if (options.DefaultLayoutType is null)
        {
            return ValidateOptionsResult.Fail("Default layout type must be defined.");
        }

        // Validate default culture is in supported cultures
        if (!options.SupportedCultures.Contains(options.DefaultCulture))
        {
            return ValidateOptionsResult.Fail("Default culture must be in the list of supported cultures.");
        }

        return ValidateOptionsResult.Success;
    }
}

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
