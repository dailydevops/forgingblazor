namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides the default implementation of <see cref="IApplication"/> for executing ForgingBlazor command-line applications.
/// </summary>
/// <remarks>
/// <para>
/// This sealed class encapsulates the complete application lifecycle and command execution logic for ForgingBlazor CLI applications.
/// It accepts command-line arguments and a <see cref="IServiceProvider"/>, orchestrating the parsing and invocation of registered
/// commands through the System.CommandLine framework.
/// </para>
/// <para>
/// The application retrieves the root command from the service provider, parses the command-line arguments,
/// and delegates execution to the appropriate command handler based on the parsed input.
/// </para>
/// <para>
/// This class is typically not instantiated directly; use <see cref="ApplicationBuilder"/> to create and configure
/// application instances with proper dependency injection setup.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var builder = ApplicationBuilder.CreateDefaultBuilder(args);
/// var app = builder.Build();
/// return await app.RunAsync();
/// </code>
/// </example>
/// <seealso cref="IApplication"/>
/// <seealso cref="ApplicationBuilder"/>
/// <seealso cref="RootCommand"/>
internal sealed class Application : IApplication
{
    /// <summary>
    /// Stores the command-line arguments passed to the application.
    /// </summary>
    /// <remarks>
    /// These arguments are captured from the application entry point and later parsed by the
    /// <see cref="RootCommand"/> to determine which CLI commands to execute and with what parameters.
    /// </remarks>
    private readonly string[] _args;

    /// <summary>
    /// Stores the service provider for resolving dependencies during command execution.
    /// </summary>
    /// <remarks>
    /// This service provider is created by the <see cref="ApplicationBuilder"/> and contains all
    /// registered application services, including the <see cref="RootCommand"/> and registered content registrations.
    /// It is used during <see cref="RunAsync"/> to retrieve the root command for parsing and executing CLI commands.
    /// </remarks>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Gets or sets the invocation configuration used to customize command execution behavior.
    /// </summary>
    /// <value>
    /// An optional <see cref="InvocationConfiguration"/> instance that controls how commands are executed,
    /// including error handling, output redirection, and other CLI invocation options.
    /// Returns <see langword="null"/> if no custom configuration is configured.
    /// </value>
    /// <remarks>
    /// This configuration is passed to the invocation method during command execution in the <see cref="RunAsync"/> method.
    /// It allows fine-grained control over how the System.CommandLine framework processes and executes parsed commands.
    /// </remarks>
    /// <seealso cref="RunAsync"/>
    /// <seealso cref="InvocationConfiguration"/>
    internal InvocationConfiguration? InvocationConfiguration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Application"/> class with command-line arguments and a service provider.
    /// </summary>
    /// <param name="args">
    /// The command-line arguments passed to the application.
    /// Cannot be <see langword="null"/>.
    /// These arguments are parsed to determine which commands to execute and with what parameters.
    /// </param>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance that provides access to registered application services.
    /// Cannot be <see langword="null"/>.
    /// This service provider is used to retrieve the <see cref="RootCommand"/> and other registered services
    /// for command execution and dependency resolution.
    /// </param>
    /// <remarks>
    /// <para>
    /// This constructor stores references to both the command-line arguments and the service provider
    /// for later use during the <see cref="RunAsync"/> execution phase.
    /// </para>
    /// <para>
    /// This constructor should typically not be called directly. Instead, use <see cref="ApplicationBuilder"/>
    /// to create and configure application instances with proper dependency injection setup and default services.
    /// </para>
    /// </remarks>
    /// <seealso cref="ApplicationBuilder.Build"/>
    /// <seealso cref="ApplicationBuilder.CreateDefaultBuilder(string[])"/>
    /// <seealso cref="RunAsync"/>
    public Application(string[] args, IServiceProvider serviceProvider)
    {
        _args = args;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Asynchronously runs the application by parsing and executing the command-line arguments.
    /// </summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> that can be used to request cancellation of the application execution.
    /// Defaults to <see cref="CancellationToken.None"/> if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
    /// The task result contains an exit code where 0 typically indicates success, and non-zero values indicate error conditions.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method executes the following steps:
    /// <list type="number">
    /// <item><description>Retrieves the <see cref="RootCommand"/> from the service provider</description></item>
    /// <item><description>Parses the command-line arguments using the root command's parsing logic</description></item>
    /// <item><description>Invokes the parsed command with the optional <see cref="InvocationConfiguration"/></description></item>
    /// <item><description>Returns the exit code from the command invocation</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The command is resolved from the service provider at runtime, allowing for dependency injection
    /// of any required services into the command handlers. The <c>ConfigureAwait(false)</c> call ensures
    /// efficient async execution without forcing continuations to run on the original synchronization context.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the <see cref="RootCommand"/> is not found in the service provider.
    /// This indicates that the application builder's <see cref="ApplicationBuilder.Build"/> method
    /// was not properly called or required services were not registered.
    /// </exception>
    /// <seealso cref="InvocationConfiguration"/>
    /// <seealso cref="RootCommand"/>
    /// <seealso cref="ApplicationBuilder.Build"/>
    public async ValueTask<int> RunAsync(CancellationToken cancellationToken = default)
    {
        // Configure the command structure
        var rootCommand =
            _serviceProvider.GetService<RootCommand>()
            ?? throw new InvalidOperationException($"{nameof(RootCommand)} not found in the service provider.");
        var parseResults = rootCommand.Parse(_args);

        return await parseResults.InvokeAsync(InvocationConfiguration, cancellationToken).ConfigureAwait(false);
    }
}
