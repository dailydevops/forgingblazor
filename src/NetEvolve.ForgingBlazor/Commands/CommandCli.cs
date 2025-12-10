namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;

internal sealed class CommandCli : RootCommand
{
    public CommandCli(IServiceProvider serviceProvider)
        : base("ForgingBlazor CLI")
    {
        Subcommands.Add(new CommandBuild(serviceProvider));
        Subcommands.Add(new CommandCreate(serviceProvider));
        Subcommands.Add(new CommandExample(serviceProvider));
        Subcommands.Add(new CommandServe(serviceProvider));
    }
}
