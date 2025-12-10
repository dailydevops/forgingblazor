namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandCreate : Command, IStartUpMarker
{
    private readonly IServiceProvider _serviceProvider;

    public CommandCreate(IServiceProvider serviceProvider)
        : base("create", "Creates a new page")
    {
        _serviceProvider = serviceProvider;
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
