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
    public void PageSize_WhenSizeTooSmall_ThrowsArgumentOutOfRangeException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = Assert.Throws<ArgumentOutOfRangeException>("size", () => config.PageSize(Defaults.PageSizeMinimum - 1));
    }

    [Test]
    public void PageSize_WhenSizeTooLarge_ThrowsArgumentOutOfRangeException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = Assert.Throws<ArgumentOutOfRangeException>("size", () => config.PageSize(Defaults.PageSizeMaximum + 1));
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
    public void UrlFormat_WithPrefixedFormatNoPrefix_ThrowsArgumentException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = Assert.Throws<ArgumentException>("prefix", () => config.UrlFormat(PaginationUrlFormat.Prefixed));
    }

    [Test]
    public void UrlFormat_WithPrefixedFormatEmptyPrefix_ThrowsArgumentException()
    {
        var state = new PaginationConfigurationBuilderState();
        var config = new PaginationConfiguration(state);

        _ = Assert.Throws<ArgumentException>("prefix", () => config.UrlFormat(PaginationUrlFormat.Prefixed, ""));
    }

    [Test]
    public void Constructor_WhenStateNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>("pagination", () => _ = new PaginationConfiguration(null!));
}
