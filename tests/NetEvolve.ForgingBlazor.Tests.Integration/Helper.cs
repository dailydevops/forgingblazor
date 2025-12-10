namespace NetEvolve.ForgingBlazor.Tests.Integration;

internal static class Helper
{
    public static async ValueTask VerifyStaticContent(string directoryPath, string[] args)
    {
        var builder = ForgingBlazorApplicationBuilder.CreateDefaultBuilder(args);

        var app = builder.Build();

        var exitCode = await app.RunAsync().ConfigureAwait(false);

        using (Assert.Multiple())
        {
            _ = await Assert.That(exitCode).IsEqualTo(0);
            _ = await VerifyDirectory(directoryPath).HashParameters();
        }
    }
}
