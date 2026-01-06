namespace NetEvolve.ForgingBlazor.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

/// <summary>
/// Binds and validates configuration values for the site settings section.
/// </summary>
internal sealed class SiteConfigValidation : IConfigureOptions<SiteConfig>, IValidateOptions<SiteConfig>
{
    /// <summary>
    /// Configuration source that provides the site settings.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="SiteConfigValidation"/> class.
    /// </summary>
    /// <param name="configuration">The configuration source containing the site section.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
    public SiteConfigValidation(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _configuration = configuration;
    }

    /// <summary>
    /// Binds the "site" configuration section to the provided options instance.
    /// </summary>
    /// <param name="options">The options instance to populate from configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public void Configure(SiteConfig options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _configuration.Bind("site", options);
    }

    /// <summary>
    /// Validates the populated site configuration values for correctness and completeness.
    /// </summary>
    /// <param name="name">The named options instance being validated.</param>
    /// <param name="options">The configuration values to validate.</param>
    /// <returns>A <see cref="ValidateOptionsResult"/> indicating the validation outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public ValidateOptionsResult Validate(string? name, SiteConfig options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUrl))
        {
            return Fail("Configuration(Site.BaseUrl): The base URL must be a valid absolute URI.");
        }

        if (baseUrl.Scheme != Uri.UriSchemeHttp && baseUrl.Scheme != Uri.UriSchemeHttps)
        {
            return Fail("Configuration(Site.BaseUrl): The base URL must use either HTTP or HTTPS scheme.");
        }

        if (string.IsNullOrWhiteSpace(options.LanguageCode))
        {
            return Fail("Configuration(Site.LanguageCode): The language code must be provided.");
        }

        if (string.IsNullOrWhiteSpace(options.Title))
        {
            return Fail("Configuration(Site.Title): The site title must be provided.");
        }

        return Success;
    }
}
