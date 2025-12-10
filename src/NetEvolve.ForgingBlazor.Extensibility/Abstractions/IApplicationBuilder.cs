namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides a builder pattern for configuring and constructing an <see cref="IApplication"/> instance.
/// </summary>
/// <remarks>
/// This interface follows the builder pattern to allow fluent configuration of services and application settings
/// before creating the final application instance. Use the <see cref="Services"/> property to register services
/// in the dependency injection container, then call <see cref="Build"/> to create the configured application.
/// </remarks>
public interface IApplicationBuilder
{
    /// <summary>
    /// Builds and returns a configured <see cref="IApplication"/> instance based on the current builder state.
    /// </summary>
    /// <returns>A fully configured application instance ready to be run.</returns>
    /// <remarks>
    /// This method should typically be called once after all configuration is complete.
    /// The returned application will have access to all services registered in the <see cref="Services"/> collection.
    /// </remarks>
    IApplication Build();

    /// <summary>
    /// Gets the service collection used to register services for dependency injection.
    /// </summary>
    /// <value>
    /// An <see cref="IServiceCollection"/> instance that allows registration of application services,
    /// configurations, and dependencies that will be available throughout the application lifecycle.
    /// </value>
    IServiceCollection Services { get; }
}
