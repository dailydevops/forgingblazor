namespace NetEvolve.ForgingBlazor.Tests.Integration.Commands;

using System.Threading.Tasks;

public class CommandHelpTests
{
    [Test]
    [MethodDataSource(nameof(GetHelpArguments))]
    public async ValueTask Help_Theory_Expected(string[] args)
    {
        var builder = ForgingBlazorApplicationBuilder.CreateDefaultBuilder(args);

        var app = builder.Build();
        using var output = new StringWriter();

        if (app is ForgingBlazorApplication forgingBlazorApplication)
        {
            forgingBlazorApplication.InvocationConfiguration = new() { Output = output };
        }

        _ = await app.RunAsync().ConfigureAwait(false);

        _ = await Verify(new { args, output }).DontIgnoreEmptyCollections().HashParameters();
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
