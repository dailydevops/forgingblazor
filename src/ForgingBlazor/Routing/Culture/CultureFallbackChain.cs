namespace NetEvolve.ForgingBlazor.Routing.Culture;

using System.Globalization;

/// <summary>
/// Implements culture fallback hierarchy for content lookup.
/// </summary>
/// <remarks>
/// Fallback order (example for de-DE):
/// 1. Specific culture: de-DE
/// 2. Neutral culture: de
/// 3. Default culture: en-US (configurable)
/// 4. Default neutral culture: en
/// 5. No culture suffix (invariant)
/// </remarks>
internal sealed class CultureFallbackChain
{
    private readonly CultureInfo _defaultCulture;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureFallbackChain"/> class.
    /// </summary>
    /// <param name="defaultCulture">The default culture to use in the fallback chain. Defaults to en-US if <c>null</c>.</param>
    public CultureFallbackChain(CultureInfo? defaultCulture = null)
    {
        _defaultCulture = defaultCulture ?? CultureInfo.GetCultureInfo("en-US");
    }

    /// <summary>
    /// Gets the fallback chain for the specified culture.
    /// </summary>
    /// <param name="culture">The culture to get the fallback chain for.</param>
    /// <returns>An ordered list of cultures to try, from most specific to least specific.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="culture"/> is <c>null</c>.</exception>
    public IReadOnlyList<CultureInfo?> GetFallbackChain(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var chain = new List<CultureInfo?>();

        // 1. Add the specific culture (e.g., de-DE) or neutral culture (e.g., de)
        chain.Add(culture);

        // 2. Add the parent neutral culture if the current culture is specific (e.g., de from de-DE)
        if (!culture.IsNeutralCulture && !string.IsNullOrEmpty(culture.Parent?.Name))
        {
            chain.Add(culture.Parent);
        }

        // 3. Add the default culture (e.g., en-US) if it's different from the requested culture
        if (!_defaultCulture.Equals(culture) && !_defaultCulture.Equals(culture.Parent))
        {
            if (!_defaultCulture.IsNeutralCulture)
            {
                chain.Add(_defaultCulture);
            }

            // 4. Add the default neutral culture (e.g., en)
            if (!string.IsNullOrEmpty(_defaultCulture.Parent?.Name))
            {
                chain.Add(_defaultCulture.Parent);
            }
        }

        // 5. Add invariant culture (no suffix)
        chain.Add(null);

        return chain.AsReadOnly();
    }

    /// <summary>
    /// Generates culture suffixes for file lookup.
    /// </summary>
    /// <param name="culture">The culture to generate suffixes for.</param>
    /// <returns>An ordered list of culture suffixes, including an empty string for no suffix.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="culture"/> is <c>null</c>.</exception>
    /// <remarks>
    /// Example for de-DE:
    /// - .de-DE
    /// - .de
    /// - .en-US
    /// - .en
    /// - (empty string for no suffix)
    /// </remarks>
    public IReadOnlyList<string> GetCultureSuffixes(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var fallbackChain = GetFallbackChain(culture);
        var suffixes = new List<string>(fallbackChain.Count);

        foreach (var fallbackCulture in fallbackChain)
        {
            if (fallbackCulture is null)
            {
                suffixes.Add(string.Empty);
            }
            else
            {
                suffixes.Add($".{fallbackCulture.Name}");
            }
        }

        return suffixes.AsReadOnly();
    }
}
