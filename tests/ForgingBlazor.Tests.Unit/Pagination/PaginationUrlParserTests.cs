namespace NetEvolve.ForgingBlazor.Tests.Unit.Pagination;

using System;
using NetEvolve.ForgingBlazor.Pagination;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="PaginationUrlParser"/>.
/// </summary>
public sealed class PaginationUrlParserTests
{
    [Test]
    public async Task TryParse_WithNullSegment_ThrowsArgumentNullException()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act & Assert
        _ = await Assert
            .That(() => PaginationUrlParser.TryParse(null!, settings, out _))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task TryParse_WithNullSettings_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert.That(() => PaginationUrlParser.TryParse("2", null!, out _)).Throws<ArgumentNullException>();

    [Test]
    public async Task TryParse_NumericFormat_ValidNumber_ReturnsTrue()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);

        // Act
        var result = PaginationUrlParser.TryParse("5", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(5);
    }

    [Test]
    public async Task TryParse_PrefixedFormat_ValidSegment_ReturnsTrue()
    {
        // Arrange
        var settings = new PaginationSettings(10, PaginationUrlFormat.Prefixed, "page");

        // Act
        var result = PaginationUrlParser.TryParse("page-3", settings, out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(3);
    }

    [Test]
    public async Task TryParseNumeric_WithNullSegment_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert.That(() => PaginationUrlParser.TryParseNumeric(null!, out _)).Throws<ArgumentNullException>();

    [Test]
    public async Task TryParseNumeric_WithEmptySegment_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParseNumeric("", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParseNumeric_WithWhitespace_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParseNumeric("   ", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParseNumeric_WithValidNumber_ReturnsTrue()
    {
        // Act
        var result = PaginationUrlParser.TryParseNumeric("42", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(42);
    }

    [Test]
    public async Task TryParseNumeric_WithZero_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParseNumeric("0", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParseNumeric_WithNegativeNumber_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParseNumeric("-5", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParseNumeric_WithNonNumericText_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParseNumeric("abc", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_WithNullSegment_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert
            .That(() => PaginationUrlParser.TryParsePrefixed(null!, "page", out _))
            .Throws<ArgumentNullException>();

    [Test]
    public async Task TryParsePrefixed_WithNullPrefix_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert
            .That(() => PaginationUrlParser.TryParsePrefixed("page-2", null!, out _))
            .Throws<ArgumentNullException>();

    [Test]
    public async Task TryParsePrefixed_WithEmptySegment_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_WithEmptyPrefix_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("page-2", "", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_WithValidSegment_ReturnsTrue()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("page-7", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(7);
    }

    [Test]
    public async Task TryParsePrefixed_WithMissingPrefix_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("7", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_WithWrongPrefix_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("other-7", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_WithMissingNumber_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("page-", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_WithZero_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("page-0", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_WithNegativeNumber_ReturnsFalse()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("page--3", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsFalse();
        _ = await Assert.That(pageNumber).IsEqualTo(0);
    }

    [Test]
    public async Task TryParsePrefixed_CaseInsensitive_ReturnsTrue()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("PAGE-5", "page", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(5);
    }

    [Test]
    public async Task TryParsePrefixed_WithCustomPrefix_ReturnsTrue()
    {
        // Act
        var result = PaginationUrlParser.TryParsePrefixed("seite-3", "seite", out var pageNumber);

        // Assert
        _ = await Assert.That(result).IsTrue();
        _ = await Assert.That(pageNumber).IsEqualTo(3);
    }
}
