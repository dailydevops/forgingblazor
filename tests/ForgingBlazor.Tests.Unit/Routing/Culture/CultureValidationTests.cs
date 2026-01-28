namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing.Culture;

using System.Globalization;
using NetEvolve.ForgingBlazor.Routing.Culture;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class CultureValidationTests
{
    [Test]
    public async Task Constructor_WithValidParameters_CreatesInstance()
    {
        var supportedCultures = new HashSet<CultureInfo>
        {
            CultureInfo.GetCultureInfo("en-US"),
            CultureInfo.GetCultureInfo("de-DE"),
        };
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");

        var validation = new CultureValidation(supportedCultures, defaultCulture);

        using (Assert.Multiple())
        {
            _ = await Assert.That(validation).IsNotNull();
            _ = await Assert.That(validation.DefaultCulture).IsEqualTo(defaultCulture);
            _ = await Assert.That(validation.SupportedCultures.Count).IsEqualTo(2);
        }
    }

    [Test]
    public void Constructor_WithNullSupportedCultures_ThrowsArgumentNullException()
    {
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");

        _ = Assert.Throws<ArgumentNullException>(
            "supportedCultures",
            () => _ = new CultureValidation(null!, defaultCulture)
        );
    }

    [Test]
    public void Constructor_WithNullDefaultCulture_ThrowsArgumentNullException()
    {
        var supportedCultures = new HashSet<CultureInfo> { CultureInfo.GetCultureInfo("en-US") };

        _ = Assert.Throws<ArgumentNullException>(
            "defaultCulture",
            () => _ = new CultureValidation(supportedCultures, null!)
        );
    }

    [Test]
    public void Constructor_WithEmptySupportedCultures_ThrowsArgumentException()
    {
        var supportedCultures = new HashSet<CultureInfo>();
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");

        _ = Assert.Throws<ArgumentException>(
            "supportedCultures",
            () => _ = new CultureValidation(supportedCultures, defaultCulture)
        );
    }

    [Test]
    public async Task IsCultureSupported_WithSupportedCulture_ReturnsTrue()
    {
        var supportedCultures = new HashSet<CultureInfo> { CultureInfo.GetCultureInfo("en-US") };
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var validation = new CultureValidation(supportedCultures, defaultCulture);

        var result = validation.IsCultureSupported(CultureInfo.GetCultureInfo("en-US"));

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsCultureSupported_WithUnsupportedCulture_ReturnsFalse()
    {
        var supportedCultures = new HashSet<CultureInfo> { CultureInfo.GetCultureInfo("en-US") };
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var validation = new CultureValidation(supportedCultures, defaultCulture);

        var result = validation.IsCultureSupported(CultureInfo.GetCultureInfo("de-DE"));

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public void IsCultureSupported_WithNullCulture_ThrowsArgumentNullException()
    {
        var supportedCultures = new HashSet<CultureInfo> { CultureInfo.GetCultureInfo("en-US") };
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var validation = new CultureValidation(supportedCultures, defaultCulture);

        _ = Assert.Throws<ArgumentNullException>("culture", () => validation.IsCultureSupported(null!));
    }

    [Test]
    public async Task ValidateDefaultCulture_WhenDefaultIsSupported_DoesNotThrow()
    {
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var supportedCultures = new HashSet<CultureInfo> { defaultCulture };
        var validation = new CultureValidation(supportedCultures, defaultCulture);

        validation.ValidateDefaultCulture();
    }

    [Test]
    public async Task ValidateDefaultCulture_WhenDefaultIsNotSupported_ThrowsInvalidOperationException()
    {
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var supportedCultures = new HashSet<CultureInfo> { CultureInfo.GetCultureInfo("de-DE") };
        var validation = new CultureValidation(supportedCultures, defaultCulture);

        _ = Assert.Throws<InvalidOperationException>(() => validation.ValidateDefaultCulture());
    }

    [Test]
    public async Task ThrowIfUnsupported_WithSupportedCulture_DoesNotThrow()
    {
        var culture = CultureInfo.GetCultureInfo("en-US");
        var supportedCultures = new HashSet<CultureInfo> { culture };
        var validation = new CultureValidation(supportedCultures, culture);

        validation.ThrowIfUnsupported(culture);
    }

    [Test]
    public async Task ThrowIfUnsupported_WithUnsupportedCulture_ThrowsInvalidOperationException()
    {
        var supportedCultures = new HashSet<CultureInfo> { CultureInfo.GetCultureInfo("en-US") };
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var validation = new CultureValidation(supportedCultures, defaultCulture);
        var unsupportedCulture = CultureInfo.GetCultureInfo("de-DE");

        _ = Assert.Throws<InvalidOperationException>(() => validation.ThrowIfUnsupported(unsupportedCulture));
    }

    [Test]
    public void ThrowIfUnsupported_WithNullCulture_ThrowsArgumentNullException()
    {
        var supportedCultures = new HashSet<CultureInfo> { CultureInfo.GetCultureInfo("en-US") };
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var validation = new CultureValidation(supportedCultures, defaultCulture);

        _ = Assert.Throws<ArgumentNullException>("culture", () => validation.ThrowIfUnsupported(null!));
    }

    [Test]
    public async Task SupportedCultures_ReturnsReadOnlySet()
    {
        var supportedCultures = new HashSet<CultureInfo>
        {
            CultureInfo.GetCultureInfo("en-US"),
            CultureInfo.GetCultureInfo("de-DE"),
        };
        var defaultCulture = CultureInfo.GetCultureInfo("en-US");
        var validation = new CultureValidation(supportedCultures, defaultCulture);

        var result = validation.SupportedCultures;

        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Count).IsEqualTo(2);
            _ = await Assert.That(result.Contains(CultureInfo.GetCultureInfo("en-US"))).IsTrue();
            _ = await Assert.That(result.Contains(CultureInfo.GetCultureInfo("de-DE"))).IsTrue();
        }
    }
}
