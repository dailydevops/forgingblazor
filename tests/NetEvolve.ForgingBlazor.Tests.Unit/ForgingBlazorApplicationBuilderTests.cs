namespace NetEvolve.ForgingBlazor.Tests.Unit;

public sealed class ForgingBlazorApplicationBuilderTests
{
    [Test]
    public async Task CreateDefaultBuilder_EmptyArguments_ReturnsOne()
    {
        var args = Array.Empty<string>();
        var builder = ForgingBlazorApplicationBuilder.CreateDefaultBuilder(args);
        var app = builder.Build();

        var exitCode = await app.RunAsync();

        _ = await Assert.That(exitCode).IsEqualTo(1);
    }

    [Test]
    public async Task CreateEmptyBuilder_EmptyArguments_ReturnsOne()
    {
        var args = Array.Empty<string>();
        var builder = ForgingBlazorApplicationBuilder.CreateEmptyBuilder(args);
        var app = builder.Build();

        var exitCode = await app.RunAsync();

        _ = await Assert.That(exitCode).IsEqualTo(1);
    }
}
