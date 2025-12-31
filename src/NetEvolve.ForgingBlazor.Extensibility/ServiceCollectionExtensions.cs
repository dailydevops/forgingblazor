namespace NetEvolve.ForgingBlazor.Extensibility;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to facilitate service management and transfer operations.
/// </summary>
/// <remarks>
/// <para>
/// This static class offers utility methods for transferring services between service providers,
/// particularly for propagating services from a parent container to child scopes while filtering
/// out startup-specific and transient enumerable services.
/// </para>
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Cached <see cref="Type"/> reference for <see cref="IStartUpMarker"/> used for filtering services.
    /// </summary>
    /// <remarks>
    /// This field caches the type reference to improve performance during service filtering operations
    /// by avoiding repeated reflection calls.
    /// </remarks>
    private static readonly Type _typeStartUpMarker = typeof(IStartUpMarker);

    /// <summary>
    /// Cached <see cref="Type"/> reference for <see cref="IEnumerable{ServiceDescriptor}"/> used for filtering services.
    /// </summary>
    /// <remarks>
    /// This field caches the type reference to improve performance during service filtering operations
    /// by avoiding repeated reflection calls. It is used to filter out the enumerable of service descriptors
    /// that is passed between providers.
    /// </remarks>
    private static readonly Type _typeEnumerableServiceDescriptor = typeof(IEnumerable<ServiceDescriptor>);

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
    /// The same <see cref="IServiceCollection"/> instance with transferred services added, enabling method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method transfers service descriptors from one service provider to a service collection,
    /// while filtering out services that:
    /// <list type="bullet">
    /// <item><description>Implement <see cref="IStartUpMarker"/> (startup-specific services)</description></item>
    /// <item><description>Have implementation types that implement <see cref="IStartUpMarker"/></description></item>
    /// <item><description>Are assignable to <see cref="IEnumerable{ServiceDescriptor}"/> (transient enumerable collections)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This is useful for propagating services from a parent container to child scopes without including startup-specific services
    /// or internal service descriptors that are used only for configuration.
    /// </para>
    /// <para>
    /// The method uses cached type references (_typeStartUpMarker and _typeEnumerableServiceDescriptor) to optimize performance
    /// during repeated filtering operations.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
    public static IServiceCollection TransferAllServices(
        this IServiceCollection services,
        IServiceProvider serviceProvider
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        foreach (var descriptors in serviceProvider.GetServices<ServiceDescriptor>())
        {
            var serviceType = descriptors.ServiceType;
            var implementationType = descriptors.ImplementationType;

            if (
                serviceType.IsAssignableTo(_typeStartUpMarker)
                || implementationType?.IsAssignableTo(_typeStartUpMarker) == true
                || serviceType.IsAssignableTo(_typeEnumerableServiceDescriptor)
                || implementationType?.IsAssignableTo(_typeEnumerableServiceDescriptor) == true
            )
            {
                continue;
            }

            services.Add(descriptors);
        }

        return services;
    }
}
