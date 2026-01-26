namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using Microsoft.AspNetCore.Components;
using NetEvolve.ForgingBlazor.Routing;
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

    [Test]
    public async Task Register_WithMultipleRoutes_AllRetrievable()
    {
        var registry = new RouteRegistry();
        var definition1 = new RouteDefinition("/test1", typeof(RouteView), null, null, null, null);
        var definition2 = new RouteDefinition("/test2", typeof(LayoutView), null, null, null, null);
        var definition3 = new RouteDefinition("/test3", typeof(RouteView), null, null, null, null);

        registry.Register("/test1", definition1);
        registry.Register("/test2", definition2);
        registry.Register("/test3", definition3);

        using (Assert.Multiple())
        {
            _ = await Assert.That(registry.TryGetRoute("/test1", out var r1)).IsTrue();
            _ = await Assert.That(r1).IsEqualTo(definition1);
            _ = await Assert.That(registry.TryGetRoute("/test2", out var r2)).IsTrue();
            _ = await Assert.That(r2).IsEqualTo(definition2);
            _ = await Assert.That(registry.TryGetRoute("/test3", out var r3)).IsTrue();
            _ = await Assert.That(r3).IsEqualTo(definition3);
        }
    }

    [Test]
    public async Task Register_CaseSensitive_DifferentRoutes()
    {
        var registry = new RouteRegistry();
        var definition1 = new RouteDefinition("/Test", typeof(RouteView), null, null, null, null);
        var definition2 = new RouteDefinition("/test", typeof(LayoutView), null, null, null, null);

        registry.Register("/Test", definition1);
        registry.Register("/test", definition2);

        var all = registry.GetAll();

        _ = await Assert.That(all.Count).IsEqualTo(2);
    }

    [Test]
    public async Task TryGetRoute_CaseSensitive_ExactMatch()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/Test", typeof(RouteView), null, null, null, null);
        registry.Register("/Test", definition);

        var result1 = registry.TryGetRoute("/Test", out _);
        var result2 = registry.TryGetRoute("/test", out _);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result1).IsTrue();
            _ = await Assert.That(result2).IsFalse();
        }
    }

    [Test]
    public async Task GetAll_AfterClear_IsEmpty()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);
        registry.Register("/test", definition);

        registry.Clear();

        var all = registry.GetAll();

        _ = await Assert.That(all.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ThreadSafety_ConcurrentRegistration()
    {
        var registry = new RouteRegistry();
        var tasks = new List<Task>();

        for (var i = 0; i < 10; i++)
        {
            var index = i;
            var task = Task.Run(() =>
            {
                var definition = new RouteDefinition($"/test{index}", typeof(RouteView), null, null, null, null);
                registry.Register($"/test{index}", definition);
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        var all = registry.GetAll();

        _ = await Assert.That(all.Count).IsEqualTo(10);
    }

    [Test]
    public async Task ThreadSafety_ConcurrentLookup()
    {
        var registry = new RouteRegistry();
        var definition = new RouteDefinition("/test", typeof(RouteView), null, null, null, null);
        registry.Register("/test", definition);

        var tasks = new List<Task>();
        var successCount = 0;

        for (var i = 0; i < 10; i++)
        {
            var task = Task.Run(() =>
            {
                if (registry.TryGetRoute("/test", out _))
                {
                    _ = Interlocked.Increment(ref successCount);
                }
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        _ = await Assert.That(successCount).IsEqualTo(10);
    }
}
