namespace NetEvolve.ForgingBlazor.Commands;

using System;
using System.CommandLine;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandExample : Command
{
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IServiceProvider _serviceProvider;
#pragma warning restore S4487 // Unread "private" fields should be removed

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

    private static Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken) =>
        Task.FromResult(0);
}
