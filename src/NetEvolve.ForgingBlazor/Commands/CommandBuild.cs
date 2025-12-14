namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;

/// <summary>
/// Provides the "build" command implementation for building and generating static content from a Forging Blazor application.
/// </summary>
/// <remarks>
/// This sealed class implements the build command that processes pages and generates static site output based on
/// configuration options. It registers standard options for environment, drafts, future content, logging, and output paths.
/// </remarks>
/// <seealso cref="CommandOptions"/>
/// <seealso cref="IStartUpMarker"/>
internal sealed class CommandBuild : Command, IStartUpMarker
{
    /// <summary>
    /// Stores the service provider for transferring services during command execution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBuild"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance providing access to registered application services.
    /// This is used to transfer services for command execution.
    /// </param>
    public CommandBuild(IServiceProvider serviceProvider)
        : base("build", "Builds and generates static content for a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(CommandOptions.ContentPath);
        Add(CommandOptions.Environment);
        Add(CommandOptions.IncludeDrafts);
        Add(CommandOptions.IncludeFuture);
        Add(CommandOptions.ProjectPath);
        Add(CommandOptions.OutputPath);

        SetAction(ExecuteAsync);
    }

    /// <summary>
    /// Executes the build command asynchronously.
    /// </summary>
    /// <param name="parseResult">The parsed command-line arguments.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, with an exit code result.</returns>
    private Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        _ = new ServiceCollection().TransferAllServices(_serviceProvider);

        return Task.FromResult(0);
    }
}
