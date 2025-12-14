namespace NetEvolve.ForgingBlazor.Tests.Integration.Commands;

public sealed class CommandBuildTests
{
    [Test]
    [MethodDataSource(nameof(GetBuildArguments))]
    public async ValueTask Build_DefaultArguments_GeneratesStaticContent(string[] args)
    {
        using var directory = new TempDirectory();

        if (args is not null && args.Length != 0)
        {
            args = [.. args, directory.Path, "--content-path", "_setup/content"];
        }
        else
        {
            args = ["build", "--content-path", "_setup/Content"];
        }

        await Helper.VerifyStaticContent(directory.Path, args).ConfigureAwait(false);
    }

    public static IEnumerable<Func<string[]>> GetBuildArguments => [() => [], () => ["build", "--output-path"]];
}
