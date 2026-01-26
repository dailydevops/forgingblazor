namespace NetEvolve.ForgingBlazor.Tests.Unit.Content;

using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using NetEvolve.ForgingBlazor.Content;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="ContentCacheInvalidationHandler"/>.
/// </summary>
public sealed class ContentCacheInvalidationHandlerTests
{
    [Test]
    public async Task Constructor_WithNullCacheService_ThrowsArgumentNullException() =>
        // Act & Assert
        _ = await Assert
            .That(() =>
                new ContentCacheInvalidationHandler(null!, NullLogger<ContentCacheInvalidationHandler>.Instance)
            )
            .Throws<ArgumentNullException>();

    [Test]
    public async Task Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var cacheService = new ContentCacheService(cache);

        // Act & Assert
        _ = await Assert
            .That(() => new ContentCacheInvalidationHandler(cacheService, null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task HandleFileChange_WithNullPath_ThrowsArgumentNullException()
    {
        // Arrange
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var cacheService = new ContentCacheService(cache);
        var handler = new ContentCacheInvalidationHandler(
            cacheService,
            NullLogger<ContentCacheInvalidationHandler>.Instance
        );

        // Act & Assert
        _ = await Assert.That(() => handler.HandleFileChange(null!)).Throws<ArgumentNullException>();
    }
}
