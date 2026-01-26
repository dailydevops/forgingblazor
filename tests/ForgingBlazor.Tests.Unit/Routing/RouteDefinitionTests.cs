namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using Microsoft.AspNetCore.Components;
using NetEvolve.ForgingBlazor.Routing;
using NetEvolve.ForgingBlazor.Tests.Unit.Storage;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class RouteDefinitionTests
{
    [Test]
    public async Task Create_WithValidParameters_InitializesProperties()
    {
        var componentType = typeof(RouteView);
        var layoutType = typeof(LayoutView);
        var contentType = typeof(TestContentDescriptor);

        var definition = new RouteDefinition("/blog", componentType, layoutType, contentType, null, null);

        using (Assert.Multiple())
        {
            _ = await Assert.That(definition.PathPattern).IsEqualTo("/blog");
            _ = await Assert.That(definition.ComponentType).IsEqualTo(componentType);
            _ = await Assert.That(definition.LayoutType).IsEqualTo(layoutType);
            _ = await Assert.That(definition.ContentType).IsEqualTo(contentType);
            _ = await Assert.That(definition.PaginationSettings).IsNull();
            _ = await Assert.That(definition.Parent).IsNull();
        }
    }

    [Test]
    public async Task Create_WithNullOptionalProperties_InitializesWithNulls()
    {
        var componentType = typeof(RouteView);

        var definition = new RouteDefinition("/test", componentType, null, null, null, null);

        using (Assert.Multiple())
        {
            _ = await Assert.That(definition.LayoutType).IsNull();
            _ = await Assert.That(definition.ContentType).IsNull();
            _ = await Assert.That(definition.PaginationSettings).IsNull();
            _ = await Assert.That(definition.Parent).IsNull();
        }
    }

    [Test]
    public async Task Create_WithParentRoute_InitializesParentReference()
    {
        var parent = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        var child = new RouteDefinition("/blog/posts", typeof(RouteView), null, null, null, parent);

        _ = await Assert.That(child.Parent).IsEqualTo(parent);
    }

    [Test]
    public async Task Equality_WithIdenticalValues_AreEqual()
    {
        var def1 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        var def2 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);

        _ = await Assert.That(def1).IsEqualTo(def2);
    }

    [Test]
    public async Task Equality_WithDifferentPathPatterns_AreNotEqual()
    {
        var def1 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        var def2 = new RouteDefinition("/posts", typeof(RouteView), null, null, null, null);

        _ = await Assert.That(def1).IsNotEqualTo(def2);
    }

    [Test]
    public async Task Equality_WithDifferentComponentTypes_AreNotEqual()
    {
        var def1 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        var def2 = new RouteDefinition("/blog", typeof(LayoutView), null, null, null, null);

        _ = await Assert.That(def1).IsNotEqualTo(def2);
    }

    [Test]
    public async Task Equality_WithDifferentLayoutTypes_AreNotEqual()
    {
        var def1 = new RouteDefinition("/blog", typeof(RouteView), typeof(LayoutView), null, null, null);
        var def2 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);

        _ = await Assert.That(def1).IsNotEqualTo(def2);
    }

    [Test]
    public async Task Immutability_RecordIsNotMutable()
    {
        var definition = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);

        // Records are immutable by design - verify that modifying creates a new instance
        var modified = definition with
        {
            PathPattern = "/posts",
        };

        _ = await Assert.That(definition.PathPattern).IsEqualTo("/blog");
        _ = await Assert.That(modified.PathPattern).IsEqualTo("/posts");
        _ = await Assert.That(definition).IsNotEqualTo(modified);
    }

    [Test]
    public async Task ToString_ReturnsFormattedString()
    {
        var definition = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);

        var str = definition.ToString();

        _ = await Assert.That(str).IsNotNull();
        _ = await Assert.That(str).Contains("/blog");
    }

    [Test]
    public async Task GetHashCode_WithIdenticalValues_SameHashCode()
    {
        var def1 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        var def2 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);

        _ = await Assert.That(def1.GetHashCode()).IsEqualTo(def2.GetHashCode());
    }

    [Test]
    public async Task GetHashCode_WithDifferentValues_DifferentHashCode()
    {
        var def1 = new RouteDefinition("/blog", typeof(RouteView), null, null, null, null);
        var def2 = new RouteDefinition("/posts", typeof(RouteView), null, null, null, null);

        _ = await Assert.That(def1.GetHashCode()).IsNotEqualTo(def2.GetHashCode());
    }
}
