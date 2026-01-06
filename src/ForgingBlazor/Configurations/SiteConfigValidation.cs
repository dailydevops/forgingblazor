namespace NetEvolve.ForgingBlazor.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

internal sealed class SiteConfigValidation : IConfigureOptions<SiteConfig>, IValidateOptions<SiteConfig>
{
    private readonly IConfiguration _configuration;

    public SiteConfigValidation(IConfiguration configuration) => _configuration = configuration;

    public void Configure(SiteConfig options) => _configuration.Bind("site", options);

    public ValidateOptionsResult Validate(string? name, SiteConfig options)
    {
        if (!Uri.IsWellFormedUriString(options.BaseUrl, UriKind.Absolute))
        {
            return Fail("Configuration(BaseUrl): The base URL must be a valid absolute URI.");
        }

        if (string.IsNullOrWhiteSpace(options.LanguageCode))
        {
            return Fail("Configuration(LanguageCode): The language code must be provided.");
        }

        if (string.IsNullOrWhiteSpace(options.Title))
        {
            return Fail("Configuration(Title): The site title must be provided.");
        }

        return Success;
    }
}
