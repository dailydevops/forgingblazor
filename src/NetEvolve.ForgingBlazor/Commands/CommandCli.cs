namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;

/// <summary>
/// Provides the root CLI command for the ForgingBlazor command-line interface.
/// </summary>
/// <remarks>
/// <para>
/// This sealed class implements the root command that serves as the entry point for the ForgingBlazor CLI.
/// It aggregates all registered sub-commands and provides access to the complete command hierarchy.
/// All available sub-commands are automatically registered from the service provider during initialization.
/// </para>
/// <para>
/// If no sub-commands are registered in the service provider, an <see cref="InvalidOperationException"/> is thrown during instantiation.
/// </para>
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
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// <para>
    /// This constructor retrieves all registered commands from the service provider using <see cref="ServiceProviderServiceExtensions.GetServices{T}(IServiceProvider)"/>
    /// and adds them as sub-commands to the root command.
    /// </para>
    /// <para>
    /// Additionally, the <see cref="CommandOptions.LogLevel"/> option is added as a global option to all commands.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when no sub-commands are registered in the service provider.</exception>
    public CommandCli(IServiceProvider serviceProvider)
        : base("Command-line interface for managing Forging Blazor applications.")
    {
        var subCommands = serviceProvider.GetServices<Command>().ToArray();

        if (subCommands is null || subCommands.Length == 0)
        {
            throw new InvalidOperationException("No sub-commands registered");
        }

        foreach (var command in subCommands)
        {
            Add(command);
        }

        Add(CommandOptions.LogLevel);
    }
}
