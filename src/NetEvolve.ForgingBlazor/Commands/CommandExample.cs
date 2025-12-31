namespace NetEvolve.ForgingBlazor.Commands;

using System;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;
using static NetEvolve.ForgingBlazor.Extensibility.Commands.CommandOptions;

/// <summary>
/// Provides the <c>example</c> command implementation for creating example pages based on ForgingBlazor configuration.
/// </summary>
/// <remarks>
/// <para>
/// This sealed class implements the <c>example</c> command that generates a folder structure with sample pages
/// demonstrating the capabilities of the current ForgingBlazor configuration. This is useful for users
/// learning the framework or setting up starter content.
/// </para>
/// <para>
/// The command transfers services from the parent provider to create a new service scope for example generation.
/// </para>
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
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// <para>
    /// This constructor initializes the <c>example</c> command with:
    /// <list type="bullet">
    /// <item><description><see cref="ProjectPath"/> option for specifying the project directory</description></item>
    /// <item><description><see cref="OutputPath"/> option for specifying the output location for example pages</description></item>
    /// <item><description>Action handler via <see cref="Command.SetAction(Func{ParseResult, CancellationToken, Task{int}})"/></description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public CommandExample(IServiceProvider serviceProvider)
        : base("example", "Creates a folder structure with example pages for a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(ProjectPath);
        Add(OutputPath);

        SetAction(ExecuteAsync);
    }

    /// <summary>
    /// Executes the <c>example</c> command asynchronously.
    /// </summary>
    /// <param name="parseResult">
    /// The <see cref="ParseResult"/> containing parsed command-line arguments.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the example generation operation.
    /// Defaults to <see cref="CancellationToken.None"/> if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains 0 on success or completion.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs the following steps:
    /// <list type="number">
    /// <item><description>Transfers all non-startup services from the parent service provider</description></item>
    /// <item><description>Builds a new service provider from the transferred services</description></item>
    /// <item><description>Executes example generation logic (currently returns 0)</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        _ = new ServiceCollection().TransferAllServices(_serviceProvider);

        return Task.FromResult(0);
    }
}
