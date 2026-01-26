namespace NetEvolve.ForgingBlazor.Tests.Unit.Content;

using System;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using NetEvolve.ForgingBlazor.Content;
using NetEvolve.ForgingBlazor.Tests.Unit.Storage;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="ContentCacheService"/>.
/// </summary>
public sealed class ContentCacheServiceTests
{
    [Test]
    public async Task GetOrCreate_WhenContentNotCached_CallsFactory()
    {
        // Arrange
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ContentCacheService(cache);
        var culture = CultureInfo.GetCultureInfo("en-US");
        var factoryCalled = false;

        // Act
        var result = service.GetOrCreate(
            "blog",
            "test",
            culture,
            factory: () =>
            {
                factoryCalled = true;
                return new TestContentDescriptor
                {
                    Title = "Test",
                    Slug = "test",
                    PublishedDate = DateTimeOffset.UtcNow,
                };
            }
        );

        // Assert
        _ = await Assert.That(factoryCalled).IsTrue();
    }

    [Test]
    public async Task Invalidate_RemovesCachedEntry()
    {
        // Arrange
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new ContentCacheService(cache);
        var culture = CultureInfo.GetCultureInfo("en-US");

        _ = service.GetOrCreate(
            "blog",
            "test",
            culture,
            factory: () =>
                new TestContentDescriptor
                {
                    Title = "Test",
                    Slug = "test",
                    PublishedDate = DateTimeOffset.UtcNow,
                }
        );

        // Act
        service.Invalidate("blog", "test", culture);

        var factoryCalled = false;
        _ = service.GetOrCreate(
            "blog",
            "test",
            culture,
            factory: () =>
            {
                factoryCalled = true;
                return new TestContentDescriptor
                {
                    Title = "Updated",
                    Slug = "test",
                    PublishedDate = DateTimeOffset.UtcNow,
                };
            }
        );

        // Assert
        _ = await Assert.That(factoryCalled).IsTrue();
    }
}
