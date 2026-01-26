namespace NetEvolve.ForgingBlazor.Tests.Unit.Content;

using System.Globalization;
using NetEvolve.ForgingBlazor.Content;
using NetEvolve.ForgingBlazor.Routing.Culture;
using NSubstitute;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class CultureContentLocatorTests
{
    [Test]
    public async Task Constructor_WithValidParameters_CreatesInstance()
    {
        var fallbackChain = new CultureFallbackChain();
        var storageProvider = Substitute.For<IContentStorageProvider>();

        var locator = new CultureContentLocator(fallbackChain, storageProvider);

        _ = await Assert.That(locator).IsNotNull();
    }

    [Test]
    public void Constructor_WithNullFallbackChain_ThrowsArgumentNullException()
    {
        var storageProvider = Substitute.For<IContentStorageProvider>();

        _ = Assert.Throws<ArgumentNullException>(
            "fallbackChain",
            () => _ = new CultureContentLocator(null!, storageProvider)
        );
    }

    [Test]
    public void Constructor_WithNullStorageProvider_ThrowsArgumentNullException()
    {
        var fallbackChain = new CultureFallbackChain();

        _ = Assert.Throws<ArgumentNullException>(
            "storageProvider",
            () => _ = new CultureContentLocator(fallbackChain, null!)
        );
    }

    [Test]
    public async Task GetLookupPaths_WithValidLookupPath_ReturnsOrderedPaths()
    {
        var fallbackChain = new CultureFallbackChain();
        var storageProvider = Substitute.For<IContentStorageProvider>();
        var locator = new CultureContentLocator(fallbackChain, storageProvider);
        var culture = CultureInfo.GetCultureInfo("de-DE");
        var lookupPath = new ContentLookupPath("posts/article", ".md", culture);

        var result = locator.GetLookupPaths(lookupPath);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Count).IsGreaterThan(0);
            _ = await Assert.That(result[0]).IsEqualTo("posts/article.de-DE.md");
            _ = await Assert.That(result[^1]).IsEqualTo("posts/article.md");
        }
    }

    [Test]
    public async Task GetLookupPaths_WithNullLookupPath_ThrowsArgumentNullException()
    {
        var fallbackChain = new CultureFallbackChain();
        var storageProvider = Substitute.For<IContentStorageProvider>();
        var locator = new CultureContentLocator(fallbackChain, storageProvider);

        _ = Assert.Throws<ArgumentNullException>("lookupPath", () => locator.GetLookupPaths(null!));
    }

    [Test]
    public async Task GetLookupPaths_WithDifferentCultures_ReturnsDifferentPaths()
    {
        var fallbackChain = new CultureFallbackChain();
        var storageProvider = Substitute.For<IContentStorageProvider>();
        var locator = new CultureContentLocator(fallbackChain, storageProvider);

        var deLookup = new ContentLookupPath("posts/article", ".md", CultureInfo.GetCultureInfo("de-DE"));
        var frLookup = new ContentLookupPath("posts/article", ".md", CultureInfo.GetCultureInfo("fr-FR"));

        var deResult = locator.GetLookupPaths(deLookup);
        var frResult = locator.GetLookupPaths(frLookup);

        using (Assert.Multiple())
        {
            _ = await Assert.That(deResult[0]).IsEqualTo("posts/article.de-DE.md");
            _ = await Assert.That(frResult[0]).IsEqualTo("posts/article.fr-FR.md");
        }
    }
}
