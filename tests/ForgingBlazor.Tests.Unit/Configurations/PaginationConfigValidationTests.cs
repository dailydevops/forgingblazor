namespace NetEvolve.ForgingBlazor.Tests.Unit.Configurations;

using Microsoft.Extensions.Configuration;
using NetEvolve.ForgingBlazor.Configurations;

public class PaginationConfigValidationTests
{
    [Test]
    public async Task ConstructorThrowsArgumentNullExceptionWhenConfigurationIsNull() =>
        _ = await Assert.That(() => new PaginationConfigValidation(null!)).Throws<ArgumentNullException>();

    [Test]
    public async Task ConfigureThrowsArgumentNullExceptionWhenOptionsIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new PaginationConfigValidation(configuration);

        // Act & Assert
        _ = await Assert.That(() => validation.Configure(null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ConfigureBindsPaginationSectionToOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { "pagination:Mode", "1" },
                    { "pagination:PageSize", "25" },
                    { "pagination:Path", "page" },
                }
            )
            .Build();
        var validation = new PaginationConfigValidation(configuration);
        var options = new PaginationConfig();

        // Act
        validation.Configure(options);

        // Assert
        _ = await Assert.That(options.Mode).EqualTo(PaginationMode.Prefixed);
        _ = await Assert.That(options.PageSize).EqualTo(25);
        _ = await Assert.That(options.Path).EqualTo("page");
    }

    [Test]
    public async Task ValidateThrowsArgumentNullExceptionWhenOptionsIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new PaginationConfigValidation(configuration);

        // Act & Assert
        _ = await Assert.That(() => validation.Validate(null, null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ValidateSucceedsWithValidOptions()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new PaginationConfigValidation(configuration);
        var options = new PaginationConfig { PageSize = 20, Path = "pages" };

        // Act
        var result = validation.Validate(null, options);

        // Assert
        _ = await Assert.That(result.Succeeded).EqualTo(true);
    }

    [Test]
    public async Task PostConfigureThrowsArgumentNullExceptionWhenOptionsIsNull()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new PaginationConfigValidation(configuration);

        // Act & Assert
        _ = await Assert.That(() => validation.PostConfigure(null, null!)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task PostConfigureSetsPathToNullWhenEmpty()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new PaginationConfigValidation(configuration);
        var options = new PaginationConfig { Path = string.Empty };

        // Act
        validation.PostConfigure(null, options);

        // Assert
        _ = await Assert.That(options.Path).IsNull();
    }

    [Test]
    public async Task PostConfigureSetsPathToNullWhenWhitespace()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new PaginationConfigValidation(configuration);
        var options = new PaginationConfig { Path = "   " };

        // Act
        validation.PostConfigure(null, options);

        // Assert
        _ = await Assert.That(options.Path).IsNull();
    }

    [Test]
    public async Task PostConfigureKeepsPathWhenNotEmpty()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var validation = new PaginationConfigValidation(configuration);
        var options = new PaginationConfig { Path = "pages" };

        // Act
        validation.PostConfigure(null, options);

        // Assert
        _ = await Assert.That(options.Path).EqualTo("pages");
    }
}
