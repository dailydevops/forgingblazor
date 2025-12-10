namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
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
                // Register all Commands
                .AddSingleton<RootCommand, CommandCli>()
                .AddSingleton<Command, CommandBuild>()
                .AddSingleton<Command, CommandCreate>()
                .AddSingleton<Command, CommandExample>()
                .AddSingleton<Command, CommandServe>()
                // Register core services
                .AddSingleton<IContentRegister, ForgingBlazorContentRegister>();
        }

        return services;
    }

    /// <summary>
    /// Transfers all non-startup services from one service provider to a service collection, excluding startup markers and enumerables.
    /// </summary>
    /// <param name="services">
    /// The target <see cref="IServiceCollection"/> to transfer services into.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="serviceProvider">
    /// The source <see cref="IServiceProvider"/> containing the service descriptors to transfer.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance with transferred services added.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method transfers service descriptors from one service provider to a service collection,
    /// while filtering out services that implement <see cref="IStartUpMarker"/> or are enumerable of <see cref="ServiceDescriptor"/>.
    /// This is useful for propagating services from a parent container to child scopes without including startup-specific services.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
    internal static IServiceCollection TransferAllServices(
        this IServiceCollection services,
        IServiceProvider serviceProvider
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var typeStartUpMarker = typeof(IStartUpMarker);
        var typeEnumerableServiceDescriptor = typeof(IEnumerable<ServiceDescriptor>);

        foreach (var descriptors in serviceProvider.GetServices<ServiceDescriptor>())
        {
            var serviceType = descriptors.ServiceType;
            var implementationType = descriptors.ImplementationType;

            if (
                serviceType.IsAssignableTo(typeStartUpMarker)
                || implementationType?.IsAssignableTo(typeStartUpMarker) == true
                || serviceType.IsAssignableTo(typeEnumerableServiceDescriptor)
                || implementationType?.IsAssignableTo(typeEnumerableServiceDescriptor) == true
            )
            {
                continue;
            }

            services.Add(descriptors);
        }

        return services;
    }

    /// <summary>
    /// Marker class used to identify that core ForgingBlazor services have been registered.
    /// </summary>
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
