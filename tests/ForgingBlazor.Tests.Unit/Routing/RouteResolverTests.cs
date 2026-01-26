namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using Microsoft.AspNetCore.Components;
using NetEvolve.ForgingBlazor.Routing;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class RouteResolverTests
{
    [Test]
    public async Task TryResolve_WithExactMatch_ReturnsTrue()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);
        registry.Register("/test", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("/test", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithNormalizedPath_ReturnsTrue()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);
        registry.Register("/test", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("test/", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithNonExistingPath_ReturnsFalse()
    {
        var registry = new RouteRegistry();
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("/nonexistent", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsFalse();
            _ = await Assert.That(resolved).IsNull();
        }
    }

    [Test]
    public void TryResolve_WithNullRequestPath_ThrowsArgumentNullException()
    {
        var registry = new RouteRegistry();
        var resolver = new RouteResolver(registry);

        _ = Assert.Throws<ArgumentNullException>("requestPath", () => resolver.TryResolve(null!, out _));
    }

    [Test]
    public void Constructor_WithNullRegistry_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>("registry", () => _ = new RouteResolver(null!));

    [Test]
    public async Task TryResolve_WithRootPath_NormalizesToSlash()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/", typeof(RouteView), null, null, null, null);
        registry.Register("/", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithTrailingSlash_Normalizes()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/blog/post", typeof(RouteView), null, null, null, null);
        registry.Register("/blog/post", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("/blog/post/", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithoutLeadingSlash_Normalizes()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        registry.Register("/blog", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("blog", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithCaseInsensitiveMatch_ReturnsTrue()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/BLOG", typeof(RouteView), null, null, null, null);
        registry.Register("/BLOG", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("/blog", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithMultipleSegments_ReturnsCorrectRoute()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/blog/posts/article", typeof(RouteView), null, null, null, null);
        registry.Register("/blog/posts/article", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("/blog/posts/article", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithEmptyPath_NormalizesToRoot()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/", typeof(RouteView), null, null, null, null);
        registry.Register("/", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithPartialPathMatch_ReturnsMatchingRoute()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        registry.Register("/blog", definition);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("/blog/posts/my-article", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryResolve_WithMultipleRegisteredRoutes_ReturnsCorrectRoute()
    {
        var registry = new RouteRegistry();
        var definition1 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        var definition2 = new RouteDefinition("/posts", typeof(LayoutView), null, null, null, null);
        registry.Register("/blog", definition1);
        registry.Register("/posts", definition2);
        var resolver = new RouteResolver(registry);

        var result = resolver.TryResolve("/posts", out var resolved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(resolved).IsEqualTo(definition2);
        }
    }
}
