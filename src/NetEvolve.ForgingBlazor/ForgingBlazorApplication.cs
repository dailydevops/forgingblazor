namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

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
        var rootCommand = new CommandCli(_serviceProvider);
        var parseResults = rootCommand.Parse(_args);

        return await parseResults.InvokeAsync(InvocationConfiguration, cancellationToken).ConfigureAwait(false);
    }
}
