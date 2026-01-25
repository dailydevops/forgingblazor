namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing.Configurations;

using System;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Routing;
using global::NetEvolve.ForgingBlazor.Routing.Configurations;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class PaginationConfigurationTests
{
    [Test]
    public async Task PageSize_WithValidSize_SetsPageSize()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        var result = config.PageSize(20);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.PageSize).IsEqualTo(20);
        }
    }

    [Test]
    public async Task PageSize_WithDefaultSize_SetsDefaultPageSize()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        var result = config.PageSize();

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.PageSize).IsEqualTo(Defaults.PageSizeDefault);
        }
    }

    [Test]
    public async Task PageSize_WithMinimumSize_SetsPageSize()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        var result = config.PageSize(Defaults.PageSizeMinimum);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.PageSize).IsEqualTo(Defaults.PageSizeMinimum);
        }
    }

    [Test]
    public async Task PageSize_WithMaximumSize_SetsPageSize()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        var result = config.PageSize(Defaults.PageSizeMaximum);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.PageSize).IsEqualTo(Defaults.PageSizeMaximum);
        }
    }

    [Test]
    public async Task PageSize_WhenSizeTooSmall_ThrowsArgumentOutOfRangeException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = await Assert
            .That(() => config.PageSize(Defaults.PageSizeMinimum - 1))
            .ThrowsExactly<ArgumentOutOfRangeException>()
            .WithParameterName("size");
    }

    [Test]
    public async Task PageSize_WhenSizeTooLarge_ThrowsArgumentOutOfRangeException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = await Assert
            .That(() => config.PageSize(Defaults.PageSizeMaximum + 1))
            .ThrowsExactly<ArgumentOutOfRangeException>()
            .WithParameterName("size");
    }

    [Test]
    public async Task UrlFormat_WithNumericFormat_SetsFormat()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        var result = config.UrlFormat(PaginationUrlFormat.Numeric);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.Format).IsEqualTo(PaginationUrlFormat.Numeric);
            _ = await Assert.That(state.Prefix).IsNull();
        }
    }

    [Test]
    public async Task UrlFormat_WithPrefixedFormatAndPrefix_SetsFormatAndPrefix()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        var result = config.UrlFormat(PaginationUrlFormat.Prefixed, "page");

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(config);
            _ = await Assert.That(state.Format).IsEqualTo(PaginationUrlFormat.Prefixed);
            _ = await Assert.That(state.Prefix).IsEqualTo("page");
        }
    }

    [Test]
    public async Task UrlFormat_WithPrefixedFormatNoPrefix_ThrowsArgumentException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = await Assert
            .That(() => config.UrlFormat(PaginationUrlFormat.Prefixed))
            .ThrowsExactly<ArgumentException>()
            .WithParameterName("prefix");
    }

    [Test]
    public async Task UrlFormat_WithPrefixedFormatEmptyPrefix_ThrowsArgumentException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = await Assert
            .That(() => config.UrlFormat(PaginationUrlFormat.Prefixed, ""))
            .ThrowsExactly<ArgumentException>()
            .WithParameterName("prefix");
    }

    [Test]
    public async Task Constructor_WhenStateNull_ThrowsArgumentNullException() =>
        _ = await Assert
            .That(() => new PaginationConfiguration(null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("pagination");
}
