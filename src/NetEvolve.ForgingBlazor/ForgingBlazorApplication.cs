namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides the default implementation of <see cref="IApplication"/> for executing ForgingBlazor command-line applications.
/// </summary>
/// <remarks>
/// This sealed class encapsulates the application lifecycle and command execution logic for ForgingBlazor CLI applications.
/// It accepts command-line arguments and a service provider, orchestrating the parsing and invocation of registered commands.
/// </remarks>
/// <seealso cref="IApplication"/>
/// <seealso cref="ForgingBlazorApplicationBuilder"/>
internal sealed class ForgingBlazorApplication : IApplication
{
    private readonly string[] _args;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Gets or sets the invocation configuration used to customize command execution behavior.
    /// </summary>
    /// <value>
    /// An optional <see cref="InvocationConfiguration"/> instance that controls how commands are executed,
    /// or <see langword="null"/> if no custom configuration is required.
    /// </value>
    internal InvocationConfiguration? InvocationConfiguration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgingBlazorApplication"/> class with command-line arguments and a service provider.
    /// </summary>
    /// <param name="args">
    /// The command-line arguments passed to the application. Cannot be <see langword="null"/>.
    /// These arguments are parsed to determine which commands to execute and with what parameters.
    /// </param>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance that provides access to registered application services.
    /// This service provider is used to resolve dependencies for command execution.
    /// </param>
    /// <remarks>
    /// This constructor should typically not be called directly. Instead, use <see cref="ForgingBlazorApplicationBuilder"/>
    /// to create and configure application instances.
    /// </remarks>
    public ForgingBlazorApplication(string[] args, IServiceProvider serviceProvider)
    {
        _args = args;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Asynchronously runs the application by parsing and executing the command-line arguments.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the application execution.
    /// Defaults to <see cref="CancellationToken.None"/> if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
    /// The task result contains an exit code where 0 typically indicates success, and non-zero values indicate error conditions.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method retrieves the root command from the service provider, parses the command-line arguments,
    /// and invokes the appropriate command handler based on the parsed results.
    /// </para>
    /// <para>
    /// If the <see cref="RootCommand"/> is not registered in the service provider, an <see cref="InvalidOperationException"/> is thrown.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the <see cref="RootCommand"/> is not found in the service provider.</exception>
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
