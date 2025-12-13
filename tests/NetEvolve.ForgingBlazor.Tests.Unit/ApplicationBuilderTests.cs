namespace NetEvolve.ForgingBlazor.Tests.Unit;

public sealed class ApplicationBuilderTests
{
    [Test]
    public async Task CreateDefaultBuilder_EmptyArguments_ReturnsOne()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);
        var app = builder.Build();

        var exitCode = await app.RunAsync();

        _ = await Assert.That(exitCode).IsEqualTo(1);
    }

    [Test]
    public async Task CreateEmptyBuilder_EmptyArguments_ReturnsOne()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args).AddDefaultContent();
        var app = builder.Build();

        var exitCode = await app.RunAsync();

        _ = await Assert.That(exitCode).IsEqualTo(1);
    }
}
