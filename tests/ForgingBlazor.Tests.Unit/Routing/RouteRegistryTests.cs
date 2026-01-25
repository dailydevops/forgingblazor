namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using global::NetEvolve.ForgingBlazor.Routing;
using Microsoft.AspNetCore.Components;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class RouteRegistryTests
{
    [Test]
    public async Task Register_WithValidRoute_AddsToRegistry()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);

        registry.Register("/test", definition);

        using (Assert.Multiple())
        {
            _ = await Assert.That(registry.TryGetRoute("/test", out var retrieved)).IsTrue();
            _ = await Assert.That(retrieved).IsEqualTo(definition);
        }
    }

    [Test]
    public void Register_WithNullPathPattern_ThrowsArgumentNullException()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);

        _ = Assert.Throws<ArgumentNullException>("pathPattern", () => registry.Register(null!, definition));
    }

    [Test]
    public void Register_WithNullDefinition_ThrowsArgumentNullException()
    {
        var registry = new RouteRegistry();

        _ = Assert.Throws<ArgumentNullException>("definition", () => registry.Register("/test", null!));
    }

    [Test]
    public void Register_WithEmptyPathPattern_ThrowsArgumentException()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);

        _ = Assert.Throws<ArgumentException>("pathPattern", () => registry.Register("", definition));
    }

    [Test]
    public void Register_WithWhitespacePathPattern_ThrowsArgumentException()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);

        _ = Assert.Throws<ArgumentException>("pathPattern", () => registry.Register("   ", definition));
    }

    [Test]
    public void Register_WithDuplicatePathPattern_ThrowsInvalidOperationException()
    {
        var registry = new RouteRegistry();
        var definition1 = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);
        var definition2 = new RouteDefinition("/test", typeof(LayoutView), null, null, null, null);

        registry.Register("/test", definition1);

        _ = Assert.Throws<InvalidOperationException>(() => registry.Register("/test", definition2));
    }

    [Test]
    public async Task TryGetRoute_WithExistingRoute_ReturnsTrue()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);
        registry.Register("/test", definition);

        var result = registry.TryGetRoute("/test", out var retrieved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsTrue();
            _ = await Assert.That(retrieved).IsEqualTo(definition);
        }
    }

    [Test]
    public async Task TryGetRoute_WithNonExistingRoute_ReturnsFalse()
    {
        var registry = new RouteRegistry();

        var result = registry.TryGetRoute("/nonexistent", out var retrieved);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsFalse();
            _ = await Assert.That(retrieved).IsNull();
        }
    }

    [Test]
    public void TryGetRoute_WithNullPathPattern_ThrowsArgumentNullException()
    {
        var registry = new RouteRegistry();

        _ = Assert.Throws<ArgumentNullException>("pathPattern", () => registry.TryGetRoute(null!, out _));
    }

    [Test]
    public async Task GetAll_WithMultipleRoutes_ReturnsAllRoutes()
    {
        var registry = new RouteRegistry();
        var definition1 = new RouteDefinition("/test1", typeof(RouteView), null, null, null, null);
        var definition2 = new RouteDefinition("/test2", typeof(LayoutView), null, null, null, null);

        registry.Register("/test1", definition1);
        registry.Register("/test2", definition2);

        var all = registry.GetAll();

        using (Assert.Multiple())
        {
            _ = await Assert.That(all.Count).IsEqualTo(2);
            _ = await Assert.That(all.Contains(definition1)).IsTrue();
            _ = await Assert.That(all.Contains(definition2)).IsTrue();
        }
    }

    [Test]
    public async Task GetAll_WithNoRoutes_ReturnsEmptyCollection()
    {
        var registry = new RouteRegistry();

        var all = registry.GetAll();

        _ = await Assert.That(all.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Clear_RemovesAllRoutes()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);
        registry.Register("/test", definition);

        registry.Clear();

        var result = registry.TryGetRoute("/test", out _);
        _ = await Assert.That(result).IsFalse();
    }
}
