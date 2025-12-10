namespace NetEvolve.ForgingBlazor.Commands;

using System;
using System.CommandLine;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandExample : Command
{
    private IServiceProvider _serviceProvider;

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
        var projectPath = parseResult.GetRequiredValue(ProjectPath);
        var outputPath = parseResult.GetValue(OutputPath);

        return Task.FromResult(0);
    }
}
