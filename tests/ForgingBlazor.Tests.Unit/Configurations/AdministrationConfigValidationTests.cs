namespace NetEvolve.ForgingBlazor.Tests.Unit.Configurations;

using Microsoft.Extensions.Configuration;
using NetEvolve.ForgingBlazor.Configurations;

public class AdministrationConfigValidationTests
{
    [Test]
    public async Task ConstructorThrowsArgumentNullExceptionWhenConfigurationIsNull() =>
        _ = await Assert.That(() => new AdministrationConfigValidation(null!)).Throws<ArgumentNullException>();

    [Test]
    public async Task ConfigureThrowsArgumentNullExceptionWhenOptionsIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new AdministrationConfigValidation(configuration);

        // Act & Assert
        _ = await Assert.That(() => validation.Configure(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ConfigureBindsAdministrationSectionToOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "administration:IsEnabled", "false" },
                    { "administration:PathSegment", "custom-admin" },
                }
            )
            .Build();
        var validation = new AdministrationConfigValidation(configuration);
        var options = new AdministrationConfig();

        // Act
        validation.Configure(options);

        // Assert
        _ = await Assert.That(options.IsEnabled).EqualTo(false);
        _ = await Assert.That(options.PathSegment).EqualTo("custom-admin");
    }

    [Test]
    public async Task ValidateThrowsArgumentNullExceptionWhenOptionsIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new AdministrationConfigValidation(configuration);

        // Act & Assert
        _ = await Assert.That(() => validation.Validate(null, null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ValidateFailsWhenPathSegmentIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new AdministrationConfigValidation(configuration);
        var options = new AdministrationConfig { PathSegment = null! };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Failed).EqualTo(true);
        _ = await Assert.That(result.FailureMessage).Contains("path segment must be provided");
    }

    [Test]
    public async Task ValidateFailsWhenPathSegmentIsEmpty()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new AdministrationConfigValidation(configuration);
        var options = new AdministrationConfig { PathSegment = string.Empty };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Failed).EqualTo(true);
        _ = await Assert.That(result.FailureMessage).Contains("path segment must be provided");
    }

    [Test]
    public async Task ValidateFailsWhenPathSegmentIsWhitespace()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new AdministrationConfigValidation(configuration);
        var options = new AdministrationConfig { PathSegment = "   " };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Failed).EqualTo(true);
        _ = await Assert.That(result.FailureMessage).Contains("path segment must be provided");
    }

    [Test]
    [Arguments("admin with space")]
    [Arguments("!admin")]
    public async Task ValidateFailsWhenPathSegmentIsInvalid(string invalidPathSegment)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new AdministrationConfigValidation(configuration);
        var options = new AdministrationConfig { PathSegment = invalidPathSegment };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Failed).EqualTo(true);
        _ = await Assert
            .That(result.FailureMessage)
            .EndsWith("The pagination path must be a valid URL path segment.", StringComparison.Ordinal);
    }

    [Test]
    [Arguments("admin")]
    [Arguments("admin-panel")]
    [Arguments("admin_panel")]
    [Arguments("admin123")]
    [Arguments("a12")]
    [Arguments("a1_b2-c3")]
    public async Task ValidateSucceedsWhenPathSegmentIsValid(string validPathSegment)
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new AdministrationConfigValidation(configuration);
        var options = new AdministrationConfig { PathSegment = validPathSegment };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Succeeded).EqualTo(true);
    }
}
