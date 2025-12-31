namespace NetEvolve.ForgingBlazor.Extensibility.Commands;

using System.CommandLine;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides standard command-line options used across multiple ForgingBlazor CLI commands.
/// </summary>
/// <remarks>
/// This static class centralizes the definition of common command-line options to ensure consistency
/// across all commands. Each option is pre-configured with appropriate defaults and descriptions.
/// </remarks>
public static class CommandOptions
{
    /// <summary>
    /// Gets the command-line option for specifying the configuration file path.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> for the "--config-path" ("-c") flag with no default value.
    /// </value>
    /// <remarks>
    /// This option allows users to specify an absolute or relative path to a configuration file
    /// that contains settings for the ForgingBlazor application.
    /// </remarks>
    public static Option<string?> ConfigFile { get; } =
        new Option<string?>("--config-path", "-c")
        {
            Description = "Specifies the absolute or relative path to the configuration file",
            Arity = ArgumentArity.ZeroOrOne,
        };

    /// <summary>
    /// Gets the command-line option for specifying the content directory path.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of <see cref="string"/> for the "--content-path" flag with a default value of "content".
    /// </value>
    /// <remarks>
    /// This option allows users to specify an absolute or relative path to the directory containing content files.
    /// The default value of "content" assumes a standard project structure with a content folder at the project root.
    /// </remarks>
    public static Option<string> ContentPath { get; } =
        new Option<string>("--content-path")
        {
            Description = "Specifies the absolute or relative path to the content directory",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => DefaultPaths.Content,
        };

    /// <summary>
    /// Gets the command-line option for excluding dynamic content from processing.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of <see cref="bool"/> for the "--exclude-dynamic-content" flag with a default value of <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// When specified, this option excludes dynamically generated content from the build output,
    /// resulting in a purely static site without any runtime-generated pages.
    /// </remarks>
    public static Option<bool> ExcludeDynamicContent { get; } =
        new Option<bool>("--exclude-dynamic-content")
        {
            Description = "Excludes dynamically generated content from the build output",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => false,
        };

    /// <summary>
    /// Gets the command-line option for specifying the execution environment.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of <see cref="string"/> for the "--environment" ("-e") flag with a default value of "Production".
    /// </value>
    /// <remarks>
    /// This option allows users to specify the environment context for command execution,
    /// such as Development, Staging, or Production. This can affect behavior such as draft inclusion
    /// and logging levels.
    /// </remarks>
    public static Option<string> Environment { get; } =
        new Option<string>("--environment", "-e")
        {
            Description = "Specifies the execution environment (e.g., Development, Staging, Production)",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => Defaults.Environment,
        };

    /// <summary>
    /// Gets the command-line option for including draft pages in processing.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of <see cref="bool"/> for the "--include-drafts" flag with a default value of <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// When specified, this option includes draft content in the output. By default, only published content is processed.
    /// </remarks>
    public static Option<bool> IncludeDrafts { get; } =
        new Option<bool>("--include-drafts")
        {
            Description = "Includes draft pages in the build output",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => false,
        };

    /// <summary>
    /// Gets the command-line option for including future-dated pages in processing.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of <see cref="bool"/> for the "--include-future" flag with a default value of <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// When specified, this option includes pages scheduled for future publication in the output.
    /// By default, only currently published content is processed.
    /// </remarks>
    public static Option<bool> IncludeFuture { get; } =
        new Option<bool>("--include-future")
        {
            Description = "Includes pages with future publication dates in the build output",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => false,
        };

    /// <summary>
    /// Gets the command-line option for setting the logging level.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of <see cref="Microsoft.Extensions.Logging.LogLevel"/> for the "--log-level" ("-l") flag
    /// with a default value of <see cref="LogLevel.Information"/>.
    /// </value>
    /// <remarks>
    /// This option allows users to control the verbosity of logging output, from minimal (Error) to detailed (Trace) logs.
    /// </remarks>
    public static Option<LogLevel> LogLevel { get; } =
        new Option<LogLevel>("--log-level", "-l")
        {
            Description =
                "Specifies the minimum logging level (Trace, Debug, Information, Warning, Error, Critical, None)",
            Arity = ArgumentArity.ZeroOrOne,
            Recursive = true,
            DefaultValueFactory = static _ => Microsoft.Extensions.Logging.LogLevel.Information,
        };

    /// <summary>
    /// Gets the command-line option for specifying the project path.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of nullable <see cref="string"/> for the "--project-path" flag with no default value.
    /// </value>
    /// <remarks>
    /// This option allows users to specify the path to the ForgingBlazor project to process.
    /// </remarks>
    public static Option<string> ProjectPath { get; } =
        new Option<string>("--project-path")
        {
            Description = "Specifies the path to the ForgingBlazor project to process",
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = static _ => Directory.GetCurrentDirectory(),
        };

    /// <summary>
    /// Gets the command-line option for specifying the output path.
    /// </summary>
    /// <value>
    /// An <see cref="Option{T}"/> of nullable <see cref="string"/> for the "--output-path" flag with no default value.
    /// </value>
    /// <remarks>
    /// This option allows users to specify an absolute or relative path where output files should be generated.
    /// </remarks>
    public static Option<string?> OutputPath { get; } =
        new Option<string?>("--output-path")
        {
            Description = "Specifies the absolute or relative path for the build output",
            Arity = ArgumentArity.ZeroOrOne,
        };
}
