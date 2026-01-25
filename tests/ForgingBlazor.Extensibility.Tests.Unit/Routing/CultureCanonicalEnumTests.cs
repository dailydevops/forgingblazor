namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

public class CultureCanonicalEnumTests
{
    [Test]
    public async Task Enum_HasWithoutPrefixValue()
    {
        _ = await Assert.That((int)CultureCanonical.WithoutPrefix).IsEqualTo(0);
    }

    [Test]
    public async Task Enum_HasWithPrefixValue()
    {
        _ = await Assert.That((int)CultureCanonical.WithPrefix).IsEqualTo(1);
    }

    [Test]
    public async Task Enum_HasTwoValues()
    {
        var values = System.Enum.GetValues<CultureCanonical>();

        _ = await Assert.That(values.Length).IsEqualTo(2);
    }
}
