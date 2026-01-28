namespace NetEvolve.ForgingBlazor.Tests.Unit.Pagination;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NetEvolve.ForgingBlazor.Pagination;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="PaginationService"/>.
/// </summary>
public sealed class PaginationServiceTests
{
    [Test]
    public async Task CreatePaginatedResult_WithNullItems_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.CreatePaginatedResult<string>(null!, 1, 10))
            .Throws<ArgumentNullException>();

    [Test]
    public async Task CreatePaginatedResult_WithPageNumberLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var items = new List<string> { "Item1" }.AsReadOnly();

        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.CreatePaginatedResult(items, 0, 10))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task CreatePaginatedResult_WithPageSizeLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var items = new List<string> { "Item1" }.AsReadOnly();

        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.CreatePaginatedResult(items, 1, 0))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task CreatePaginatedResult_WithEmptyCollection_ReturnsEmptyResult()
    {
        // Arrange
        var items = Array.Empty<string>();

        // Act
        var result = PaginationService.CreatePaginatedResult(items, 1, 10);

        // Assert
        _ = await Assert.That(result.Items.Count).IsEqualTo(0);
        _ = await Assert.That(result.CurrentPage).IsEqualTo(1);
        _ = await Assert.That(result.TotalPages).IsEqualTo(1);
        _ = await Assert.That(result.TotalItems).IsEqualTo(0);
        _ = await Assert.That(result.HasPrevious).IsFalse();
        _ = await Assert.That(result.HasNext).IsFalse();
    }

    [Test]
    public async Task CreatePaginatedResult_FirstPage_ReturnsCorrectResult()
    {
        // Arrange
        var items = CreateTestItems(25);

        // Act
        var result = PaginationService.CreatePaginatedResult(items, 1, 10);

        // Assert
        _ = await Assert.That(result.Items.Count).IsEqualTo(10);
        _ = await Assert.That(result.CurrentPage).IsEqualTo(1);
        _ = await Assert.That(result.TotalPages).IsEqualTo(3);
        _ = await Assert.That(result.TotalItems).IsEqualTo(25);
        _ = await Assert.That(result.HasPrevious).IsFalse();
        _ = await Assert.That(result.HasNext).IsTrue();
        _ = await Assert.That(result.Items[0]).IsEqualTo("Item1");
        _ = await Assert.That(result.Items[9]).IsEqualTo("Item10");
    }

    [Test]
    public async Task CreatePaginatedResult_MiddlePage_ReturnsCorrectResult()
    {
        // Arrange
        var items = CreateTestItems(25);

        // Act
        var result = PaginationService.CreatePaginatedResult(items, 2, 10);

        // Assert
        _ = await Assert.That(result.Items.Count).IsEqualTo(10);
        _ = await Assert.That(result.CurrentPage).IsEqualTo(2);
        _ = await Assert.That(result.TotalPages).IsEqualTo(3);
        _ = await Assert.That(result.TotalItems).IsEqualTo(25);
        _ = await Assert.That(result.HasPrevious).IsTrue();
        _ = await Assert.That(result.HasNext).IsTrue();
        _ = await Assert.That(result.Items[0]).IsEqualTo("Item11");
        _ = await Assert.That(result.Items[9]).IsEqualTo("Item20");
    }

    [Test]
    public async Task CreatePaginatedResult_LastPage_ReturnsCorrectResult()
    {
        // Arrange
        var items = CreateTestItems(25);

        // Act
        var result = PaginationService.CreatePaginatedResult(items, 3, 10);

        // Assert
        _ = await Assert.That(result.Items.Count).IsEqualTo(5);
        _ = await Assert.That(result.CurrentPage).IsEqualTo(3);
        _ = await Assert.That(result.TotalPages).IsEqualTo(3);
        _ = await Assert.That(result.TotalItems).IsEqualTo(25);
        _ = await Assert.That(result.HasPrevious).IsTrue();
        _ = await Assert.That(result.HasNext).IsFalse();
        _ = await Assert.That(result.Items[0]).IsEqualTo("Item21");
        _ = await Assert.That(result.Items[4]).IsEqualTo("Item25");
    }

    [Test]
    public async Task CreatePaginatedResult_ExactPageBoundary_ReturnsCorrectResult()
    {
        // Arrange
        var items = CreateTestItems(20);

        // Act
        var result = PaginationService.CreatePaginatedResult(items, 2, 10);

        // Assert
        _ = await Assert.That(result.Items.Count).IsEqualTo(10);
        _ = await Assert.That(result.TotalPages).IsEqualTo(2);
        _ = await Assert.That(result.HasNext).IsFalse();
    }

    [Test]
    public async Task IsValidPageNumber_WithNegativePageNumber_ReturnsFalse()
    {
        // Act
        var result = PaginationService.IsValidPageNumber(-1, 25, 10);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidPageNumber_WithZeroPageNumber_ReturnsFalse()
    {
        // Act
        var result = PaginationService.IsValidPageNumber(0, 25, 10);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidPageNumber_WithEmptyCollection_OnlyPageOneIsValid()
    {
        // Act
        var pageOneValid = PaginationService.IsValidPageNumber(1, 0, 10);
        var pageTwoValid = PaginationService.IsValidPageNumber(2, 0, 10);

        // Assert
        _ = await Assert.That(pageOneValid).IsTrue();
        _ = await Assert.That(pageTwoValid).IsFalse();
    }

    [Test]
    public async Task IsValidPageNumber_WithinRange_ReturnsTrue()
    {
        // Act
        var result = PaginationService.IsValidPageNumber(2, 25, 10);

        // Assert
        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidPageNumber_OutOfRange_ReturnsFalse()
    {
        // Act
        var result = PaginationService.IsValidPageNumber(4, 25, 10);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task GeneratePageUrl_WithNullBasePath_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.GeneratePageUrl(null!, 1, settings))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task GeneratePageUrl_WithNullSettings_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.GeneratePageUrl("/posts", 1, null!))
            .Throws<ArgumentNullException>();

    [Test]
    public async Task GeneratePageUrl_WithPageNumberLessThanOne_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.GeneratePageUrl("/posts", 0, settings))
            .Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task GeneratePageUrl_PageOne_ReturnsBasePath()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act
        var result = PaginationService.GeneratePageUrl("/posts", 1, settings);

        // Assert
        _ = await Assert.That(result).IsEqualTo("/posts");
    }

    [Test]
    public async Task GeneratePageUrl_NumericFormat_ReturnsNumericUrl()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act
        var result = PaginationService.GeneratePageUrl("/posts", 2, settings);

        // Assert
        _ = await Assert.That(result).IsEqualTo("/posts/2");
    }

    [Test]
    public async Task GeneratePageUrl_PrefixedFormat_ReturnsPrefixedUrl()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Prefixed, "page");

        // Act
        var result = PaginationService.GeneratePageUrl("/posts", 3, settings);

        // Assert
        _ = await Assert.That(result).IsEqualTo("/posts/page-3");
    }

    [Test]
    public async Task GeneratePageUrl_PrefixedFormatWithNullPrefix_UsesDefaultPrefix()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Prefixed, null);

        // Act
        var result = PaginationService.GeneratePageUrl("/posts", 2, settings);

        // Assert
        _ = await Assert.That(result).IsEqualTo("/posts/page-2");
    }

    [Test]
    public async Task TryParsePageNumber_WithNullSegment_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.TryParsePageNumber(null!, settings, out _))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task TryParsePageNumber_WithNullSettings_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert
            .That(() => PaginationService.TryParsePageNumber("2", null!, out _))
            .Throws<ArgumentNullException>();

    [Test]
    public async Task TryParsePageNumber_WithEmptySegment_ReturnsFalse()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act
        var result = PaginationService.TryParsePageNumber("", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePageNumber_NumericFormat_ValidNumber_ReturnsTrue()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act
        var result = PaginationService.TryParsePageNumber("5", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(5);
    }

    [Test]
    public async Task TryParsePageNumber_NumericFormat_Zero_ReturnsFalse()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act
        var result = PaginationService.TryParsePageNumber("0", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePageNumber_NumericFormat_NegativeNumber_ReturnsFalse()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act
        var result = PaginationService.TryParsePageNumber("-1", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePageNumber_PrefixedFormat_ValidSegment_ReturnsTrue()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Prefixed, "page");

        // Act
        var result = PaginationService.TryParsePageNumber("page-3", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(3);
    }

    [Test]
    public async Task TryParsePageNumber_PrefixedFormat_MissingPrefix_ReturnsFalse()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Prefixed, "page");

        // Act
        var result = PaginationService.TryParsePageNumber("3", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePageNumber_PrefixedFormat_WrongPrefix_ReturnsFalse()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Prefixed, "page");

        // Act
        var result = PaginationService.TryParsePageNumber("other-3", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePageNumber_PrefixedFormat_MissingNumber_ReturnsFalse()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Prefixed, "page");

        // Act
        var result = PaginationService.TryParsePageNumber("page-", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    private static ReadOnlyCollection<string> CreateTestItems(int count)
    {
        var items = new List<string>();
        for (var i = 1; i <= count; i++)
        {
            items.Add($"Item{i}");
        }

        return items.AsReadOnly();
    }
}
