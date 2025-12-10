namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

internal sealed class CommandBuild : Command, IStartUpMarker
{
    private readonly IServiceProvider _serviceProvider;

    public CommandBuild(IServiceProvider serviceProvider)
        : base("build", "Builds and generates the static content for a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(CommandOptions.Environment);
        Add(CommandOptions.IncludeDrafts);
        Add(CommandOptions.IncludeFuture);
        Add(CommandOptions.LogLevel);
        Add(CommandOptions.ProjectPath);
        Add(CommandOptions.OutputPath);

        SetAction(ExecuteAsync);
    }

    private Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        _ = new ServiceCollection().TransferAllServices(_serviceProvider);

        return Task.FromResult(0);
    }
}
