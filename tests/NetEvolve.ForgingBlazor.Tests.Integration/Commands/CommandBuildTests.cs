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
            args = [.. args, directory.Path];
        }
        else
        {
            args = ["build"];
        }
        await Helper.VerifyStaticContent(directory.Path, args).ConfigureAwait(false);
    }

    public static IEnumerable<Func<string[]>> GetBuildArguments =>
        [() => Array.Empty<string>(), () => ["build", "--output-path"], () => ["build", "-o"]];
}
