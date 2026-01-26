namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using NetEvolve.ForgingBlazor.Routing;
using NetEvolve.ForgingBlazor.Tests.Unit.Storage;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class RoutingBuilderTests
{
    [Test]
    public async Task ConfigureRoot_WithValidConfiguration_ReturnsRootConfiguration()
    {
        var builder = new RoutingBuilder();

        var result = builder.ConfigureRoot(root => _ = root.WithDefaultContentType<TestContentDescriptor>());

        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(builder.State.Root).IsNotNull();
    }

    [Test]
    public void ConfigureRoot_WhenConfigureIsNull_ThrowsArgumentNullException()
    {
        var builder = new RoutingBuilder();

        _ = Assert.Throws<ArgumentNullException>("configure", () => _ = builder.ConfigureRoot(null!));
    }

    [Test]
    public async Task MapSegment_WithValidNameAndConfiguration_ReturnsSegmentConfiguration()
    {
        var builder = new RoutingBuilder();

        var result = builder.MapSegment("posts", segment => _ = segment.WithContentType<TestContentDescriptor>());

        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(builder.State.Segments).IsNotEmpty();
    }

    [Test]
    public async Task MapSegment_WithNestedSegment_CreatesHierarchy()
    {
        var builder = new RoutingBuilder();

        _ = builder.MapSegment(
            "posts",
            segment =>
            {
                _ = segment.MapSegment(
                    "tutorials",
                    nestedSegment => _ = nestedSegment.WithContentType<TestContentDescriptor>()
                );
            }
        );

        _ = await Assert.That(builder.State.Segments.Count).IsGreaterThan(0);
    }

    [Test]
    public void MapSegment_WhenNameIsNull_ThrowsArgumentException()
    {
        var builder = new RoutingBuilder();

        _ = Assert.ThrowsAsync<ArgumentException>(() =>
        {
            _ = builder.MapSegment(null!, segment => { });
            return Task.CompletedTask;
        });
    }

    [Test]
    public void MapSegment_WhenNameIsWhiteSpace_ThrowsArgumentException()
    {
        var builder = new RoutingBuilder();

        _ = Assert.ThrowsAsync<ArgumentException>(() =>
        {
            _ = builder.MapSegment("   ", segment => { });
            return Task.CompletedTask;
        });
    }

    [Test]
    public void MapSegment_WhenConfigureIsNull_ThrowsArgumentNullException()
    {
        var builder = new RoutingBuilder();

        _ = Assert.ThrowsAsync<ArgumentNullException>(() =>
        {
            _ = builder.MapSegment("posts", null!);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task MapPage_WithValidSlugAndConfiguration_ReturnsPageConfiguration()
    {
        var builder = new RoutingBuilder();

        var result = builder.MapPage("about", page => _ = page.WithContentType<TestContentDescriptor>());

        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(builder.State.Pages).IsNotEmpty();
    }

    [Test]
    public void MapPage_WhenSlugIsNull_ThrowsArgumentException()
    {
        var builder = new RoutingBuilder();

        _ = Assert.ThrowsAsync<ArgumentException>(() =>
        {
            _ = builder.MapPage(null!, page => { });
            return Task.CompletedTask;
        });
    }

    [Test]
    public void MapPage_WhenSlugIsWhiteSpace_ThrowsArgumentException()
    {
        var builder = new RoutingBuilder();

        _ = Assert.ThrowsAsync<ArgumentException>(() =>
        {
            _ = builder.MapPage("   ", page => { });
            return Task.CompletedTask;
        });
    }

    [Test]
    public void MapPage_WhenConfigureIsNull_ThrowsArgumentNullException()
    {
        var builder = new RoutingBuilder();

        _ = Assert.ThrowsAsync<ArgumentNullException>(() =>
        {
            _ = builder.MapPage("about", null!);
            return Task.CompletedTask;
        });
    }

    [Test]
    public async Task MapSegment_WithMultipleSegments_AccumulatesConfiguration()
    {
        var builder = new RoutingBuilder();

        _ = builder.MapSegment("posts", segment => { });
        _ = builder.MapSegment("blog", segment => { });

        _ = await Assert.That(builder.State.Segments.Count).IsGreaterThanOrEqualTo(2);
    }

    [Test]
    public async Task MapPage_WithMultiplePages_AccumulatesConfiguration()
    {
        var builder = new RoutingBuilder();

        _ = builder.MapPage("about", page => { });
        _ = builder.MapPage("contact", page => { });

        _ = await Assert.That(builder.State.Pages.Count).IsEqualTo(2);
    }

    [Test]
    public async Task State_IsAccessibleForTesting()
    {
        var builder = new RoutingBuilder();

        _ = await Assert.That(builder.State).IsNotNull();
    }
}
