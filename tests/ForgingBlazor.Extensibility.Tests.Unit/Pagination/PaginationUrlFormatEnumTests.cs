namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

public class PaginationUrlFormatEnumTests
{
    [Test]
    public async Task Enum_HasTwoValues()
    {
        var values = Enum.GetValues<PaginationUrlFormat>();

        _ = await Assert.That(values.Length).IsEqualTo(2);
    }
}
