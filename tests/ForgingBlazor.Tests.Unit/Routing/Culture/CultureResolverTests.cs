namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing.Culture;

using System.Globalization;
using global::NetEvolve.ForgingBlazor.Routing.Culture;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class CultureResolverTests
{
    [Test]
    public async Task FromTwoLetterCode_WithValidCode_ReturnsCulture()
    {
        var result = CultureResolver.FromTwoLetterCode("en");

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.TwoLetterISOLanguageName).IsEqualTo("en");
        }
    }

    [Test]
    public async Task FromTwoLetterCode_WithInvalidCode_ReturnsNull()
    {
        var result = CultureResolver.FromTwoLetterCode("zz");

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task FromTwoLetterCode_WithNullCode_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => CultureResolver.FromTwoLetterCode(null!));

        _ = await Assert.That(exception.ParamName).IsEqualTo("twoLetterCode");
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task FromTwoLetterCode_WithEmptyOrWhitespace_ReturnsNull(string code)
    {
        var result = CultureResolver.FromTwoLetterCode(code);

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    [Arguments("e")]
    [Arguments("eng")]
    public async Task FromTwoLetterCode_WithInvalidLength_ReturnsNull(string code)
    {
        var result = CultureResolver.FromTwoLetterCode(code);

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task FromLcid_WithValidLcid_ReturnsCulture()
    {
        var result = CultureResolver.FromLcid(1033); // en-US

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.Name).IsEqualTo("en-US");
        }
    }

    [Test]
    public async Task FromLcid_WithInvalidLcid_ReturnsNull()
    {
        var result = CultureResolver.FromLcid(999999);

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    [Arguments(-100)]
    public async Task FromLcid_WithNegativeOrZeroLcid_ReturnsNull(int lcid)
    {
        var result = CultureResolver.FromLcid(lcid);

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task FromCultureName_WithValidName_ReturnsCulture()
    {
        var result = CultureResolver.FromCultureName("de-DE");

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.Name).IsEqualTo("de-DE");
        }
    }

    [Test]
    public async Task FromCultureName_WithInvalidName_ReturnsNull()
    {
        var result = CultureResolver.FromCultureName("invalid-culture");

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public void FromCultureName_WithNullName_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>("cultureName", () => CultureResolver.FromCultureName(null!));

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task FromCultureName_WithEmptyOrWhitespace_ReturnsNull(string name)
    {
        var result = CultureResolver.FromCultureName(name);

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Resolve_WithTwoLetterCode_ReturnsCulture()
    {
        var result = CultureResolver.Resolve("en");

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.TwoLetterISOLanguageName).IsEqualTo("en");
        }
    }

    [Test]
    public async Task Resolve_WithLcidString_ReturnsCulture()
    {
        var result = CultureResolver.Resolve("1033");

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.Name).IsEqualTo("en-US");
        }
    }

    [Test]
    public async Task Resolve_WithFullCultureName_ReturnsCulture()
    {
        var result = CultureResolver.Resolve("fr-FR");

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.Name).IsEqualTo("fr-FR");
        }
    }

    [Test]
    public async Task Resolve_WithNullString_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>("cultureString", () => CultureResolver.Resolve(null!));

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task Resolve_WithEmptyOrWhitespace_ReturnsNull(string cultureString)
    {
        var result = CultureResolver.Resolve(cultureString);

        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task Resolve_WithInvalidString_ReturnsNull()
    {
        var result = CultureResolver.Resolve("invalid");

        _ = await Assert.That(result).IsNull();
    }
}
