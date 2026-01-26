namespace NetEvolve.ForgingBlazor.Tests.Integration.Storage;

using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using NetEvolve.ForgingBlazor.Content;
using NetEvolve.ForgingBlazor.Storage;
using NetEvolve.ForgingBlazor.Storage.FileSystem;
using NetEvolve.ForgingBlazor.Tests.Integration.Fixtures;
using NSubstitute;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Integration tests for publishing workflow scenarios including draft/published states,
/// content expiration, and multi-culture fallback.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public sealed class PublishingWorkflowIntegrationTests : IAsyncDisposable
{
    private TestContentFixture _fixture = null!;
    private FileSystemStorageOptions _storageOptions = null!;

    [Before(Test)]
    public async Task SetupAsync()
    {
        _fixture = new TestContentFixture();
        await _fixture.InitializeAsync().ConfigureAwait(false);

        _storageOptions = new FileSystemStorageOptions();
        _ = _storageOptions.WithBasePath(_fixture.BaseDirectory);
    }

    [After(Test)]
    public async Task CleanupAsync()
    {
        if (_fixture != null)
        {
            await _fixture.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_fixture != null)
        {
            await _fixture.DisposeAsync().ConfigureAwait(false);
        }
    }

    [Test]
    public async Task DraftContentFilter_InDevelopment_IncludesDraftContent()
    {
        // Arrange
        var environment = new HostingEnvironment { EnvironmentName = Environments.Development };
        var filter = new DraftContentFilter(environment);

        var publishedContent = new TestContentDescriptor
        {
            Title = "Published",
            Slug = "published",
            PublishedDate = DateTimeOffset.UtcNow.AddDays(-1),
            Draft = false,
        };

        var draftContent = new TestContentDescriptor
        {
            Title = "Draft",
            Slug = "draft",
            PublishedDate = DateTimeOffset.UtcNow.AddDays(-1),
            Draft = true,
        };

        var contentList = new[] { publishedContent, draftContent };

        // Act
        var result = filter.FilterDrafts(contentList).ToList();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Count).IsEqualTo(2);
            _ = await Assert.That(result.Any(c => c.Slug == "published")).IsTrue();
            _ = await Assert.That(result.Any(c => c.Slug == "draft")).IsTrue();
        }
    }

    [Test]
    public async Task DraftContentFilter_InProduction_ExcludesDraftContent()
    {
        // Arrange
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };
        var filter = new DraftContentFilter(environment);

        var publishedContent = new TestContentDescriptor
        {
            Title = "Published",
            Slug = "published",
            PublishedDate = DateTimeOffset.UtcNow.AddDays(-1),
            Draft = false,
        };

        var draftContent = new TestContentDescriptor
        {
            Title = "Draft",
            Slug = "draft",
            PublishedDate = DateTimeOffset.UtcNow.AddDays(-1),
            Draft = true,
        };

        var contentList = new[] { publishedContent, draftContent };

        // Act
        var result = filter.FilterDrafts(contentList).ToList();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Count).IsEqualTo(1);
            _ = await Assert.That(result.Single().Slug).IsEqualTo("published");
        }
    }

    [Test]
    public async Task ContentExpirationService_WithExpiredContent_FiltersCorrectly()
    {
        // Arrange
        var fixedTime = DateTimeOffset.Parse("2026-01-25T12:00:00Z", CultureInfo.InvariantCulture);
        var timeProvider = Substitute.For<TimeProvider>();
        _ = timeProvider.GetUtcNow().Returns(fixedTime);

        var expirationService = new ContentExpirationService(timeProvider);

        var currentContent = new TestContentDescriptor
        {
            Title = "Current",
            Slug = "current",
            PublishedDate = DateTimeOffset.Parse("2026-01-20T10:00:00Z", CultureInfo.InvariantCulture),
            Draft = false,
            ExpiredAt = null,
        };

        var expiredContent = new TestContentDescriptor
        {
            Title = "Expired",
            Slug = "expired",
            PublishedDate = DateTimeOffset.Parse("2026-01-15T10:00:00Z", CultureInfo.InvariantCulture),
            Draft = false,
            ExpiredAt = DateTimeOffset.Parse("2026-01-20T23:59:59Z", CultureInfo.InvariantCulture),
        };

        var contentList = new[] { currentContent, expiredContent };

        // Act
        var result = expirationService.FilterExpired(contentList).ToList();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Count).IsEqualTo(1);
            _ = await Assert.That(result.Single().Slug).IsEqualTo("current");
        }
    }

    [Test]
    public async Task ContentExpirationService_WithFutureExpirationDate_IncludesContent()
    {
        // Arrange
        var fixedTime = DateTimeOffset.Parse("2026-01-25T12:00:00Z", CultureInfo.InvariantCulture);
        var timeProvider = Substitute.For<TimeProvider>();
        _ = timeProvider.GetUtcNow().Returns(fixedTime);

        var expirationService = new ContentExpirationService(timeProvider);

        var content = new TestContentDescriptor
        {
            Title = "Future Expiration",
            Slug = "future-expiration",
            PublishedDate = DateTimeOffset.Parse("2026-01-20T10:00:00Z", CultureInfo.InvariantCulture),
            Draft = false,
            ExpiredAt = DateTimeOffset.Parse("2026-02-01T23:59:59Z", CultureInfo.InvariantCulture),
        };

        // Act
        var isExpired = expirationService.IsExpired(content);

        // Assert
        _ = await Assert.That(isExpired).IsFalse();
    }

    [Test]
    public async Task FileSystemContentStorageProvider_WithPublishedContent_ReturnsContent()
    {
        // Arrange
        using var provider = new FileSystemContentStorageProvider(_storageOptions);
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act
        var result = await provider
            .GetContentAsync<TestContentDescriptor>("blog", "getting-started", culture)
            .ConfigureAwait(false);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.Title).IsEqualTo("Getting Started with Blazor");
            _ = await Assert.That(result.Slug).IsEqualTo("getting-started");
            _ = await Assert.That(result.Draft).IsFalse();
        }
    }

    [Test]
    public async Task FileSystemContentStorageProvider_WithDraftContent_ReturnsContent()
    {
        // Arrange
        using var provider = new FileSystemContentStorageProvider(_storageOptions);
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act
        var result = await provider
            .GetContentAsync<TestContentDescriptor>("blog", "work-in-progress", culture)
            .ConfigureAwait(false);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.Title).IsEqualTo("Work in Progress");
            _ = await Assert.That(result.Draft).IsTrue();
        }
    }

    [Test]
    public async Task FileSystemContentStorageProvider_WithMultipleCultures_ReturnsCultureSpecificContent()
    {
        // Arrange
        using var provider = new FileSystemContentStorageProvider(_storageOptions);
        var englishCulture = CultureInfo.GetCultureInfo("en-US");
        var germanCulture = CultureInfo.GetCultureInfo("de-DE");

        // Act
        var englishContent = await provider
            .GetContentAsync<TestContentDescriptor>("blog", "getting-started", englishCulture)
            .ConfigureAwait(false);
        var germanContent = await provider
            .GetContentAsync<TestContentDescriptor>("blog", "getting-started", germanCulture)
            .ConfigureAwait(false);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(englishContent).IsNotNull();
            _ = await Assert.That(englishContent!.Title).IsEqualTo("Getting Started with Blazor");

            _ = await Assert.That(germanContent).IsNotNull();
            _ = await Assert.That(germanContent!.Title).IsEqualTo("Erste Schritte mit Blazor");
        }
    }

    [Test]
    public async Task FileSystemContentStorageProvider_WithMissingTranslation_ReturnsNull()
    {
        // Arrange
        using var provider = new FileSystemContentStorageProvider(_storageOptions);
        var spanishCulture = CultureInfo.GetCultureInfo("es-ES");

        // Act
        var result = await provider
            .GetContentAsync<TestContentDescriptor>("blog", "getting-started", spanishCulture)
            .ConfigureAwait(false);

        // Assert - Should return null as there's no Spanish translation
        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task FileSystemContentStorageProvider_GetContentsAsync_ReturnsMultipleContents()
    {
        // Arrange
        using var provider = new FileSystemContentStorageProvider(_storageOptions);
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act
        var results = await provider.GetContentsAsync<TestContentDescriptor>("blog", culture).ConfigureAwait(false);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(results.Count).IsGreaterThanOrEqualTo(2);
            _ = await Assert.That(results.Any(c => c.Slug == "getting-started")).IsTrue();
            _ = await Assert.That(results.Any(c => c.Slug == "advanced-routing")).IsTrue();
        }
    }

    [Test]
    public async Task PublishingWorkflow_DraftToPublished_ContentVisibilityChanges()
    {
        // Arrange
        var productionEnvironment = new HostingEnvironment { EnvironmentName = Environments.Production };
        var filter = new DraftContentFilter(productionEnvironment);

        // Initial state: draft content
        var draftContent = new TestContentDescriptor
        {
            Title = "New Post",
            Slug = "new-post",
            PublishedDate = DateTimeOffset.UtcNow,
            Draft = true,
        };

        // Act 1: Filter draft content in production
        var shouldFilterDraft = filter.ShouldFilter(draftContent);

        // Simulate publishing: change draft flag to false
        draftContent.Draft = false;

        // Act 2: Filter published content in production
        var shouldFilterPublished = filter.ShouldFilter(draftContent);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(shouldFilterDraft).IsTrue();
            _ = await Assert.That(shouldFilterPublished).IsFalse();
        }
    }

    [Test]
    public async Task PublishingWorkflow_WithExpirationDate_ContentBecomesInvisible()
    {
        // Arrange - Time before expiration
        var beforeExpirationTime = DateTimeOffset.Parse("2026-01-17T10:00:00Z", CultureInfo.InvariantCulture);
        var timeProviderBefore = Substitute.For<TimeProvider>();
        _ = timeProviderBefore.GetUtcNow().Returns(beforeExpirationTime);

        var expirationServiceBefore = new ContentExpirationService(timeProviderBefore);

        var content = new TestContentDescriptor
        {
            Title = "Limited Offer",
            Slug = "limited-offer",
            PublishedDate = DateTimeOffset.Parse("2026-01-15T10:00:00Z", CultureInfo.InvariantCulture),
            Draft = false,
            ExpiredAt = DateTimeOffset.Parse("2026-01-18T23:59:59Z", CultureInfo.InvariantCulture),
        };

        // Act 1: Check before expiration
        var isExpiredBefore = expirationServiceBefore.IsExpired(content);

        // Arrange - Time after expiration
        var afterExpirationTime = DateTimeOffset.Parse("2026-01-20T10:00:00Z", CultureInfo.InvariantCulture);
        var timeProviderAfter = Substitute.For<TimeProvider>();
        _ = timeProviderAfter.GetUtcNow().Returns(afterExpirationTime);

        var expirationServiceAfter = new ContentExpirationService(timeProviderAfter);

        // Act 2: Check after expiration
        var isExpiredAfter = expirationServiceAfter.IsExpired(content);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(isExpiredBefore).IsFalse();
            _ = await Assert.That(isExpiredAfter).IsTrue();
        }
    }

    private sealed class TestContentDescriptor : ContentDescriptor
    {
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public TestContentDescriptor()
        {
            Title = string.Empty;
            Slug = string.Empty;
            PublishedDate = DateTimeOffset.MinValue;
        }
    }
}
