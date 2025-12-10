namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using static NetEvolve.ForgingBlazor.Commands.CommandOptions;

/// <summary>
/// Provides the "serve" command implementation for serving a Forging Blazor application.
/// </summary>
/// <remarks>
/// This sealed class implements the serve command that runs the ForgingBlazor application in a development or
/// preview server mode. It registers standard options for environment, drafts, future content, logging, and output paths.
/// </remarks>
/// <seealso cref="CommandOptions"/>
/// <seealso cref="IStartUpMarker"/>
internal sealed class CommandServe : Command, IStartUpMarker
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandServe"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance providing access to registered application services.
    /// This is used to transfer services for command execution.
    /// </param>
    public CommandServe(IServiceProvider serviceProvider)
        : base("serve", "Serves a Forging Blazor application.")
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
        _ = new ServiceCollection().TransferAllServices(_serviceProvider);
        return Task.FromResult(0);
    }
}
