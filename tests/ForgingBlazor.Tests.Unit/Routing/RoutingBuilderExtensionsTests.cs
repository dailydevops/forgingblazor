namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Routing;
using NetEvolve.ForgingBlazor.Routing.Constraints;

public class RoutingBuilderExtensionsTests
{
    [Test]
    public async Task AddRouting_RegistersRoutingConfiguration()
    {
        // Arrange
        var builder = new ForgingBlazorApplicationBuilder([]);
        IForgingBlazorApplicationBuilder interfaceBuilder = builder;

        _ = interfaceBuilder.AddRouting(routing =>
        {
            _ = routing.ConfigureRoot(root =>
            {
                _ = new TestContentDescriptor();
                _ = root.WithDefaultContentType<TestContentDescriptor>()
                    .WithDefaultComponent<TestIndexComponent>()
                    .WithDefaultLayout<TestLayout>()
                    .WithHomePage<TestContentDescriptor>();
            });

            _ = routing.MapSegment(
                "posts",
                segment =>
                    segment
                        .WithContentType<TestContentDescriptor>()
                        .WithIndexComponent<TestIndexComponent>()
                        .WithPageComponent<TestPageComponent>()
                        .WithPagination(pagination =>
                            pagination.PageSize(5).UrlFormat(PaginationUrlFormat.Prefixed, "page-")
                        )
                        .WithMetadata(metadata => metadata.ExtendWith<string>("author"))
            );

            _ = routing.MapPage(
                "about",
                page =>
                    page.WithContentType<TestContentDescriptor>()
                        .WithComponent<TestPageComponent>()
                        .WithLayout<TestLayout>()
                        .WithMetadata(metadata => metadata.ExtendWith<bool>("featured", true))
            );
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        // Act
        var configuration = serviceProvider.GetRequiredService<RoutingConfiguration>();

        // Assert
        _ = await Assert.That(configuration.Root.DefaultContentType).IsSameReferenceAs(typeof(TestContentDescriptor));
        _ = await Assert.That(configuration.Root.DefaultComponentType).IsSameReferenceAs(typeof(TestIndexComponent));
        _ = await Assert.That(configuration.Root.DefaultLayoutType).IsSameReferenceAs(typeof(TestLayout));
        _ = await Assert.That(configuration.Root.HomePageContentType).IsSameReferenceAs(typeof(TestContentDescriptor));

        _ = await Assert.That(configuration.Segments.ContainsKey("posts")).IsTrue();
        var segment = configuration.Segments["posts"];
        _ = await Assert.That(segment.Pagination).IsNotNull();
        _ = await Assert.That(segment.Pagination!.PageSize).EqualTo(5);
        _ = await Assert.That(segment.Pagination!.Format).EqualTo(PaginationUrlFormat.Prefixed);
        _ = await Assert.That(segment.Pagination!.Prefix).EqualTo("page-");
        _ = await Assert.That(segment.Metadata.Fields.ContainsKey("author")).IsTrue();

        _ = await Assert.That(configuration.Pages.ContainsKey("about")).IsTrue();
        var page = configuration.Pages["about"];
        _ = await Assert.That(page.ComponentType).IsSameReferenceAs(typeof(TestPageComponent));
        _ = await Assert.That(page.LayoutType).IsSameReferenceAs(typeof(TestLayout));
        _ = await Assert.That(page.Metadata.Fields.ContainsKey("featured")).IsTrue();
    }

    [Test]
    public async Task AddRouting_ThrowsWhenCalledMultipleTimes()
    {
        // Arrange
        var builder = new ForgingBlazorApplicationBuilder([]);
        IForgingBlazorApplicationBuilder interfaceBuilder = builder;
        _ = interfaceBuilder.AddRouting(_ => { });

        // Act & Assert
        _ = await Assert.That(() => interfaceBuilder.AddRouting(_ => { })).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task MapSegment_AllowsNestedSegments()
    {
        // Arrange
        var builder = new ForgingBlazorApplicationBuilder([]);
        IForgingBlazorApplicationBuilder interfaceBuilder = builder;
        _ = interfaceBuilder.AddRouting(routing =>
        {
            _ = routing.MapSegment(
                "blog",
                segment => segment.MapSegment("tutorials", nested => nested.WithContentType<TestContentDescriptor>())
            );
        });

        var serviceProvider = builder.Services.BuildServiceProvider();

        // Act
        var configuration = serviceProvider.GetRequiredService<RoutingConfiguration>();

        // Assert
        _ = await Assert.That(configuration.Segments.ContainsKey("blog")).IsTrue();
        _ = await Assert.That(configuration.Segments.ContainsKey("blog/tutorials")).IsTrue();
    }

    [Test]
    public async Task MapPage_InvalidSlug_ThrowsArgumentException()
    {
        // Arrange
        var builder = new ForgingBlazorApplicationBuilder([]);
        IForgingBlazorApplicationBuilder interfaceBuilder = builder;

        // Act & Assert
        _ = await Assert
            .That(() =>
                interfaceBuilder.AddRouting(routing =>
                {
                    _ = routing.MapPage("invalid_slug", _ => { });
                })
            )
            .Throws<ArgumentException>();
    }

    [Test]
    [MethodDataSource(nameof(SlugConstraint_Theory_Expected_Data))]
    public async Task SlugRouteConstraint_Theory_Expected(string? slug, bool expected)
    {
        // Arrange
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = slug! };

        // Act
        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        // Assert
        _ = await Assert.That(result).EqualTo(expected);
    }

    public static IEnumerable<(string?, bool)> SlugConstraint_Theory_Expected_Data =>
        [
            (null, false),
            ("", false),
            ("abc", true),
            ("my-article", true),
            ("one--double", false),
            ("invalid_slug", false),
        ];

    private sealed class TestContentDescriptor : ContentDescriptor
    {
        [SetsRequiredMembers]
        public TestContentDescriptor()
        {
            Title = "Test";
            Slug = "test";
            PublishedDate = DateTimeOffset.UtcNow;
            Body = string.Empty;
            BodyHtml = string.Empty;
        }
    }

    private sealed class TestIndexComponent : ComponentBase { }

    private sealed class TestPageComponent : ComponentBase { }

    private sealed class TestLayout : LayoutComponentBase { }
}
