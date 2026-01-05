namespace NetEvolve.ForgingBlazor.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

/// <summary>
/// Provides configuration and validation for <see cref="AdministrationConfiguration"/> settings.
/// </summary>
internal class AdministrationConfigurationValidation
    : IConfigureOptions<AdministrationConfiguration>,
        IValidateOptions<AdministrationConfiguration>
{
    /// <summary>
    /// The application configuration instance used to bind settings.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdministrationConfigurationValidation"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration instance.</param>
    public AdministrationConfigurationValidation(IConfiguration configuration) => _configuration = configuration;

    /// <summary>
    /// Configures the <see cref="AdministrationConfiguration"/> options by binding values from the "administration" configuration section.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(AdministrationConfiguration options) => _configuration.Bind("administration", options);

    /// <summary>
    /// Validates the <see cref="AdministrationConfiguration"/> options to ensure required settings are provided.
    /// </summary>
    /// <param name="name">The name of the options instance being validated.</param>
    /// <param name="options">The options instance to validate.</param>
    /// <returns>A <see cref="ValidateOptionsResult"/> indicating whether validation succeeded or failed.</returns>
    public ValidateOptionsResult Validate(string? name, AdministrationConfiguration options)
    {
        if (string.IsNullOrWhiteSpace(options.PathSegment))
        {
            return Fail("PathSegment must be provided.");
        }

        return Success;
    }
}
