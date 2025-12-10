namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides the root CLI command for the ForgingBlazor command-line interface.
/// </summary>
/// <remarks>
/// This sealed class implements the root command that serves as the entry point for the ForgingBlazor CLI.
/// It aggregates all registered sub-commands and provides access to the complete command hierarchy.
/// All available sub-commands are automatically registered from the service provider during initialization.
/// </remarks>
/// <seealso cref="IStartUpMarker"/>
internal sealed class CommandCli : RootCommand, IStartUpMarker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandCli"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance providing access to registered sub-commands.
    /// Must contain registered <see cref="Command"/> instances to populate the root command.
    /// </param>
    /// <remarks>
    /// This constructor retrieves all registered commands from the service provider and adds them as sub-commands.
    /// If no commands are registered, an <see cref="InvalidOperationException"/> is thrown.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when no sub-commands are registered in the service provider.</exception>
    public CommandCli(IServiceProvider serviceProvider)
        : base("ForgingBlazor CLI")
    {
        var subCommands = serviceProvider.GetServices<Command>().ToArray();

        if (subCommands is null || subCommands.Length == 0)
        {
            throw new InvalidOperationException("No sub-commands registered");
        }

        foreach (var command in subCommands)
        {
            Subcommands.Add(command);
        }
    }
}
