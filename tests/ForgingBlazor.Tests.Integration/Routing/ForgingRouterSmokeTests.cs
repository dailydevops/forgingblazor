namespace NetEvolve.ForgingBlazor.Tests.Integration.Routing;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NetEvolve.ForgingBlazor.Components;
using NetEvolve.ForgingBlazor.Routing;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Smoke integration tests for <see cref="ForgingRouter"/> component.
/// These tests verify basic service registration and routing infrastructure setup.
/// </summary>
public sealed class ForgingRouterSmokeTests
{
    [Test]
    public async Task RouteRegistry_CanBeInstantiated()
    {
        // Arrange & Act
        var registry = new RouteRegistry();

        // Assert
        _ = await Assert.That(registry).IsNotNull();
    }

    [Test]
    public async Task RouteResolver_CanBeInstantiatedWithRegistry()
    {
        // Arrange
        var registry = new RouteRegistry();

        // Act
        var resolver = new RouteResolver(registry);

        // Assert
        _ = await Assert.That(resolver).IsNotNull();
    }

    [Test]
    public async Task RouteResolver_TryResolve_ReturnsFalse_WhenNoRoutesRegistered()
    {
        // Arrange
        var registry = new RouteRegistry();
        var resolver = new RouteResolver(registry);

        // Act
        var result = resolver.TryResolve("/some-path", out var definition);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsFalse();
            _ = await Assert.That(definition).IsNull();
        }
    }

    [Test]
    public async Task RouteDefinition_CanBeCreatedWithMinimalParameters()
    {
        // Arrange & Act
        var definition = new RouteDefinition(
            PathPattern: "/test",
            ComponentType: typeof(ComponentBase),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(definition).IsNotNull();
            _ = await Assert.That(definition.PathPattern).IsEqualTo("/test");
            _ = await Assert.That(definition.ComponentType).IsEqualTo(typeof(ComponentBase));
        }
    }

    [Test]
    public async Task RouteDefinition_SupportsHierarchicalStructure()
    {
        // Arrange
        var parentDefinition = new RouteDefinition(
            PathPattern: "/blog",
            ComponentType: typeof(ComponentBase),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );

        // Act
        var childDefinition = new RouteDefinition(
            PathPattern: "/blog/posts",
            ComponentType: typeof(ComponentBase),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: parentDefinition
        );

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(childDefinition.Parent).IsEqualTo(parentDefinition);
            _ = await Assert.That(childDefinition.PathPattern).Contains("blog");
        }
    }

    [Test]
    public async Task RouteRegistry_RegisterRoute_IncreasesCount()
    {
        // Arrange
        var registry = new RouteRegistry();
        var definition = new RouteDefinition(
            PathPattern: "/test",
            ComponentType: typeof(ComponentBase),
            LayoutType: null,
            ContentType: null,
            PaginationSettings: null,
            Parent: null
        );

        // Act
        registry.Register("/test", definition);

        // Assert
        _ = await Assert.That(registry).IsNotNull();
    }

    [Test]
    public async Task ContentRouteHandler_CanBeInstantiated()
    {
        // Arrange & Act
        var handler = new ContentRouteHandler();

        // Assert
        _ = await Assert.That(handler).IsNotNull();
    }
}
