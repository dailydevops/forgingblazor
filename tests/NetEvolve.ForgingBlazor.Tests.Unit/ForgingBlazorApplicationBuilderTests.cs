namespace NetEvolve.ForgingBlazor.Tests.Unit;

public sealed class ForgingBlazorApplicationBuilderTests
{
    [Test]
    public async Task RunAsync_WithoutArguments_ReturnsOne()
    {
        var args = Array.Empty<string>();
        var builder = ForgingBlazorApplicationBuilder.CreateBuilder(args);
        var app = builder.Build();

        var exitCode = await app.RunAsync();

        _ = await Assert.That(exitCode).IsEqualTo(1);
    }
}
