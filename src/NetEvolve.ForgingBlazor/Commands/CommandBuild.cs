namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandBuild : Command
{
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IServiceProvider _serviceProvider;
#pragma warning restore S4487 // Unread "private" fields should be removed

    public CommandBuild(IServiceProvider serviceProvider)
        : base("build", "Builds and generates the static content for a Forging Blazor application.")
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

    private static Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken) =>
        Task.FromResult(0);
}
