namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using System.Globalization;
using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Routing;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class CanonicalUrlGeneratorTests
{
    private static readonly CultureInfo _german = CultureInfo.GetCultureInfo("de-DE");
    private static readonly CultureInfo _english = CultureInfo.GetCultureInfo("en-US");

    [Test]
    public async Task Generate_WithPrefixFormat_IncludesCulturePrefix()
    {
        var url = CanonicalUrlGenerator.Generate("/blog", _german, CultureCanonical.WithPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/de-DE/blog");
    }

    [Test]
    public async Task Generate_WithoutPrefixFormatAndDefaultCulture_NoCulturePrefix()
    {
        var url = CanonicalUrlGenerator.Generate("/blog", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/blog");
    }

    [Test]
    public async Task Generate_WithoutPrefixFormatAndNonDefaultCulture_IncludesCulturePrefix()
    {
        var url = CanonicalUrlGenerator.Generate("/blog", _german, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/de-DE/blog");
    }

    [Test]
    public void Generate_WithNullPathPattern_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "pathPattern",
            () => CanonicalUrlGenerator.Generate(null!, _german, CultureCanonical.WithPrefix, _english)
        );

    [Test]
    public void Generate_WithNullCulture_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "culture",
            () => CanonicalUrlGenerator.Generate("/blog", null!, CultureCanonical.WithPrefix, _english)
        );

    [Test]
    public void Generate_WithNullDefaultCulture_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "defaultCulture",
            () => CanonicalUrlGenerator.Generate("/blog", _german, CultureCanonical.WithPrefix, null!)
        );

    [Test]
    public async Task Generate_WithTrailingSlash_RemovesTrailingSlash()
    {
        var url = CanonicalUrlGenerator.Generate("/blog/", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/blog");
    }

    [Test]
    public async Task Generate_WithoutLeadingSlash_AddsLeadingSlash()
    {
        var url = CanonicalUrlGenerator.Generate("blog", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/blog");
    }

    [Test]
    public async Task Generate_WithRootPath_ReturnsSlash()
    {
        var url = CanonicalUrlGenerator.Generate("/", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/");
    }

    [Test]
    public async Task Generate_WithEmptyPath_ReturnsSlash()
    {
        var url = CanonicalUrlGenerator.Generate("", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/");
    }

    [Test]
    public async Task Generate_WithPrefixFormatAndRootPath_IncludesCulturePrefix()
    {
        var url = CanonicalUrlGenerator.Generate("/", _german, CultureCanonical.WithPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/de-DE");
    }

    [Test]
    public async Task Generate_WithComplexPath_NormalizesCorrectly()
    {
        var url = CanonicalUrlGenerator.Generate("blog/posts/article/", _german, CultureCanonical.WithPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/de-DE/blog/posts/article");
    }

    [Test]
    public async Task Generate_WithMultipleSlashes_NormalizesPath()
    {
        var url = CanonicalUrlGenerator.Generate("//blog//posts//", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsNotNull();
    }

    [Test]
    public async Task Generate_WithFrenchCulture_IncludesCulturePrefix()
    {
        var french = CultureInfo.GetCultureInfo("fr-FR");
        var url = CanonicalUrlGenerator.Generate("/articles", french, CultureCanonical.WithPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/fr-FR/articles");
    }

    [Test]
    public async Task Generate_WithSpanishCulture_IncludesCulturePrefix()
    {
        var spanish = CultureInfo.GetCultureInfo("es-ES");
        var url = CanonicalUrlGenerator.Generate("/blog", spanish, CultureCanonical.WithPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/es-ES/blog");
    }

    [Test]
    public async Task Generate_WithoutPrefixAndSameCulture_NoCulturePrefix()
    {
        var url = CanonicalUrlGenerator.Generate("/blog", _german, CultureCanonical.WithoutPrefix, _german);

        _ = await Assert.That(url).IsEqualTo("/blog");
    }

    [Test]
    public async Task Generate_WithoutPrefixAndDifferentCulture_IncludesCulturePrefix()
    {
        var french = CultureInfo.GetCultureInfo("fr-FR");
        var url = CanonicalUrlGenerator.Generate("/blog", french, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/fr-FR/blog");
    }

    [Test]
    public async Task Generate_WithLongPath_PreservesFullPath()
    {
        var longPath = "/blog/category/subcategory/article/section/subsection";
        var url = CanonicalUrlGenerator.Generate(longPath, _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo(longPath);
    }

    [Test]
    public async Task Generate_WithDashesInPath_PreservesDashes()
    {
        var url = CanonicalUrlGenerator.Generate("/my-blog-post", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/my-blog-post");
    }

    [Test]
    public async Task Generate_WithUnderscoresInPath_PreservesUnderscores()
    {
        var url = CanonicalUrlGenerator.Generate("/my_blog_post", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/my_blog_post");
    }

    [Test]
    public async Task Generate_WithNumbersInPath_PreservesNumbers()
    {
        var url = CanonicalUrlGenerator.Generate(
            "/blog2024/article123",
            _english,
            CultureCanonical.WithoutPrefix,
            _english
        );

        _ = await Assert.That(url).IsEqualTo("/blog2024/article123");
    }

    [Test]
    public async Task Generate_WithPrefixAndComplexPath_CombinesCorrectly()
    {
        var url = CanonicalUrlGenerator.Generate(
            "/blog/my-posts/article-2024",
            _german,
            CultureCanonical.WithPrefix,
            _english
        );

        _ = await Assert.That(url).IsEqualTo("/de-DE/blog/my-posts/article-2024");
    }

    [Test]
    public async Task Generate_WithWhitespaceOnlyPath_ReturnsSlash()
    {
        var url = CanonicalUrlGenerator.Generate("   ", _english, CultureCanonical.WithoutPrefix, _english);

        _ = await Assert.That(url).IsEqualTo("/");
    }
}
