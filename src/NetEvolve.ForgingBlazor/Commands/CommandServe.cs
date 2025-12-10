namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandServe : Command, IStartUpMarker
{
    private readonly IServiceProvider _serviceProvider;

    public CommandServe(IServiceProvider serviceProvider)
        : base("serve", "Serves a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(Environment);
        Add(IncludeDrafts);
        Add(IncludeFuture);
        Add(LogLevel);
        Add(ProjectPath);
        Add(OutputPath);

        SetAction(ExecuteAsync);
    }

    private Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        _ = new ServiceCollection().TransferAllServices(_serviceProvider);
        return Task.FromResult(0);
    }
}
