namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.Logging;

internal static class CommandOptions
{
    internal static Option<string> Environment { get; } =
        new Option<string>("--environment", "-E")
        {
            Description = "Sets the environment (e.g., Development, Staging, Production)",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => "Production",
        };

    internal static Option<bool> IncludeDrafts { get; } =
        new Option<bool>("--include-drafts", "-D")
        {
            Description = "Includes draft content in the output",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => false,
        };

    internal static Option<bool> IncludeFuture { get; } =
        new Option<bool>("--include-future", "-F")
        {
            Description = "Includes future content in the output",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => false,
        };

    internal static Option<LogLevel> LogLevel { get; } =
        new Option<LogLevel>("--log-level", "-L")
        {
            Description = "Sets the logging level",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => Microsoft.Extensions.Logging.LogLevel.Information,
        };

    internal static Option<string?> ProjectPath { get; } =
        new Option<string?>("--project-path", "-p") { Description = "", Arity = ArgumentArity.ZeroOrOne };

    internal static Option<string?> OutputPath { get; } =
        new Option<string?>("--output-path", "-o")
        {
            Description = "Absolute or relative path for output",
            Arity = ArgumentArity.ZeroOrOne,
        };
}
