namespace NetEvolve.ForgingBlazor.Commands;

using System;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandExample : Command, IStartUpMarker
{
    private readonly IServiceProvider _serviceProvider;

    public CommandExample(IServiceProvider serviceProvider)
        : base(
            "example",
            "Creates a folder structure with example pages based on the current ForgingBlazor configuration."
        )
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
