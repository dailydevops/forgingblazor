namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandCreate : Command
{
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IServiceProvider _serviceProvider;
#pragma warning restore S4487 // Unread "private" fields should be removed

    public CommandCreate(IServiceProvider serviceProvider)
        : base("create", "Creates a new page")
    {
        _serviceProvider = serviceProvider;
        Add(ProjectPath);
        Add(OutputPath);
        SetAction(ExecuteAsync);
    }

    private static Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken) =>
        Task.FromResult(0);
}
