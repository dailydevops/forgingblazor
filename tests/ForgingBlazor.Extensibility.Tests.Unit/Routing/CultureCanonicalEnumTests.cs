namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

public class CultureCanonicalEnumTests
{
    [Test]
    public async Task Enum_HasTwoValues()
    {
        var values = Enum.GetValues<CultureCanonical>();

        _ = await Assert.That(values.Length).IsEqualTo(2);
    }
}
