#pragma warning disable CA1716 // Method name 'Default' is intentionally part of the public contract
namespace NetEvolve.ForgingBlazor;

using System.Globalization;

/// <summary>
/// Configures default and supported cultures for routing and content resolution.
/// </summary>
public interface ICultureConfiguration
{
    /// <summary>
    /// Sets the default culture.
    /// </summary>
    /// <param name="culture">The default <see cref="CultureInfo"/>.</param>
    /// <returns>The <see cref="ICultureConfiguration"/> for chaining.</returns>
    ICultureConfiguration Default(CultureInfo culture);

    /// <summary>
    /// Sets the default culture by name.
    /// </summary>
    /// <param name="culture">The culture name (e.g., "en-US").</param>
    /// <returns>The <see cref="ICultureConfiguration"/> for chaining.</returns>
    ICultureConfiguration Default(string culture);

    /// <summary>
    /// Sets the default culture by LCID.
    /// </summary>
    /// <param name="lcid">The culture LCID.</param>
    /// <returns>The <see cref="ICultureConfiguration"/> for chaining.</returns>
    ICultureConfiguration Default(int lcid);

    /// <summary>
    /// Registers supported cultures.
    /// </summary>
    /// <param name="cultures">A set of cultures to support.</param>
    /// <returns>The <see cref="ICultureConfiguration"/> for chaining.</returns>
    ICultureConfiguration Supported(params CultureInfo[] cultures);

    /// <summary>
    /// Registers supported cultures by name.
    /// </summary>
    /// <param name="cultures">Culture names (e.g., "en", "fr", "de").</param>
    /// <returns>The <see cref="ICultureConfiguration"/> for chaining.</returns>
    ICultureConfiguration Supported(params string[] cultures);

    /// <summary>
    /// Registers supported cultures by LCID.
    /// </summary>
    /// <param name="lcids">Culture LCIDs.</param>
    /// <returns>The <see cref="ICultureConfiguration"/> for chaining.</returns>
    ICultureConfiguration Supported(params int[] lcids);
}
