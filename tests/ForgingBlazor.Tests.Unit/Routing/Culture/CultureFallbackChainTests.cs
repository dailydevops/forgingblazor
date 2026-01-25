namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing.Culture;

using System.Globalization;
using global::NetEvolve.ForgingBlazor.Routing.Culture;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class CultureFallbackChainTests
{
    [Test]
    public async Task Constructor_WithNullDefaultCulture_UsesEnglishUS()
    {
        var chain = new CultureFallbackChain(null);
        var culture = CultureInfo.GetCultureInfo("de-DE");

        var result = chain.GetFallbackChain(culture);

        _ = await Assert.That(result.Any(c => c?.Name == "en-US")).IsTrue();
    }

    [Test]
    public async Task Constructor_WithCustomDefaultCulture_UsesCustomCulture()
    {
        var defaultCulture = CultureInfo.GetCultureInfo("fr-FR");
        var chain = new CultureFallbackChain(defaultCulture);
        var culture = CultureInfo.GetCultureInfo("de-DE");

        var result = chain.GetFallbackChain(culture);

        _ = await Assert.That(result.Any(c => c?.Name == "fr-FR")).IsTrue();
    }

    [Test]
    public async Task GetFallbackChain_WithSpecificCulture_ReturnsCorrectOrder()
    {
        var chain = new CultureFallbackChain();
        var culture = CultureInfo.GetCultureInfo("de-DE");

        var result = chain.GetFallbackChain(culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Count).IsGreaterThanOrEqualTo(4);
            _ = await Assert.That(result[0]?.Name).IsEqualTo("de-DE");
            _ = await Assert.That(result[1]?.Name).IsEqualTo("de");
            _ = await Assert.That(result.Any(c => c?.Name == "en-US")).IsTrue();
            _ = await Assert.That(result.Any(c => c?.Name == "en")).IsTrue();
            _ = await Assert.That(result[^1]).IsNull(); // Last should be invariant
        }
    }

    [Test]
    public async Task GetFallbackChain_WithNeutralCulture_ExcludesSpecificCulture()
    {
        var chain = new CultureFallbackChain();
        var culture = CultureInfo.GetCultureInfo("de");

        var result = chain.GetFallbackChain(culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result[0]?.Name).IsEqualTo("de");
            _ = await Assert.That(result.Any(c => c?.Name == "de-DE")).IsFalse();
        }
    }

    [Test]
    public async Task GetFallbackChain_WithDefaultCulture_ExcludesDuplicates()
    {
        var chain = new CultureFallbackChain();
        var culture = CultureInfo.GetCultureInfo("en-US");

        var result = chain.GetFallbackChain(culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result[0]?.Name).IsEqualTo("en-US");
            _ = await Assert.That(result.Where(c => c?.Name == "en-US").Count()).IsEqualTo(1);
        }
    }

    [Test]
    public async Task GetFallbackChain_WithNullCulture_ThrowsArgumentNullException()
    {
        var chain = new CultureFallbackChain();

        var exception = Assert.Throws<ArgumentNullException>(() => chain.GetFallbackChain(null!));

        _ = await Assert.That(exception.ParamName).IsEqualTo("culture");
    }

    [Test]
    public async Task GetCultureSuffixes_WithSpecificCulture_ReturnsCorrectSuffixes()
    {
        var chain = new CultureFallbackChain();
        var culture = CultureInfo.GetCultureInfo("de-DE");

        var result = chain.GetCultureSuffixes(culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Count).IsGreaterThanOrEqualTo(4);
            _ = await Assert.That(result[0]).IsEqualTo(".de-DE");
            _ = await Assert.That(result[1]).IsEqualTo(".de");
            _ = await Assert.That(result.Any(s => s == ".en-US")).IsTrue();
            _ = await Assert.That(result.Any(s => s == ".en")).IsTrue();
            _ = await Assert.That(result[^1]).IsEqualTo(string.Empty); // Last should be empty
        }
    }

    [Test]
    public async Task GetCultureSuffixes_WithNeutralCulture_ReturnsCorrectSuffixes()
    {
        var chain = new CultureFallbackChain();
        var culture = CultureInfo.GetCultureInfo("de");

        var result = chain.GetCultureSuffixes(culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result[0]).IsEqualTo(".de");
            _ = await Assert.That(result.Any(s => string.IsNullOrEmpty(s))).IsTrue();
        }
    }

    [Test]
    public async Task GetCultureSuffixes_WithNullCulture_ThrowsArgumentNullException()
    {
        var chain = new CultureFallbackChain();

        var exception = Assert.Throws<ArgumentNullException>(() => chain.GetCultureSuffixes(null!));

        _ = await Assert.That(exception.ParamName).IsEqualTo("culture");
    }

    [Test]
    public async Task GetCultureSuffixes_AlwaysEndsWithEmptyString()
    {
        var chain = new CultureFallbackChain();
        var culture = CultureInfo.GetCultureInfo("ja-JP");

        var result = chain.GetCultureSuffixes(culture);

        _ = await Assert.That(result[^1]).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task GetFallbackChain_WithCustomDefaultCulture_IncludesCustomDefault()
    {
        var defaultCulture = CultureInfo.GetCultureInfo("es-ES");
        var chain = new CultureFallbackChain(defaultCulture);
        var culture = CultureInfo.GetCultureInfo("de-DE");

        var result = chain.GetFallbackChain(culture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Any(c => c?.Name == "es-ES")).IsTrue();
            _ = await Assert.That(result.Any(c => c?.Name == "es")).IsTrue();
            _ = await Assert.That(result.Any(c => c?.Name == "en-US")).IsFalse();
        }
    }
}
