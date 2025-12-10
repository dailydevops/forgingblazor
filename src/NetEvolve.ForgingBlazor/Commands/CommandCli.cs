namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

internal sealed class CommandCli : RootCommand, IStartUpMarker
{
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
