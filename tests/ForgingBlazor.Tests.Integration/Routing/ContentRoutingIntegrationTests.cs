namespace NetEvolve.ForgingBlazor.Tests.Integration.Routing;

using System;
using System.Threading.Tasks;
using NetEvolve.ForgingBlazor.Routing;
using NetEvolve.ForgingBlazor.Storage;
using NetEvolve.ForgingBlazor.Tests.Integration.Fixtures;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Integration tests for content routing end-to-end scenarios.
/// Tests the full request/response cycle including route resolution, content loading, and component rendering.
/// </summary>
public sealed class ContentRoutingIntegrationTests : IAsyncDisposable
{
    private TestContentFixture _fixture = null!;
    private FileSystemStorageOptions _storageOptions = null!;

    [Before(Test)]
    public async Task SetupAsync()
    {
        _fixture = new TestContentFixture();
        await _fixture.InitializeAsync();

        _storageOptions = new FileSystemStorageOptions();
        _ = _storageOptions.WithBasePath(_fixture.BaseDirectory);
    }

    [After(Test)]
    public async Task CleanupAsync()
    {
        if (_fixture != null)
        {
            await _fixture.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_fixture != null)
        {
            await _fixture.DisposeAsync();
        }
    }

    [Test]
    public async Task RouteRegistry_RegisterAndRetrieve_WorksCorrectly()
    {
        // Arrange
        var registry = new RouteRegistry();
        var definition = new RouteDefinition(
            PathPattern: "/blog",
            ComponentType: typeof(TestBlogComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );

        // Act
        registry.Register("/blog", definition);
        var result = registry.TryGetRoute("/blog", out var retrieved);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(retrieved).IsNotNull();
            _ = await Assert.That(retrieved!.PathPattern).IsEqualTo("/blog");
            _ = await Assert.That(retrieved.ComponentType).IsEqualTo(typeof(TestBlogComponent));
        }
    }

    [Test]
    public async Task RouteRegistry_DuplicateRegistration_ThrowsException()
    {
        // Arrange
        var registry = new RouteRegistry();
        var definition1 = new RouteDefinition(
            PathPattern: "/blog",
            ComponentType: typeof(TestBlogComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );
        var definition2 = new RouteDefinition(
            PathPattern: "/blog",
            ComponentType: typeof(TestPageComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );

        registry.Register("/blog", definition1);

        // Act & Assert
        _ = Assert.Throws<InvalidOperationException>(() => registry.Register("/blog", definition2));
    }

    [Test]
    public async Task RouteResolver_WithValidPath_ResolvesRoute()
    {
        // Arrange
        var registry = new RouteRegistry();
        var definition = new RouteDefinition(
            PathPattern: "/blog",
            ComponentType: typeof(TestBlogComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );
        registry.Register("/blog", definition);

        var resolver = new RouteResolver(registry);

        // Act
        var result = resolver.TryResolve("/blog", out var resolved);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsNotNull();
            _ = await Assert.That(resolved!.PathPattern).IsEqualTo("/blog");
        }
    }

    [Test]
    public async Task RouteResolver_WithNonExistentPath_ReturnsFalse()
    {
        // Arrange
        var registry = new RouteRegistry();
        var resolver = new RouteResolver(registry);

        // Act
        var result = resolver.TryResolve("/nonexistent", out var definition);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsFalse();
            _ = await Assert.That(definition).IsNull();
        }
    }

    [Test]
    public async Task RouteResolver_WithTrailingSlash_NormalizesAndResolves()
    {
        // Arrange
        var registry = new RouteRegistry();
        var definition = new RouteDefinition(
            PathPattern: "/blog",
            ComponentType: typeof(TestBlogComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );
        registry.Register("/blog", definition);

        var resolver = new RouteResolver(registry);

        // Act
        var result = resolver.TryResolve("/blog/", out var resolved);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsNotNull();
            _ = await Assert.That(resolved!.PathPattern).IsEqualTo("/blog");
        }
    }

    [Test]
    public async Task RouteResolver_WithRootPath_ResolvesRoute()
    {
        // Arrange
        var registry = new RouteRegistry();
        var definition = new RouteDefinition(
            PathPattern: "/",
            ComponentType: typeof(TestPageComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );
        registry.Register("/", definition);

        var resolver = new RouteResolver(registry);

        // Act
        var result = resolver.TryResolve("/", out var resolved);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsNotNull();
            _ = await Assert.That(resolved!.PathPattern).IsEqualTo("/");
        }
    }

    [Test]
    public async Task RouteRegistry_GetAll_ReturnsAllRoutes()
    {
        // Arrange
        var registry = new RouteRegistry();
        var definition1 = new RouteDefinition(
            PathPattern: "/blog",
            ComponentType: typeof(TestBlogComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );
        var definition2 = new RouteDefinition(
            PathPattern: "/about",
            ComponentType: typeof(TestPageComponent),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );

        registry.Register("/blog", definition1);
        registry.Register("/about", definition2);

        // Act
        var all = registry.GetAll();

        // Assert
        _ = await Assert.That(all.Count).IsEqualTo(2);
    }

    private sealed class TestBlogComponent : Microsoft.AspNetCore.Components.ComponentBase { }

    private sealed class TestPageComponent : Microsoft.AspNetCore.Components.ComponentBase { }
}
