namespace NetEvolve.ForgingBlazor.Tests.Integration;

using Microsoft.Extensions.Logging;
using NetEvolve.ForgingBlazor.Logging;

internal static class Helper
{
    public static async ValueTask VerifyStaticContent(string directoryPath, string[] args)
    {
        var builder = ApplicationBuilder
            .CreateDefaultBuilder(args)
            .WithLogging(loggingBuilder => loggingBuilder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        var app = builder.Build();

        var exitCode = await app.RunAsync().ConfigureAwait(false);

        using (Assert.Multiple())
        {
            _ = await Assert.That(exitCode).IsEqualTo(0);
            _ = await VerifyDirectory(directoryPath).HashParameters();
        }
    }
}
