namespace NetEvolve.ForgingBlazor.Tests.Unit.Components;

using System;
using System.Collections.Generic;
using Bunit;
using Microsoft.AspNetCore.Components;
using NetEvolve.ForgingBlazor.Components;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="ForgingRouteView"/>.
/// </summary>
public sealed class ForgingRouteViewTests : Bunit.TestContext
{
    [Test]
    public async Task Render_WhenRouteDataIsNull_RendersNotFoundContent()
    {
        // Arrange
        var notFoundMarkup = "<p>Not Found</p>";

        // Act
        var cut = RenderComponent<ForgingRouteView>(parameters =>
            parameters
                .Add(p => p.RouteData, null)
                .Add(p => p.NotFoundContent, builder => builder.AddMarkupContent(0, notFoundMarkup))
        );

        // Assert
        _ = await Assert.That(cut.Markup).Contains(notFoundMarkup);
    }

    [Test]
    public async Task Render_WhenRouteDataIsNull_AndNoNotFoundContent_RendersEmpty()
    {
        // Arrange & Act
        var cut = RenderComponent<ForgingRouteView>(parameters => parameters.Add(p => p.RouteData, null));

        // Assert
        _ = await Assert.That(cut.Markup).IsEmpty();
    }

    [Test]
    public async Task Render_WhenStandardRoute_DelegatesToRouteView()
    {
        // Arrange
        var pageType = typeof(TestComponent);
        var routeData = new RouteData(pageType, new Dictionary<string, object?>());

        // Act
        var cut = RenderComponent<ForgingRouteView>(parameters => parameters.Add(p => p.RouteData, routeData));

        // Assert
        var routeView = cut.FindComponent<RouteView>();
        _ = await Assert.That(routeView).IsNotNull();
        _ = await Assert.That(routeView.Instance.RouteData).IsEqualTo(routeData);
    }

    [Test]
    public async Task Render_WhenContentRoute_RendersDynamicComponentWithResolvedContent()
    {
        // Arrange
        var resolvedContent = new TestResolvedContent();
        var pageType = typeof(TestContentComponent);
        var routeValues = new Dictionary<string, object?> { ["__ResolvedContent"] = resolvedContent };
        var routeData = new RouteData(pageType, routeValues);

        // Act
        var cut = RenderComponent<ForgingRouteView>(parameters => parameters.Add(p => p.RouteData, routeData));

        // Assert
        var dynamicComponent = cut.FindComponent<DynamicComponent>();
        _ = await Assert.That(dynamicComponent).IsNotNull();
        _ = await Assert.That(dynamicComponent.Instance.Type).IsEqualTo(pageType);

        var componentParameters = dynamicComponent.Instance.Parameters as Dictionary<string, object?>;
        _ = await Assert.That(componentParameters).IsNotNull();
        _ = await Assert.That(componentParameters!.ContainsKey("ResolvedContent")).IsTrue();
        _ = await Assert.That(componentParameters["ResolvedContent"]).IsEqualTo(resolvedContent);
    }

    [Test]
    public async Task Render_WhenContentRouteWithAdditionalRouteValues_IncludesThemInParameters()
    {
        // Arrange
        var resolvedContent = new TestResolvedContent();
        var pageType = typeof(TestContentComponent);
        var routeValues = new Dictionary<string, object?>
        {
            ["__ResolvedContent"] = resolvedContent,
            ["id"] = 42,
            ["slug"] = "test-post",
        };
        var routeData = new RouteData(pageType, routeValues);

        // Act
        var cut = RenderComponent<ForgingRouteView>(parameters => parameters.Add(p => p.RouteData, routeData));

        // Assert
        var dynamicComponent = cut.FindComponent<DynamicComponent>();
        var componentParameters = dynamicComponent.Instance.Parameters as Dictionary<string, object?>;
        _ = await Assert.That(componentParameters).IsNotNull();
        _ = await Assert.That(componentParameters!.ContainsKey("ResolvedContent")).IsTrue();
        _ = await Assert.That(componentParameters.ContainsKey("id")).IsTrue();
        _ = await Assert.That(componentParameters["id"]).IsEqualTo(42);
        _ = await Assert.That(componentParameters.ContainsKey("slug")).IsTrue();
        _ = await Assert.That(componentParameters["slug"]).IsEqualTo("test-post");
    }

    [Test]
    public async Task OnParametersSet_WhenRouteDataChangesToNull_ClearsInternalState()
    {
        // Arrange
        var pageType = typeof(TestComponent);
        var routeData = new RouteData(pageType, new Dictionary<string, object?>());

        var cut = RenderComponent<ForgingRouteView>(parameters => parameters.Add(p => p.RouteData, routeData));

        // Act - Change route data to null
        cut.SetParametersAndRender(parameters => parameters.Add(p => p.RouteData, null));

        // Assert - Should render empty since no NotFoundContent was provided
        _ = await Assert.That(cut.Markup).IsEmpty();
    }

    [Test]
    public async Task Render_WhenDefaultLayoutProvided_PassesToRouteView()
    {
        // Arrange
        var pageType = typeof(TestComponent);
        var routeData = new RouteData(pageType, new Dictionary<string, object?>());
        var defaultLayout = typeof(TestLayout);

        // Act
        var cut = RenderComponent<ForgingRouteView>(parameters =>
            parameters.Add(p => p.RouteData, routeData).Add(p => p.DefaultLayout, defaultLayout)
        );

        // Assert
        var routeView = cut.FindComponent<RouteView>();
        _ = await Assert.That(routeView).IsNotNull();
        _ = await Assert.That(routeView.Instance.DefaultLayout).IsEqualTo(defaultLayout);
    }

    private sealed class TestComponent : ComponentBase { }

    private sealed class TestContentComponent : ComponentBase
    {
        [Parameter]
        public TestResolvedContent? ResolvedContent { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public string? Slug { get; set; }
    }

    private sealed partial class TestResolvedContent { }

    private sealed class TestLayout : LayoutComponentBase { }
}
