namespace NetEvolve.ForgingBlazor.Tests.Unit.Content;

using System.Globalization;
using global::NetEvolve.ForgingBlazor.Content;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class ContentLookupPathTests
{
    [Test]
    public async Task Constructor_SetsProperties()
    {
        var basePath = "posts/my-article";
        var extension = ".md";
        var culture = CultureInfo.GetCultureInfo("en-US");

        var lookupPath = new ContentLookupPath(basePath, extension, culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(lookupPath.BasePath).IsEqualTo(basePath);
            _ = await Assert.That(lookupPath.Extension).IsEqualTo(extension);
            _ = await Assert.That(lookupPath.Culture).IsEqualTo(culture);
        }
    }

    [Test]
    public async Task GeneratePath_WithCultureSuffix_ReturnsCombinedPath()
    {
        var lookupPath = new ContentLookupPath("posts/my-article", ".md", CultureInfo.GetCultureInfo("en-US"));

        var result = lookupPath.GeneratePath(".de-DE");

        _ = await Assert.That(result).IsEqualTo("posts/my-article.de-DE.md");
    }

    [Test]
    public async Task GeneratePath_WithEmptySuffix_ReturnsBasePath()
    {
        var lookupPath = new ContentLookupPath("posts/my-article", ".md", CultureInfo.GetCultureInfo("en-US"));

        var result = lookupPath.GeneratePath(string.Empty);

        _ = await Assert.That(result).IsEqualTo("posts/my-article.md");
    }

    [Test]
    public async Task GeneratePath_WithNullSuffix_ThrowsArgumentNullException()
    {
        var lookupPath = new ContentLookupPath("posts/my-article", ".md", CultureInfo.GetCultureInfo("en-US"));

        var exception = Assert.Throws<ArgumentNullException>(() => lookupPath.GeneratePath(null!));

        _ = await Assert.That(exception.ParamName).IsEqualTo("cultureSuffix");
    }

    [Test]
    public async Task GeneratePath_WithMultipleSuffixes_ReturnsCorrectPaths()
    {
        var lookupPath = new ContentLookupPath("blog/post", ".md", CultureInfo.GetCultureInfo("en-US"));

        using (Assert.Multiple())
        {
            _ = await Assert.That(lookupPath.GeneratePath(".en-US")).IsEqualTo("blog/post.en-US.md");
            _ = await Assert.That(lookupPath.GeneratePath(".en")).IsEqualTo("blog/post.en.md");
            _ = await Assert.That(lookupPath.GeneratePath(string.Empty)).IsEqualTo("blog/post.md");
        }
    }

    [Test]
    public async Task Init_Properties_CanBeOverridden()
    {
        var original = new ContentLookupPath("posts/article", ".md", CultureInfo.GetCultureInfo("en-US"));

        var modified = original with { BasePath = "blog/post" };

        using (Assert.Multiple())
        {
            _ = await Assert.That(modified.BasePath).IsEqualTo("blog/post");
            _ = await Assert.That(modified.Extension).IsEqualTo(".md");
            _ = await Assert.That(modified.Culture).IsEqualTo(original.Culture);
        }
    }
}
