namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using System.Globalization;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Routing;
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
}
