namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using global::NetEvolve.ForgingBlazor.Routing;
using Microsoft.AspNetCore.Components;
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
}
