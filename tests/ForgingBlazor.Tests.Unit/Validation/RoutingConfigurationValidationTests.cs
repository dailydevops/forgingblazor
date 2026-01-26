namespace NetEvolve.ForgingBlazor.Tests.Unit.Validation;

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using NetEvolve.ForgingBlazor.Routing.Validation;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="RoutingConfigurationValidation"/>.
/// Tests routing configuration validation at startup including culture settings,
/// component types, and layout types.
/// </summary>
public sealed class RoutingConfigurationValidationTests
{
    [Test]
    public async Task Validate_WithValidConfiguration_ReturnsSuccess()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo> { CultureInfo.GetCultureInfo("en-US") },
            DefaultCulture = CultureInfo.GetCultureInfo("en-US"),
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.Failed).IsFalse();
        }
    }

    [Test]
    public void Validate_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("options", () => validator.Validate(null, null!));
    }

    [Test]
    public async Task Validate_WithNullSupportedCultures_ReturnsFail()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = null,
            DefaultCulture = CultureInfo.GetCultureInfo("en-US"),
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("culture must be configured");
        }
    }

    [Test]
    public async Task Validate_WithEmptySupportedCultures_ReturnsFail()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo>(),
            DefaultCulture = CultureInfo.GetCultureInfo("en-US"),
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("At least one culture must be configured");
        }
    }

    [Test]
    public async Task Validate_WithNullDefaultCulture_ReturnsFail()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo> { CultureInfo.GetCultureInfo("en-US") },
            DefaultCulture = null,
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("Default culture must be configured");
        }
    }

    [Test]
    public async Task Validate_WithNullDefaultComponentType_ReturnsFail()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo> { CultureInfo.GetCultureInfo("en-US") },
            DefaultCulture = CultureInfo.GetCultureInfo("en-US"),
            DefaultComponentType = null,
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("Default component type must be defined");
        }
    }

    [Test]
    public async Task Validate_WithNullDefaultLayoutType_ReturnsFail()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo> { CultureInfo.GetCultureInfo("en-US") },
            DefaultCulture = CultureInfo.GetCultureInfo("en-US"),
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = null,
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("Default layout type must be defined");
        }
    }

    [Test]
    public async Task Validate_WithDefaultCultureNotInSupportedCultures_ReturnsFail()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo> { CultureInfo.GetCultureInfo("en-US") },
            DefaultCulture = CultureInfo.GetCultureInfo("de-DE"),
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert
                .That(result.FailureMessage)
                .Contains("Default culture must be in the list of supported cultures");
        }
    }

    [Test]
    public async Task Validate_WithMultipleSupportedCultures_ReturnsSuccess()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo>
            {
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.GetCultureInfo("de-DE"),
                CultureInfo.GetCultureInfo("fr-FR"),
            },
            DefaultCulture = CultureInfo.GetCultureInfo("en-US"),
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.Failed).IsFalse();
        }
    }

    [Test]
    public async Task Validate_WithDefaultCultureAsSecondInList_ReturnsSuccess()
    {
        // Arrange
        var validator = new RoutingConfigurationValidation();
        var options = new RoutingConfiguration
        {
            SupportedCultures = new List<CultureInfo>
            {
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.GetCultureInfo("de-DE"),
            },
            DefaultCulture = CultureInfo.GetCultureInfo("de-DE"),
            DefaultComponentType = typeof(TestComponent),
            DefaultLayoutType = typeof(TestLayout),
        };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsTrue();
            _ = await Assert.That(result.Failed).IsFalse();
        }
    }

    private sealed class TestComponent : ComponentBase { }

    private sealed class TestLayout : LayoutComponentBase { }
}
