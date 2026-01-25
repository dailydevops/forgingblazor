namespace NetEvolve.ForgingBlazor.Routing.Culture;

using System.Globalization;

/// <summary>
/// Validates culture configurations at application startup.
/// </summary>
internal sealed class CultureValidation
{
    private readonly IReadOnlySet<CultureInfo> _supportedCultures;
    private readonly CultureInfo _defaultCulture;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureValidation"/> class.
    /// </summary>
    /// <param name="supportedCultures">The set of supported cultures.</param>
    /// <param name="defaultCulture">The default culture.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="supportedCultures"/> is empty.</exception>
    public CultureValidation(IReadOnlySet<CultureInfo> supportedCultures, CultureInfo defaultCulture)
    {
        ArgumentNullException.ThrowIfNull(supportedCultures);
        ArgumentNullException.ThrowIfNull(defaultCulture);

        if (supportedCultures.Count == 0)
        {
            throw new ArgumentException(
                "At least one supported culture must be configured.",
                nameof(supportedCultures)
            );
        }

        _supportedCultures = supportedCultures;
        _defaultCulture = defaultCulture;
    }

    /// <summary>
    /// Validates whether a culture is supported.
    /// </summary>
    /// <param name="culture">The culture to validate.</param>
    /// <returns><c>true</c> if the culture is supported; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="culture"/> is <c>null</c>.</exception>
    public bool IsCultureSupported(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        return _supportedCultures.Contains(culture);
    }

    /// <summary>
    /// Validates that the default culture is in the supported cultures set.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the default culture is not supported.</exception>
    public void ValidateDefaultCulture()
    {
        if (!_supportedCultures.Contains(_defaultCulture))
        {
            throw new InvalidOperationException(
                $"The default culture '{_defaultCulture.Name}' must be included in the supported cultures."
            );
        }
    }

    /// <summary>
    /// Throws an exception if the specified culture is not supported.
    /// </summary>
    /// <param name="culture">The culture to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="culture"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the culture is not supported.</exception>
    public void ThrowIfUnsupported(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        if (!IsCultureSupported(culture))
        {
            throw new InvalidOperationException(
                $"The culture '{culture.Name}' is not supported. Supported cultures: {string.Join(", ", _supportedCultures.Select(c => c.Name))}"
            );
        }
    }

    /// <summary>
    /// Gets the default culture.
    /// </summary>
    public CultureInfo DefaultCulture => _defaultCulture;

    /// <summary>
    /// Gets the set of supported cultures.
    /// </summary>
    public IReadOnlySet<CultureInfo> SupportedCultures => _supportedCultures;
}
