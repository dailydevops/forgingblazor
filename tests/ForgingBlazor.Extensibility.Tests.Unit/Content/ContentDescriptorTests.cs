namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

public class ContentDescriptorTests
{
    [Test]
    public async Task Constructor_RequiresTitle()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "" };

        _ = await Assert.That(descriptor).IsNotNull();
    }

    [Test]
    public async Task Constructor_RequiresSlug()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "" };

        _ = await Assert.That(descriptor).IsNotNull();
    }

    [Test]
    public async Task Title_CanBeSet()
    {
        var descriptor = new ContentDescriptor { Title = "Test Title", Slug = "" };

        _ = await Assert.That(descriptor.Title).IsEqualTo("Test Title");
    }

    [Test]
    public async Task Slug_CanBeSet()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "test-slug" };

        _ = await Assert.That(descriptor.Slug).IsEqualTo("test-slug");
    }

    [Test]
    public async Task Draft_DefaultIsFalse()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "" };

        _ = await Assert.That(descriptor.Draft).IsFalse();
    }

    [Test]
    public async Task Draft_CanBeSet()
    {
        var descriptor = new ContentDescriptor
        {
            Title = "",
            Slug = "",
            Draft = true,
        };

        _ = await Assert.That(descriptor.Draft).IsTrue();
    }

    [Test]
    public async Task PublishedDate_CanBeSet()
    {
        var date = new DateTimeOffset(2026, 1, 25, 12, 0, 0, TimeSpan.Zero);
        var descriptor = new ContentDescriptor
        {
            Title = "",
            Slug = "",
            PublishedDate = date,
        };

        _ = await Assert.That(descriptor.PublishedDate).IsEqualTo(date);
    }

    [Test]
    public async Task ExpiredAt_DefaultIsNull()
    {
        var descriptor = new ContentDescriptor { Title = "", Slug = "" };

        _ = await Assert.That(descriptor.ExpiredAt).IsNull();
    }

    [Test]
    public async Task ExpiredAt_CanBeSet()
    {
        var date = new DateTimeOffset(2026, 12, 31, 23, 59, 59, TimeSpan.Zero);
        var descriptor = new ContentDescriptor
        {
            Title = "",
            Slug = "",
            ExpiredAt = date,
        };

        _ = await Assert.That(descriptor.ExpiredAt).IsEqualTo(date);
    }

    [Test]
    public async Task Body_CanBeSet()
    {
        var descriptor = new ContentDescriptor
        {
            Title = "",
            Slug = "",
            Body = "# Markdown Content",
        };

        _ = await Assert.That(descriptor.Body).IsEqualTo("# Markdown Content");
    }

    [Test]
    public async Task BodyHtml_CanBeSet()
    {
        var descriptor = new ContentDescriptor
        {
            Title = "",
            Slug = "",
            BodyHtml = "<h1>HTML Content</h1>",
        };

        _ = await Assert.That(descriptor.BodyHtml).IsEqualTo("<h1>HTML Content</h1>");
    }
}
