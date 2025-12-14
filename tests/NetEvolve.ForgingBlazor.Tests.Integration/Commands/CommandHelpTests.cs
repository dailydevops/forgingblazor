namespace NetEvolve.ForgingBlazor.Tests.Integration.Commands;

using System.Threading.Tasks;
using NetEvolve.ForgingBlazor;

public class CommandHelpTests
{
    [Test]
    [MethodDataSource(nameof(GetHelpArguments))]
    public async ValueTask Help_Theory_Expected(string[] args)
    {
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var app = builder.Build();
        await using var output = new StringWriter();
        await using var error = new StringWriter();

        if (app is Application application)
        {
            application.InvocationConfiguration = new() { Error = error, Output = output };
        }

        _ = await app.RunAsync().ConfigureAwait(false);

        _ = await Verify(
                new
                {
                    args,
                    error,
                    output,
                }
            )
            .DontIgnoreEmptyCollections()
            .HashParameters();
    }

    public static IEnumerable<Func<string[]>> GetHelpArguments =>
        [
            () => [],
            () => ["-h"],
            () => ["create", "--help"],
            () => ["build", "--help"],
            () => ["example", "--help"],
            () => ["serve", "--help"],
        ];
}
