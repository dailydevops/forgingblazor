namespace NetEvolve.ForgingBlazor.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides extension methods for <see cref="IApplicationBuilder"/> to configure and integrate logging services
/// into the Forging Blazor application pipeline.
/// </summary>
/// <remarks>
/// This class offers a fluent API for adding logging capabilities to applications built with the Forging Blazor framework.
/// It provides both pre-configured default logging setups and flexible custom configurations to meet various logging requirements.
/// </remarks>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the application with default logging providers, including console and debug output.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance that represents the application being configured.
    /// This parameter serves as the entry point for adding logging services to the application's service collection.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance that was passed in, enabling method chaining
    /// and fluent configuration of additional application features.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method provides a convenient way to quickly set up logging with sensible defaults.
    /// It automatically configures both console and debug logging providers, which are suitable
    /// for most development and debugging scenarios.
    /// </para>
    /// <para>
    /// The console provider outputs log messages to the standard console output, while the debug provider
    /// writes to the debug output window in development environments and attached debuggers.
    /// </para>
    /// <para>
    /// For production scenarios or when specific logging providers are required, consider using the
    /// overload that accepts an <see cref="Action{ILoggingBuilder}"/> delegate for custom configuration.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var builder = ApplicationBuilder.Create();
    /// builder.WithLogging();
    /// </code>
    /// </example>
    /// <seealso cref="WithLogging(IApplicationBuilder, Action{ILoggingBuilder})"/>
    public static IApplicationBuilder WithLogging(this IApplicationBuilder builder) =>
        builder.WithLogging(configure => configure.AddConsole().AddDebug());

    /// <summary>
    /// Configures the application with custom logging providers using a flexible configuration delegate.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance that represents the application being configured.
    /// This parameter serves as the entry point for adding logging services to the application's service collection.
    /// </param>
    /// <param name="configure">
    /// An <see cref="Action{ILoggingBuilder}"/> delegate that provides fine-grained control over the logging configuration.
    /// This delegate receives an <see cref="ILoggingBuilder"/> instance that can be used to add logging providers,
    /// set minimum log levels, add filters, and perform other logging-related configurations.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance that was passed in, enabling method chaining
    /// and fluent configuration of additional application features.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method offers maximum flexibility for configuring logging in the Forging Blazor application.
    /// It allows developers to choose specific logging providers, configure log levels, add custom filters,
    /// and integrate third-party logging frameworks according to their application's requirements.
    /// </para>
    /// <para>
    /// The configuration delegate is executed immediately during the application builder's configuration phase,
    /// ensuring that logging services are properly registered before any application components attempt to use them.
    /// </para>
    /// <para>
    /// Common use cases include integrating structured logging providers (such as Serilog or NLog),
    /// configuring cloud-based logging services (like Application Insights or AWS CloudWatch),
    /// or setting up custom logging filters for performance optimization.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>Example with custom log levels and multiple providers:</para>
    /// <code>
    /// var builder = ApplicationBuilder.Create();
    /// builder.WithLogging(logging =>
    /// {
    ///     logging.SetMinimumLevel(LogLevel.Information);
    ///     logging.AddConsole();
    ///     logging.AddEventLog();
    ///     logging.AddFilter("Microsoft", LogLevel.Warning);
    /// });
    /// </code>
    /// </example>
    /// <seealso cref="WithLogging(IApplicationBuilder)"/>
    /// <seealso cref="ILoggingBuilder"/>
    /// <seealso cref="LoggingServiceCollectionExtensions.AddLogging(IServiceCollection, Action{ILoggingBuilder})"/>
    public static IApplicationBuilder WithLogging(this IApplicationBuilder builder, Action<ILoggingBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(builder);

        _ = builder.Services.AddLogging(configure);

        return builder;
    }
}
