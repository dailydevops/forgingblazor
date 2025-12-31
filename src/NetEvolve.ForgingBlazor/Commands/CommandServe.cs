namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;
using static NetEvolve.ForgingBlazor.Extensibility.Commands.CommandOptions;

/// <summary>
/// Provides the <c>serve</c> command implementation for serving a Forging Blazor application.
/// </summary>
/// <remarks>
/// <para>
/// This sealed class implements the <c>serve</c> command that runs the ForgingBlazor application in a development or
/// preview server mode. It registers standard options for environment, drafts, future content, logging, and output paths.
/// </para>
/// <para>
/// The command transfers services from the parent provider to create a new service scope for serving the application.
/// </para>
/// </remarks>
/// <seealso cref="CommandOptions"/>
/// <seealso cref="IStartUpMarker"/>
internal sealed class CommandServe : Command, IStartUpMarker
{
    /// <summary>
    /// Stores the service provider for transferring services during command execution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandServe"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance providing access to registered application services.
    /// This is used to transfer services for command execution.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// <para>
    /// This constructor initializes the <c>serve</c> command with:
    /// <list type="bullet">
    /// <item><description><see cref="Environment"/> option for specifying the environment (development, staging, production)</description></item>
    /// <item><description><see cref="IncludeDrafts"/> option for including draft pages</description></item>
    /// <item><description><see cref="IncludeFuture"/> option for including future-dated content</description></item>
    /// <item><description><see cref="ProjectPath"/> option for specifying the project directory</description></item>
    /// <item><description><see cref="OutputPath"/> option for specifying the output location</description></item>
    /// <item><description>Action handler via <see cref="Command.SetAction(Func{ParseResult, CancellationToken, Task{int}})"/></description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public CommandServe(IServiceProvider serviceProvider)
        : base("serve", "Starts a development server for a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(Environment);
        Add(IncludeDrafts);
        Add(IncludeFuture);
        Add(ProjectPath);
        Add(OutputPath);

        SetAction(ExecuteAsync);
    }

    /// <summary>
    /// Executes the <c>serve</c> command asynchronously.
    /// </summary>
    /// <param name="parseResult">
    /// The <see cref="ParseResult"/> containing parsed command-line arguments.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the serve operation.
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
    /// <item><description>Executes the serve logic (currently returns 0)</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        _ = new ServiceCollection().TransferAllServices(_serviceProvider);
        return Task.FromResult(0);
    }
}
