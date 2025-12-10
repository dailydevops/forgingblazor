namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

internal sealed class ForgingBlazorApplication : IApplication
{
    private readonly string[] _args;
    private readonly IServiceProvider _serviceProvider;

    internal InvocationConfiguration? InvocationConfiguration { get; set; }

    public ForgingBlazorApplication(string[] args, IServiceProvider serviceProvider)
    {
        _args = args;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<int> RunAsync(CancellationToken cancellationToken = default)
    {
        // Configurate the command structure
        var rootCommand = new RootCommand("ForgingBlazor CLI");

        rootCommand.Subcommands.Add(new CommandBuild(_serviceProvider));
        rootCommand.Subcommands.Add(new CommandExample(_serviceProvider));

        var parseResults = rootCommand.Parse(_args);

        return await parseResults.InvokeAsync(InvocationConfiguration, cancellationToken).ConfigureAwait(false);
    }
}
