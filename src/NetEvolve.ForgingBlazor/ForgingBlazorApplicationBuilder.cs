namespace NetEvolve.ForgingBlazor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides the default implementation of <see cref="IApplicationBuilder"/> for configuring and building ForgingBlazor applications.
/// </summary>
/// <remarks>
/// <para>
/// This sealed class implements the builder pattern to configure services and create <see cref="ForgingBlazorApplication"/> instances.
/// The builder provides a fluent API for configuring dependency injection services before creating the final application instance.
/// </para>
/// <para>
/// Use the static <see cref="CreateDefaultBuilder(string[])"/> method to instantiate a new builder with default services,
/// or <see cref="CreateEmptyBuilder(string[])"/> for a minimal configuration without default services.
/// Configure services through the <see cref="Services"/> property, and finally call <see cref="Build"/> to create the application.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var builder = ForgingBlazorApplicationBuilder.CreateDefaultBuilder(args);
/// builder.Services.AddSingleton&lt;IMyService, MyService&gt;();
/// var app = builder.Build();
/// await app.RunAsync();
/// </code>
/// </example>
/// <seealso cref="IApplicationBuilder"/>
/// <seealso cref="ForgingBlazorApplication"/>
/// <seealso cref="IApplication"/>
public sealed class ForgingBlazorApplicationBuilder : IApplicationBuilder
{
    private readonly string[] _args;

    /// <summary>
    /// Gets the service collection used to register services for dependency injection.
    /// </summary>
    /// <value>
    /// An <see cref="IServiceCollection"/> instance that allows registration of application services,
    /// configurations, and dependencies that will be available throughout the application lifecycle.
    /// Services registered in this collection will be resolved through the <see cref="IServiceProvider"/>
    /// created during the <see cref="Build"/> operation.
    /// </value>
    /// <remarks>
    /// Use this property to register services, configurations, and other dependencies before calling <see cref="Build"/>.
    /// The service collection is initialized in the constructor and remains available until the builder is built.
    /// </remarks>
    /// <seealso cref="Build"/>
    /// <seealso cref="IServiceCollection"/>
    public IServiceCollection Services { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgingBlazorApplicationBuilder"/> class with the specified command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application. Cannot be <see langword="null"/>.</param>
    /// <remarks>
    /// <para>
    /// This constructor initializes a new <see cref="ServiceCollection"/> for dependency injection and stores
    /// the command-line arguments for later use when creating the <see cref="ForgingBlazorApplication"/> instance.
    /// </para>
    /// <para>
    /// It is recommended to use the static factory methods <see cref="CreateDefaultBuilder(string[])"/> or
    /// <see cref="CreateEmptyBuilder(string[])"/> instead of calling this constructor directly.
    /// </para>
    /// </remarks>
    /// <seealso cref="CreateDefaultBuilder(string[])"/>
    /// <seealso cref="CreateEmptyBuilder(string[])"/>
    public ForgingBlazorApplicationBuilder(string[] args)
    {
        _args = args;

        Services = new ServiceCollection();
    }

    /// <summary>
    /// Builds and returns a configured <see cref="IApplication"/> instance based on the current builder state.
    /// </summary>
    /// <returns>
    /// A fully configured <see cref="ForgingBlazorApplication"/> instance with all registered services available
    /// through dependency injection, ready to be run via <see cref="IApplication.RunAsync"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method constructs a <see cref="IServiceProvider"/> from the registered services using
    /// <see cref="ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(IServiceCollection)"/>
    /// and creates a new <see cref="ForgingBlazorApplication"/> with the command-line arguments and service provider.
    /// </para>
    /// <para>
    /// Call this method once after all service configuration is complete. After calling <see cref="Build"/>,
    /// the builder should not be reused, and any further service registrations will not affect the created application.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there are service configuration errors or dependency resolution failures.
    /// </exception>
    /// <seealso cref="Services"/>
    /// <seealso cref="ForgingBlazorApplication"/>
    /// <seealso cref="IApplication.RunAsync"/>
    public IApplication Build()
    {
        // Check if logging is already registered
        if (!Services.IsServiceTypeRegistered<ILoggerFactory>())
        {
            // Register NullLoggerFactory and NullLogger<T> as defaults
            _ = Services
                .AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance)
                .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        }

        var serviceProvider = Services.BuildServiceProvider();

        return new ForgingBlazorApplication(_args, serviceProvider);
    }

    /// <summary>
    /// Creates a new <see cref="ForgingBlazorApplicationBuilder"/> instance with the specified command-line arguments
    /// and registers default services required for ForgingBlazor applications.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application. Cannot be <see langword="null"/>.</param>
    /// <returns>
    /// A new <see cref="IApplicationBuilder"/> instance ready for additional service configuration,
    /// with default ForgingBlazor services already registered.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This is the recommended entry point for creating a ForgingBlazor application builder with standard configuration.
    /// The method creates a new builder instance and registers all default services required by the ForgingBlazor framework.
    /// </para>
    /// <para>
    /// After calling this method, you can configure additional services using the <see cref="IApplicationBuilder.Services"/> property,
    /// then call <see cref="Build"/> to create the application instance.
    /// </para>
    /// <para>
    /// If you need a minimal builder without default services, use <see cref="CreateEmptyBuilder(string[])"/> instead.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = ForgingBlazorApplicationBuilder.CreateDefaultBuilder(args);
    /// builder.Services.AddSingleton&lt;ICustomService, CustomService&gt;();
    /// var app = builder.Build();
    /// await app.RunAsync();
    /// </code>
    /// </example>
    /// <seealso cref="CreateEmptyBuilder(string[])"/>
    /// <seealso cref="Build"/>
    /// <seealso cref="IApplicationBuilder"/>
    public static IApplicationBuilder CreateDefaultBuilder(string[] args)
    {
        var builder = new ForgingBlazorApplicationBuilder(args);

        // Add default services required for ForgingBlazor applications

        return builder;
    }

    /// <summary>
    /// Creates a new <see cref="ForgingBlazorApplicationBuilder"/> instance with the specified command-line arguments
    /// without registering any default services.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application. Cannot be <see langword="null"/>.</param>
    /// <returns>
    /// A new <see cref="IApplicationBuilder"/> instance with an empty service collection,
    /// ready for manual service configuration.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method creates a minimal builder without any pre-registered services, giving you full control
    /// over the service configuration. Use this when you need complete control over which services are registered
    /// or when the default services provided by <see cref="CreateDefaultBuilder(string[])"/> are not required.
    /// </para>
    /// <para>
    /// After calling this method, you must manually register all required services using the
    /// <see cref="IApplicationBuilder.Services"/> property before calling <see cref="Build"/>.
    /// </para>
    /// <para>
    /// For standard ForgingBlazor applications with default services, prefer using <see cref="CreateDefaultBuilder(string[])"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = ForgingBlazorApplicationBuilder.CreateEmptyBuilder(args);
    /// // Manually register all required services
    /// builder.Services.AddSingleton&lt;IMyService, MyService&gt;();
    /// var app = builder.Build();
    /// await app.RunAsync();
    /// </code>
    /// </example>
    /// <seealso cref="CreateDefaultBuilder(string[])"/>
    /// <seealso cref="Build"/>
    /// <seealso cref="IApplicationBuilder"/>
    internal static IApplicationBuilder CreateEmptyBuilder(string[] args) => new ForgingBlazorApplicationBuilder(args);
}
