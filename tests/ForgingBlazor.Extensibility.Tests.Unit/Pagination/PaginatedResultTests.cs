namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit.Pagination;

using System;
using System.Collections.Generic;
using NetEvolve.ForgingBlazor.Pagination;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="PaginatedResult{T}"/>.
/// </summary>
public sealed class PaginatedResultTests
{
    [Test]
    public async Task Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2", "Item3" }.AsReadOnly();

        // Act
        var result = new PaginatedResult<string>(
            items,
            CurrentPage: 1,
            TotalPages: 3,
            TotalItems: 10,
            HasPrevious: false,
            HasNext: true
        );

        // Assert
        _ = await Assert.That(result.Items).IsEqualTo(items);
        _ = await Assert.That(result.CurrentPage).IsEqualTo(1);
        _ = await Assert.That(result.TotalPages).IsEqualTo(3);
        _ = await Assert.That(result.TotalItems).IsEqualTo(10);
        _ = await Assert.That(result.HasPrevious).IsFalse();
        _ = await Assert.That(result.HasNext).IsTrue();
    }

    [Test]
    public async Task Constructor_WithEmptyItems_CreatesInstance()
    {
        // Arrange
        var items = Array.Empty<string>();

        // Act
        var result = new PaginatedResult<string>(
            items,
            CurrentPage: 1,
            TotalPages: 1,
            TotalItems: 0,
            HasPrevious: false,
            HasNext: false
        );

        // Assert
        _ = await Assert.That(result.Items.Count).IsEqualTo(0);
        _ = await Assert.That(result.CurrentPage).IsEqualTo(1);
        _ = await Assert.That(result.TotalPages).IsEqualTo(1);
        _ = await Assert.That(result.TotalItems).IsEqualTo(0);
        _ = await Assert.That(result.HasPrevious).IsFalse();
        _ = await Assert.That(result.HasNext).IsFalse();
    }

    [Test]
    public async Task Constructor_LastPage_HasNoPreviousAndNoNext()
    {
        // Arrange
        var items = new List<string> { "Item1" }.AsReadOnly();

        // Act
        var result = new PaginatedResult<string>(
            items,
            CurrentPage: 3,
            TotalPages: 3,
            TotalItems: 21,
            HasPrevious: true,
            HasNext: false
        );

        // Assert
        _ = await Assert.That(result.HasPrevious).IsTrue();
        _ = await Assert.That(result.HasNext).IsFalse();
    }

    [Test]
    public async Task Constructor_MiddlePage_HasPreviousAndNext()
    {
        // Arrange
        var items = new List<string> { "Item1", "Item2" }.AsReadOnly();

        // Act
        var result = new PaginatedResult<string>(
            items,
            CurrentPage: 2,
            TotalPages: 3,
            TotalItems: 25,
            HasPrevious: true,
            HasNext: true
        );

        // Assert
        _ = await Assert.That(result.HasPrevious).IsTrue();
        _ = await Assert.That(result.HasNext).IsTrue();
    }
}
