namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

public class PaginationUrlFormatEnumTests
{
    [Test]
    public async Task Enum_HasNumericValue()
    {
        _ = await Assert.That((int)PaginationUrlFormat.Numeric).IsEqualTo(0);
    }

    [Test]
    public async Task Enum_HasPrefixedValue()
    {
        _ = await Assert.That((int)PaginationUrlFormat.Prefixed).IsEqualTo(1);
    }

    [Test]
    public async Task Enum_HasTwoValues()
    {
        var values = System.Enum.GetValues<PaginationUrlFormat>();

        _ = await Assert.That(values.Length).IsEqualTo(2);
    }
}
