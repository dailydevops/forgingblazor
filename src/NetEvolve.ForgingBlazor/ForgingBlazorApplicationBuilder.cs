namespace NetEvolve.ForgingBlazor;

using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides the default implementation of <see cref="IApplicationBuilder"/> for configuring and building ForgingBlazor applications.
/// </summary>
/// <remarks>
/// This sealed class implements the builder pattern to configure services and create <see cref="ForgingBlazorApplication"/> instances.
/// Use the static <see cref="CreateBuilder(string[])"/> method to instantiate a new builder, then configure services
/// through the <see cref="Services"/> property, and finally call <see cref="Build"/> to create the application.
/// </remarks>
public sealed class ForgingBlazorApplicationBuilder : IApplicationBuilder
{
    private readonly string[] _args;

    /// <summary>
    /// Gets the service collection used to register services for dependency injection.
    /// </summary>
    /// <value>
    /// An <see cref="IServiceCollection"/> instance that allows registration of application services,
    /// configurations, and dependencies that will be available throughout the application lifecycle.
    /// </value>
    public IServiceCollection Services { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgingBlazorApplicationBuilder"/> class with the specified command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <remarks>
    /// This constructor initializes a new <see cref="ServiceCollection"/> for dependency injection.
    /// Use the static <see cref="CreateBuilder(string[])"/> method instead of calling this constructor directly.
    /// </remarks>
    public ForgingBlazorApplicationBuilder(string[] args)
    {
        _args = args;

        Services = new ServiceCollection();
    }

    /// <summary>
    /// Creates a new <see cref="ForgingBlazorApplicationBuilder"/> instance with the specified command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <returns>A new <see cref="IApplicationBuilder"/> instance ready for service configuration.</returns>
    /// <remarks>
    /// This is the recommended entry point for creating a ForgingBlazor application builder.
    /// After calling this method, configure services using the <see cref="Services"/> property,
    /// then call <see cref="Build"/> to create the application instance.
    /// </remarks>
    public static IApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = new ForgingBlazorApplicationBuilder(args);

        return builder;
    }

    /// <summary>
    /// Builds and returns a configured <see cref="IApplication"/> instance based on the current builder state.
    /// </summary>
    /// <returns>
    /// A fully configured <see cref="ForgingBlazorApplication"/> instance with all registered services available
    /// through dependency injection, ready to be run via <see cref="IApplication.RunAsync"/>.
    /// </returns>
    /// <remarks>
    /// This method constructs a service provider from the registered services and creates a new
    /// <see cref="ForgingBlazorApplication"/> with the command-line arguments and service provider.
    /// Call this method once after all service configuration is complete.
    /// </remarks>
    public IApplication Build()
    {
        var serviceProvider = Services.BuildServiceProvider();

        return new ForgingBlazorApplication(_args, serviceProvider);
    }
}
