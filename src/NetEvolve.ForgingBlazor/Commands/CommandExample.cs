namespace NetEvolve.ForgingBlazor.Commands;

using System;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;
using static NetEvolve.ForgingBlazor.Extensibility.Commands.CommandOptions;

/// <summary>
/// Provides the "example" command implementation for creating example pages based on ForgingBlazor configuration.
/// </summary>
/// <remarks>
/// This sealed class implements the example command that generates a folder structure with sample pages
/// demonstrating the capabilities of the current ForgingBlazor configuration. This is useful for users
/// learning the framework or setting up starter content.
/// </remarks>
/// <seealso cref="CommandOptions"/>
/// <seealso cref="IStartUpMarker"/>
internal sealed class CommandExample : Command, IStartUpMarker
{
    /// <summary>
    /// Stores the service provider for transferring services during command execution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandExample"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance providing access to registered application services.
    /// This is used to transfer services for command execution.
    /// </param>
    public CommandExample(IServiceProvider serviceProvider)
        : base("example", "Creates a folder structure with example pages for a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(ProjectPath);
        Add(OutputPath);

        SetAction(ExecuteAsync);
    }

    /// <summary>
    /// Executes the example command asynchronously.
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
