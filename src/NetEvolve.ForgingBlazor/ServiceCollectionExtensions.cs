namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;
using NetEvolve.ForgingBlazor.Services;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register and configure ForgingBlazor services.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core ForgingBlazor services including CLI commands and content registration.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to register services into.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method registers all core ForgingBlazor services, including the root command (<see cref="CommandCli"/>),
    /// sub-commands (build, create, example, serve), and the content register service.
    /// </para>
    /// <para>
    /// If services have already been registered (identified by the presence of the <see cref="MarkerCoreServices"/> marker),
    /// the method returns immediately without registering duplicate services.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <see langword="null"/>.</exception>
    internal static IServiceCollection AddForgingBlazorServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (!services.IsServiceTypeRegistered<MarkerCoreServices>())
        {
            _ = services
                .AddSingleton<MarkerCoreServices>()
                // Register RootCommand
                .AddSingleton<RootCommand, CommandCli>()
                // Register all Standard Commands
                .AddSingleton<Command, CommandBuild>()
                .AddSingleton<Command, CommandCreate>()
                .AddSingleton<Command, CommandExample>()
                .AddSingleton<Command, CommandServe>()
                // Register core services
                .AddSingleton<IContentRegister, ContentRegister>();
        }

        return services;
    }

    /// <summary>
    /// Internal marker class used to track that core ForgingBlazor services have been registered.
    /// </summary>
    /// <remarks>
    /// This sealed class implements <see cref="IStartUpMarker"/> to prevent duplicate service registrations
    /// and identify startup-specific services during the filtering process.
    /// </remarks>
    private sealed class MarkerCoreServices : IStartUpMarker;

    /// <summary>
    /// Determines whether a specific service type has been registered in the service collection.
    /// </summary>
    /// <typeparam name="T">The service type to check for registration.</typeparam>
    /// <param name="builder">
    /// The <see cref="IServiceCollection"/> to check for service registration.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the service type <typeparamref name="T"/> is registered; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This is a utility method for checking whether a service type has already been registered,
    /// commonly used to prevent duplicate service registrations.
    /// </remarks>
    internal static bool IsServiceTypeRegistered<T>(this IServiceCollection builder)
        where T : class => builder.Any(x => x.ServiceType == typeof(T));
}
