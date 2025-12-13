namespace NetEvolve.ForgingBlazor.Extensibility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to facilitate service management and transfer operations.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Cached <see cref="Type"/> reference for <see cref="IStartUpMarker"/> used for filtering services.
    /// </summary>
    private static readonly Type _typeStartUpMarker = typeof(IStartUpMarker);

    /// <summary>
    /// Cached <see cref="Type"/> reference for <see cref="IEnumerable{ServiceDescriptor}"/> used for filtering services.
    /// </summary>
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
