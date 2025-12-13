namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;
using static NetEvolve.ForgingBlazor.Extensibility.Commands.CommandOptions;

/// <summary>
/// Provides the "create" command implementation for creating new pages in a Forging Blazor application.
/// </summary>
/// <remarks>
/// This sealed class implements the create command that generates new page files in the specified project.
/// It registers options for specifying the project path and output destination for the new page.
/// </remarks>
/// <seealso cref="CommandOptions"/>
/// <seealso cref="IStartUpMarker"/>
internal sealed class CommandCreate : Command, IStartUpMarker
{
    /// <summary>
    /// Stores the service provider for transferring services during command execution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandCreate"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance providing access to registered application services.
    /// This is used to transfer services for command execution.
    /// </param>
    public CommandCreate(IServiceProvider serviceProvider)
        : base("create", "Creates a new page for a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(ProjectPath);
        Add(OutputPath);

        SetAction(ExecuteAsync);
    }

    /// <summary>
    /// Executes the create command asynchronously.
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
