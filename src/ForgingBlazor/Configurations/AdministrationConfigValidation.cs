namespace NetEvolve.ForgingBlazor.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

/// <summary>
/// Provides configuration and validation for <see cref="AdministrationConfig"/> settings.
/// </summary>
internal sealed class AdministrationConfigValidation
    : IConfigureOptions<AdministrationConfig>,
        IValidateOptions<AdministrationConfig>
{
    /// <summary>
    /// The application configuration instance used to bind settings.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdministrationConfigValidation"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
    public AdministrationConfigValidation(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _configuration = configuration;
    }

    /// <summary>
    /// Configures the <see cref="AdministrationConfig"/> options by binding values from the "administration" configuration section.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public void Configure(AdministrationConfig options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _configuration.Bind("administration", options);
    }

    /// <summary>
    /// Validates the <see cref="AdministrationConfig"/> options to ensure required settings are provided.
    /// </summary>
    /// <param name="name">The name of the options instance being validated.</param>
    /// <param name="options">The options instance to validate.</param>
    /// <returns>A <see cref="ValidateOptionsResult"/> indicating whether validation succeeded or failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public ValidateOptionsResult Validate(string? name, AdministrationConfig options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.PathSegment))
        {
            return Fail("Configuration(Administration.PathSegment): The path segment must be provided.");
        }

        if (!Check.IsValidAdminSegment(options.PathSegment))
        {
            return Fail("Configuration(Administration.PathSegment): The path segment contains invalid characters.");
        }

        return Success;
    }
}
