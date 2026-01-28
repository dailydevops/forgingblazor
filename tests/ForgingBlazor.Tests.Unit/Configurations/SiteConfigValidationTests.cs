namespace NetEvolve.ForgingBlazor.Tests.Unit.Configurations;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using NetEvolve.ForgingBlazor.Configurations;

[SuppressMessage("Design", "CA1054:URI-like properties should not be strings", Justification = "Test parameter")]
public class SiteConfigValidationTests
{
    [Test]
    public async Task ConstructorThrowsArgumentNullExceptionWhenConfigurationIsNull() =>
        _ = await Assert.That(() => new SiteConfigValidation(null!)).Throws<ArgumentNullException>();

    [Test]
    public async Task ConfigureThrowsArgumentNullExceptionWhenOptionsIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new SiteConfigValidation(configuration);

        // Act & Assert
        _ = await Assert.That(() => validation.Configure(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ConfigureBindsSiteSectionToOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "site:BaseUrl", "https://example.com" },
                    { "site:LanguageCode", "de-DE" },
                    { "site:Title", "Test Site" },
                }
            )
            .Build();
        var validation = new SiteConfigValidation(configuration);
        var options = new SiteConfig();

        // Act
        validation.Configure(options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(options.BaseUrl).EqualTo("https://example.com");
            _ = await Assert.That(options.LanguageCode).EqualTo("de-DE");
            _ = await Assert.That(options.Title).EqualTo("Test Site");
        }
    }

    [Test]
    public async Task ValidateThrowsArgumentNullExceptionWhenOptionsIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new SiteConfigValidation(configuration);

        // Act & Assert
        _ = await Assert.That(() => validation.Validate(null, null!)).Throws<ArgumentNullException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("not-a-url")]
    [Arguments("relative/path")]
    public async Task ValidateFailsWhenBaseUrlIsInvalid(string? invalidBaseUrl)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new SiteConfigValidation(configuration);
        var options = new SiteConfig
        {
            BaseUrl = invalidBaseUrl!,
            LanguageCode = "en-US",
            Title = "Test",
        };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).EqualTo(true);
            _ = await Assert.That(result.FailureMessage).Contains("valid absolute URI");
        }
    }

    [Test]
    [Arguments("ftp://example.com")]
    [Arguments("file:///c:/temp")]
    [Arguments("mailto:test@example.com")]
    public async Task ValidateFailsWhenBaseUrlSchemeIsNotHttpOrHttps(string invalidSchemeUrl)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new SiteConfigValidation(configuration);
        var options = new SiteConfig
        {
            BaseUrl = invalidSchemeUrl,
            LanguageCode = "en-US",
            Title = "Test",
        };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).EqualTo(true);
            _ = await Assert.That(result.FailureMessage).Contains("HTTP or HTTPS scheme");
        }
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task ValidateFailsWhenLanguageCodeIsNullOrWhitespace(string? invalidLanguageCode)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new SiteConfigValidation(configuration);
        var options = new SiteConfig
        {
            BaseUrl = "https://example.com",
            LanguageCode = invalidLanguageCode!,
            Title = "Test",
        };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).EqualTo(true);
            _ = await Assert.That(result.FailureMessage).Contains("language code must be provided");
        }
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task ValidateFailsWhenTitleIsNullOrWhitespace(string? invalidTitle)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new SiteConfigValidation(configuration);
        var options = new SiteConfig
        {
            BaseUrl = "https://example.com",
            LanguageCode = "en-US",
            Title = invalidTitle!,
        };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Failed).EqualTo(true);
            _ = await Assert.That(result.FailureMessage).Contains("site title must be provided");
        }
    }

    [Test]
    [Arguments("http://example.com")]
    [Arguments("https://example.com")]
    [Arguments("http://localhost:5000")]
    [Arguments("https://example.com/path")]
    public async Task ValidateSucceedsWithValidConfiguration(string validBaseUrl)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new SiteConfigValidation(configuration);
        var options = new SiteConfig
        {
            BaseUrl = validBaseUrl,
            LanguageCode = "en-US",
            Title = "Valid Site",
        };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Succeeded).EqualTo(true);
    }
}
