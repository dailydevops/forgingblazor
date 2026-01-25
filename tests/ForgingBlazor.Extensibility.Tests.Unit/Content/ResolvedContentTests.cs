namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

using System.Globalization;

public class ResolvedContentTests
{
    [Test]
    public async Task Constructor_CreatesInstance()
    {
        var descriptor = new ContentDescriptor { Title = "Test", Slug = "test" };
        var culture = CultureInfo.InvariantCulture;
        var canonicalUrl = "https://example.com/test";
        var routeValues = new Dictionary<string, object?> { { "slug", "test" } };

        var resolved = new ResolvedContent<ContentDescriptor>
        {
            Descriptor = descriptor,
            Culture = culture,
            CanonicalUrl = canonicalUrl,
            RouteValues = routeValues,
        };

        _ = await Assert.That(resolved).IsNotNull();
        _ = await Assert.That(resolved.Descriptor).IsEqualTo(descriptor);
        _ = await Assert.That(resolved.Culture).IsEqualTo(culture);
        _ = await Assert.That(resolved.CanonicalUrl).IsEqualTo(canonicalUrl);
        _ = await Assert.That(resolved.RouteValues).IsEqualTo(routeValues);
    }

    [Test]
    public async Task Descriptor_Required()
    {
        _ = await Assert
            .That(
                typeof(ResolvedContent<ContentDescriptor>)
                    .GetProperty("Descriptor")!
                    .GetCustomAttributes(false)
                    .Any(a => a.GetType().Name == "RequiredMemberAttribute")
            )
            .IsTrue();
    }

    [Test]
    public async Task Culture_Required()
    {
        _ = await Assert
            .That(
                typeof(ResolvedContent<ContentDescriptor>)
                    .GetProperty("Culture")!
                    .GetCustomAttributes(false)
                    .Any(a => a.GetType().Name == "RequiredMemberAttribute")
            )
            .IsTrue();
    }

    [Test]
    public async Task CanonicalUrl_Required()
    {
        _ = await Assert
            .That(
                typeof(ResolvedContent<ContentDescriptor>)
                    .GetProperty("CanonicalUrl")!
                    .GetCustomAttributes(false)
                    .Any(a => a.GetType().Name == "RequiredMemberAttribute")
            )
            .IsTrue();
    }

    [Test]
    public async Task RouteValues_Required()
    {
        _ = await Assert
            .That(
                typeof(ResolvedContent<ContentDescriptor>)
                    .GetProperty("RouteValues")!
                    .GetCustomAttributes(false)
                    .Any(a => a.GetType().Name == "RequiredMemberAttribute")
            )
            .IsTrue();
    }

    [Test]
    public async Task Descriptor_ReturnsSetValue()
    {
        var descriptor = new ContentDescriptor { Title = "Title", Slug = "title" };
        var culture = CultureInfo.InvariantCulture;
        var routeValues = new Dictionary<string, object?>();

        var resolved = new ResolvedContent<ContentDescriptor>
        {
            Descriptor = descriptor,
            Culture = culture,
            CanonicalUrl = "https://example.com",
            RouteValues = routeValues,
        };

        _ = await Assert.That(resolved.Descriptor.Title).IsEqualTo("Title");
    }

    [Test]
    public async Task Culture_ReturnsSetValue()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "" };
        var culture = new CultureInfo("de-DE");
        var routeValues = new Dictionary<string, object?>();

        var resolved = new ResolvedContent<ContentDescriptor>
        {
            Descriptor = descriptor,
            Culture = culture,
            CanonicalUrl = "https://example.com",
            RouteValues = routeValues,
        };

        _ = await Assert.That(resolved.Culture.Name).IsEqualTo("de-DE");
    }

    [Test]
    public async Task CanonicalUrl_ReturnsSetValue()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "" };
        var culture = CultureInfo.InvariantCulture;
        var url = "https://example.com/blog/test-post";
        var routeValues = new Dictionary<string, object?>();

        var resolved = new ResolvedContent<ContentDescriptor>
        {
            Descriptor = descriptor,
            Culture = culture,
            CanonicalUrl = url,
            RouteValues = routeValues,
        };

        _ = await Assert.That(resolved.CanonicalUrl).IsEqualTo(url);
    }

    [Test]
    public async Task RouteValues_ReturnsReadOnlyCollection()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "" };
        var culture = CultureInfo.InvariantCulture;
        var routeValues = new Dictionary<string, object?> { { "page", 1 } };

        var resolved = new ResolvedContent<ContentDescriptor>
        {
            Descriptor = descriptor,
            Culture = culture,
            CanonicalUrl = "https://example.com",
            RouteValues = routeValues,
        };

        _ = await Assert.That(resolved.RouteValues.Keys).Contains("page");
        _ = await Assert.That(resolved.RouteValues["page"]).IsEqualTo(1);
    }
}
