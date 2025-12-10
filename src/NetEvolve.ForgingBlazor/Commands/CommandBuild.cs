namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

internal sealed class CommandBuild : Command
{
    private readonly IServiceProvider _serviceProvider;

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

    private Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var projectPath = parseResult.GetValue(ProjectPath);
        var outputPath = parseResult.GetValue(OutputPath);

        return Task.FromResult(0);
    }
}
