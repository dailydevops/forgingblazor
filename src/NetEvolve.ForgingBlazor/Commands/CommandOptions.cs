namespace NetEvolve.ForgingBlazor.Commands;

using System.CommandLine;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides standard command-line options used across multiple ForgingBlazor CLI commands.
/// </summary>
/// <remarks>
/// This static class centralizes the definition of common command-line options to ensure consistency
/// across all commands. Each option is pre-configured with appropriate defaults and descriptions.
/// </remarks>
internal static class CommandOptions
{
    /// <summary>
    /// Gets the command-line option for specifying the execution environment.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> for the "--environment" ("-E") flag with a default value of "Production".
    /// </value>
    /// <remarks>
    /// This option allows users to specify the environment context for command execution,
    /// such as Development, Staging, or Production. This can affect behavior such as draft inclusion
    /// and logging levels.
    /// </remarks>
    internal static Option<string> Environment { get; } =
        new Option<string>("--environment", "-E")
        {
            Description = "Sets the environment (e.g., Development, Staging, Production)",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => "Production",
        };

    /// <summary>
    /// Gets the command-line option for including draft pages in processing.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> for the "--include-drafts" ("-D") flag with a default value of <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// When specified, this option includes draft content in the output. By default, only published content is processed.
    /// </remarks>
    internal static Option<bool> IncludeDrafts { get; } =
        new Option<bool>("--include-drafts", "-D")
        {
            Description = "Includes draft content in the output",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => false,
        };

    /// <summary>
    /// Gets the command-line option for including future-dated pages in processing.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> for the "--include-future" ("-F") flag with a default value of <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// When specified, this option includes pages scheduled for future publication in the output.
    /// By default, only currently published content is processed.
    /// </remarks>
    internal static Option<bool> IncludeFuture { get; } =
        new Option<bool>("--include-future", "-F")
        {
            Description = "Includes future content in the output",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => false,
        };

    /// <summary>
    /// Gets the command-line option for setting the logging level.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> for the "--log-level" ("-L") flag with a default value of <see cref="LogLevel.Information"/>.
    /// </value>
    /// <remarks>
    /// This option allows users to control the verbosity of logging output, from minimal (Error) to detailed (Trace) logs.
    /// </remarks>
    internal static Option<LogLevel> LogLevel { get; } =
        new Option<LogLevel>("--log-level", "-L")
        {
            Description = "Sets the logging level",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => Microsoft.Extensions.Logging.LogLevel.Information,
        };

    /// <summary>
    /// Gets the command-line option for specifying the project path.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> for the "--project-path" ("-p") flag with no default value.
    /// </value>
    /// <remarks>
    /// This option allows users to specify the path to the ForgingBlazor project to process.
    /// </remarks>
    internal static Option<string?> ProjectPath { get; } =
        new Option<string?>("--project-path", "-p") { Description = "", Arity = ArgumentArity.ZeroOrOne };

    /// <summary>
    /// Gets the command-line option for specifying the output path.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> for the "--output-path" ("-o") flag with no default value.
    /// </value>
    /// <remarks>
    /// This option allows users to specify an absolute or relative path where output files should be generated.
    /// </remarks>
    internal static Option<string?> OutputPath { get; } =
        new Option<string?>("--output-path", "-o")
        {
            Description = "Absolute or relative path for output",
            Arity = ArgumentArity.ZeroOrOne,
        };
}
