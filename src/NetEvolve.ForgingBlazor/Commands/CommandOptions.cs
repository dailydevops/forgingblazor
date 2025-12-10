namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;

internal static class CommandOptions
{
    internal static Option<string?> ProjectPath { get; } =
        new Option<string?>("--project-path", "-p") { Description = "", Arity = ArgumentArity.ZeroOrOne };

    internal static Option<string?> OutputPath { get; } =
        new Option<string?>("--output-path", "-o")
        {
            Description = "Absolute or relative path for output",
            Arity = ArgumentArity.ZeroOrOne,
        };
}
