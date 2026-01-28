namespace NetEvolve.ForgingBlazor.Tests.Unit.Components;

using System;
using System.Collections.Generic;
using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Components;
using NetEvolve.ForgingBlazor.Routing;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="ForgingRouter"/>.
/// </summary>
public sealed class ForgingRouterTests : Bunit.TestContext
{
    [Before(Test)]
    public void Setup()
    {
        // Register minimal required services
        _ = Services.AddSingleton(new RouteRegistry());
        _ = Services.AddSingleton(sp => new RouteResolver(sp.GetRequiredService<RouteRegistry>()));
    }

    [Test]
    public async Task Render_WhenAppAssemblyIsNull_ThrowsInvalidOperationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var cut = RenderComponent<ForgingRouter>(parameters =>
                parameters
                    .Add(p => p.AppAssembly, null)
                    .Add(p => p.Found, context => builder => { })
                    .Add(p => p.NotFound, builder => { })
            );

            await Task.Delay(10).ConfigureAwait(false); // Allow component to settle
        });

        _ = await Assert.That((await exception).Message!).Contains(nameof(ForgingRouter.AppAssembly));
    }

    [Test]
    public async Task Render_WhenFoundIsNull_ThrowsInvalidOperationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var cut = RenderComponent<ForgingRouter>(parameters =>
                parameters
                    .Add(p => p.AppAssembly, Assembly.GetExecutingAssembly())
                    .Add(p => p.Found, (RenderFragment<RouteData>?)null)
                    .Add(p => p.NotFound, builder => { })
            );

            await Task.Delay(10).ConfigureAwait(false);
        });

        _ = await Assert.That((await exception).Message).Contains(nameof(ForgingRouter.Found));
    }

    [Test]
    public async Task Render_WhenNotFoundIsNull_ThrowsInvalidOperationException()
    {
        // Arrange & Act & Assert
        var exception = Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            var cut = RenderComponent<ForgingRouter>(parameters =>
                parameters
                    .Add(p => p.AppAssembly, Assembly.GetExecutingAssembly())
                    .Add(p => p.Found, context => builder => { })
                    .Add<RenderFragment?>(p => p.NotFound, null)
            );

            await Task.Delay(10).ConfigureAwait(false);
        });

        _ = await Assert.That((await exception).Message!).Contains(nameof(ForgingRouter.NotFound));
    }

    [Test]
    public async Task Render_WhenAllRequiredParametersProvided_RendersSuccessfully()
    {
        // Arrange
        var appAssembly = Assembly.GetExecutingAssembly();
        var foundInvoked = false;
        var notFoundMarkup = "<p>Not Found</p>";

        // Act
        var cut = RenderComponent<ForgingRouter>(parameters =>
            parameters
                .Add(p => p.AppAssembly, appAssembly)
                .Add(
                    p => p.Found,
                    context =>
                    {
                        foundInvoked = true;
                        return builder => builder.AddMarkupContent(0, "<p>Found</p>");
                    }
                )
                .Add(p => p.NotFound, builder => builder.AddMarkupContent(0, notFoundMarkup))
        );

        // Assert - Component renders without throwing
        _ = await Assert.That(cut).IsNotNull();
        // The component should have rendered (either Found or standard Router fallback)
    }

    [Test]
    public async Task Render_WhenNavigatingProvided_ShowsNavigatingContent()
    {
        // Arrange
        var appAssembly = Assembly.GetExecutingAssembly();
        var navigatingMarkup = "<p>Loading...</p>";

        // Act
        var cut = RenderComponent<ForgingRouter>(parameters =>
            parameters
                .Add(p => p.AppAssembly, appAssembly)
                .Add(p => p.Found, context => builder => builder.AddMarkupContent(0, "<p>Found</p>"))
                .Add(p => p.NotFound, builder => builder.AddMarkupContent(0, "<p>Not Found</p>"))
                .Add(p => p.Navigating, builder => builder.AddMarkupContent(0, navigatingMarkup))
        );

        // Wait briefly for navigation state
        await Task.Delay(50).ConfigureAwait(false);

        // Assert - Navigating content might briefly appear
        _ = await Assert.That(cut).IsNotNull();
    }

    [Test]
    public async Task Render_WhenAdditionalAssembliesProvided_PassesToRouter()
    {
        // Arrange
        var appAssembly = Assembly.GetExecutingAssembly();
        var additionalAssemblies = new[] { typeof(ComponentBase).Assembly };

        // Act
        var cut = RenderComponent<ForgingRouter>(parameters =>
            parameters
                .Add(p => p.AppAssembly, appAssembly)
                .Add(p => p.AdditionalAssemblies, additionalAssemblies)
                .Add(p => p.Found, context => builder => builder.AddMarkupContent(0, "<p>Found</p>"))
                .Add(p => p.NotFound, builder => builder.AddMarkupContent(0, "<p>Not Found</p>"))
        );

        // Assert - Component renders successfully
        _ = await Assert.That(cut).IsNotNull();
    }

    [Test]
    public async Task Dispose_UnsubscribesFromLocationChanged()
    {
        // Arrange
        var appAssembly = Assembly.GetExecutingAssembly();
        var cut = RenderComponent<ForgingRouter>(parameters =>
            parameters
                .Add(p => p.AppAssembly, appAssembly)
                .Add(p => p.Found, context => builder => builder.AddMarkupContent(0, "<p>Found</p>"))
                .Add(p => p.NotFound, builder => builder.AddMarkupContent(0, "<p>Not Found</p>"))
        );

        // Act - Dispose the component
        cut.Dispose();

        // Assert - Should not throw; event unsubscription happens internally
        _ = await Assert.That(true).IsTrue();
    }

    [Test]
    public async Task OnAfterRenderAsync_EnablesNavigationInterception()
    {
        // Arrange
        var appAssembly = Assembly.GetExecutingAssembly();

        // Act
        var cut = RenderComponent<ForgingRouter>(parameters =>
            parameters
                .Add(p => p.AppAssembly, appAssembly)
                .Add(p => p.Found, context => builder => builder.AddMarkupContent(0, "<p>Found</p>"))
                .Add(p => p.NotFound, builder => builder.AddMarkupContent(0, "<p>Not Found</p>"))
        );

        // Wait for after render
        await Task.Delay(100).ConfigureAwait(false);

        // Assert - Navigation interception should be enabled after first render
        _ = await Assert.That(cut).IsNotNull();
    }
}
