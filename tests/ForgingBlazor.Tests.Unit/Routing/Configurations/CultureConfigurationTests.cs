namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing.Configurations;

using System;
using System.Globalization;
using global::NetEvolve.ForgingBlazor.Routing;
using global::NetEvolve.ForgingBlazor.Routing.Configurations;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class CultureConfigurationTests
{
    [Test]
    public async Task Default_WithCultureInfo_SetsDefaultCulture()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);
        var culture = CultureInfo.GetCultureInfo("en-US");

        var result = config.Default(culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.DefaultCulture).IsEqualTo(CultureInfo.ReadOnly(culture));
        }
    }

    [Test]
    public async Task Default_WithString_SetsDefaultCulture()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        var result = config.Default("de-DE");

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.DefaultCulture?.Name).IsEqualTo("de-DE");
        }
    }

    [Test]
    public async Task Default_WithLcid_SetsDefaultCulture()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        var result = config.Default(1033);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.DefaultCulture?.Name).IsEqualTo("en-US");
        }
    }

    [Test]
    public async Task Default_WhenCultureNull_ThrowsArgumentNullException()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        _ = await Assert
            .That(() => config.Default((CultureInfo)null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("culture");
    }

    [Test]
    public async Task Default_WhenStringEmpty_ThrowsArgumentException()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        _ = await Assert
            .That(() => config.Default(string.Empty))
            .ThrowsExactly<ArgumentException>()
            .WithParameterName("culture");
    }

    [Test]
    public async Task Supported_WithCultureInfoArray_AddsSupportedCultures()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);
        var cultures = new[] { CultureInfo.GetCultureInfo("en-US"), CultureInfo.GetCultureInfo("de-DE") };

        var result = config.Supported(cultures);

        _ = await Assert.That(result).IsEqualTo(config);
    }

    [Test]
    public async Task Supported_WithStringArray_AddsSupportedCultures()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        var result = config.Supported("en-US", "de-DE", "fr-FR");

        _ = await Assert.That(result).IsEqualTo(config);
    }

    [Test]
    public async Task Supported_WithLcidArray_AddsSupportedCultures()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        var result = config.Supported(1033, 1031);

        _ = await Assert.That(result).IsEqualTo(config);
    }

    [Test]
    public async Task Supported_WhenCultureArrayNull_ThrowsArgumentNullException()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        _ = await Assert
            .That(() => config.Supported((CultureInfo[])null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("cultures");
    }

    [Test]
    public async Task Supported_WhenStringArrayNull_ThrowsArgumentNullException()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        _ = await Assert
            .That(() => config.Supported((string[])null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("cultures");
    }

    [Test]
    public async Task Supported_WhenLcidArrayNull_ThrowsArgumentNullException()
    {
        var state = new CultureConfigurationBuilderState();
        var config = new CultureConfiguration(state);

        _ = await Assert
            .That(() => config.Supported((int[])null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("lcids");
    }

    [Test]
    public async Task Constructor_WhenStateNull_ThrowsArgumentNullException() =>
        _ = await Assert
            .That(() => new CultureConfiguration(null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("state");
}
