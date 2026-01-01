namespace NetEvolve.ForgingBlazor.Tests.Unit;

using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Builders;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;
using NetEvolve.ForgingBlazor.Models;

public sealed class ApplicationBuilderExtensionsTests
{
    [Test]
    public void AddDefaultContent_WithNullBuilder_ThrowsArgumentNullException()
    {
        IApplicationBuilder builder = null!;

        _ = Assert.Throws<ArgumentNullException>(() => builder.AddDefaultContent());
    }

    [Test]
    public async Task AddDefaultContent_RegistersDefaultContentServices()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        var result = builder.AddDefaultContent();

        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(builder.Services.Any(x => x.ServiceType == typeof(IContentRegistration))).IsTrue();
    }

    [Test]
    public async Task AddDefaultContent_RegistersForgingBlazorServices()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = builder.AddDefaultContent();

        var hasContentRegister = builder.Services.Any(x => x.ServiceType == typeof(IContentRegister));
        _ = await Assert.That(hasContentRegister).IsTrue();
    }

    [Test]
    public async Task AddDefaultContent_RegistersMarkdownServices()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = builder.AddDefaultContent();

        var hasMarkdownPipeline = builder.Services.Any(x => x.ServiceType == typeof(Markdig.MarkdownPipeline));
        _ = await Assert.That(hasMarkdownPipeline).IsTrue();
    }

    [Test]
    public void AddDefaultContent_CalledTwice_ThrowsInvalidOperationException()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = builder.AddDefaultContent();
        _ = Assert.Throws<InvalidOperationException>(() => builder.AddDefaultContent());
    }

    [Test]
    public void AddDefaultContentGeneric_WithNullBuilder_ThrowsArgumentNullException()
    {
        IApplicationBuilder builder = null!;

        _ = Assert.Throws<ArgumentNullException>(() => builder.AddDefaultContent<TestPage>());
    }

    [Test]
    public async Task AddDefaultContentGeneric_RegistersDefaultContentWithCustomPageType()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        var result = builder.AddDefaultContent<TestPage>();

        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(builder.Services.Any(x => x.ServiceType == typeof(IContentRegistration))).IsTrue();
    }

    [Test]
    public void AddDefaultContentGeneric_CalledTwice_ThrowsInvalidOperationException()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = builder.AddDefaultContent<TestPage>();
        _ = Assert.Throws<InvalidOperationException>(() => builder.AddDefaultContent<TestPage>());
    }

    [Test]
    public void AddSegment_WithNullBuilder_ThrowsArgumentNullException()
    {
        IApplicationBuilder builder = null!;

        _ = Assert.Throws<ArgumentNullException>(() => builder.AddSegment("test"));
    }

    [Test]
    [Arguments(null!)]
    [Arguments("")]
    [Arguments("   ")]
    public void AddSegment_WithNullOrWhiteSpaceSegment_ThrowsArgumentException(string segment)
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        _ = Assert.Throws<ArgumentException>(() => builder.AddSegment(segment));
    }

    [Test]
    public async Task AddSegment_WithValidSegment_ReturnsSegmentBuilder()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var result = builder.AddSegment("blog");

        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task AddSegment_RegistersForgingBlazorServices()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = builder.AddSegment("test");

        var hasContentRegister = builder.Services.Any(x => x.ServiceType == typeof(IContentRegister));
        _ = await Assert.That(hasContentRegister).IsTrue();
    }

    [Test]
    public void AddSegmentGeneric_WithNullBuilder_ThrowsArgumentNullException()
    {
        IApplicationBuilder builder = null!;

        _ = Assert.Throws<ArgumentNullException>(() => builder.AddSegment<TestPage>("test"));
    }

    [Test]
    [Arguments(null!)]
    [Arguments("")]
    [Arguments("   ")]
    public void AddSegmentGeneric_WithNullOrWhiteSpaceSegment_ThrowsArgumentException(string segment)
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        _ = Assert.Throws<ArgumentException>(() => builder.AddSegment<TestPage>(segment));
    }

    [Test]
    public async Task AddSegmentGeneric_WithValidSegment_ReturnsSegmentBuilder()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var result = builder.AddSegment<TestPage>("docs");

        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public void AddBlogSegment_WithNullBuilder_ThrowsArgumentNullException()
    {
        IApplicationBuilder builder = null!;

        _ = Assert.Throws<ArgumentNullException>(() => builder.AddBlogSegment("blog"));
    }

    [Test]
    [Arguments(null!)]
    [Arguments("")]
    [Arguments("   ")]
    public void AddBlogSegment_WithNullOrWhiteSpaceSegment_ThrowsArgumentException(string segment)
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        _ = Assert.Throws<ArgumentException>(() => builder.AddBlogSegment(segment));
    }

    [Test]
    public async Task AddBlogSegment_WithValidSegment_ReturnsBlogSegmentBuilder()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var result = builder.AddBlogSegment("blog");

        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public void AddBlogSegmentGeneric_WithNullBuilder_ThrowsArgumentNullException()
    {
        IApplicationBuilder builder = null!;

        _ = Assert.Throws<ArgumentNullException>(() => builder.AddBlogSegment<TestBlogPost>("blog"));
    }

    [Test]
    [Arguments(null!)]
    [Arguments("")]
    [Arguments("   ")]
    public void AddBlogSegmentGeneric_WithNullOrWhiteSpaceSegment_ThrowsArgumentException(string segment)
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        _ = Assert.Throws<ArgumentException>(() => builder.AddBlogSegment<TestBlogPost>(segment));
    }

    [Test]
    public async Task AddBlogSegmentGeneric_WithValidSegment_ReturnsBlogSegmentBuilder()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var result = builder.AddBlogSegment<TestBlogPost>("posts");

        _ = await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task AddDefaultContent_RegistersContentCollector()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = builder.AddDefaultContent();

        var hasContentCollector = builder.Services.Any(x => x.ServiceType == typeof(IContentCollector));
        _ = await Assert.That(hasContentCollector).IsTrue();
    }

    [Test]
    public async Task AddSegment_MultipleSegments_RegistersAll()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var result1 = builder.AddSegment("docs");
        var result2 = builder.AddSegment("guides");

        _ = await Assert.That(result1).IsNotNull();
        _ = await Assert.That(result2).IsNotNull();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1812",
        Justification = "Used as type parameter in tests"
    )]
    private sealed record TestPage : PageBase;

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1812",
        Justification = "Used as type parameter in tests"
    )]
    private sealed record TestBlogPost : BlogPostBase;
}
