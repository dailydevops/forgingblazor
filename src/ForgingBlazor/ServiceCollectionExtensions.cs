namespace NetEvolve.ForgingBlazor;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to support service registration validation.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Validates that a marker type has not already been registered in the service collection.
    /// </summary>
    /// <typeparam name="TMarker">The marker type used to identify a set of registered services.</typeparam>
    /// <param name="services">The service collection to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown when the marker type has already been registered.</exception>
    internal static void ThrowIfAlreadyRegistered<TMarker>(this IServiceCollection services)
    {
        var typeOfTMarker = typeof(TMarker);
        var alreadyRegistered = services.Any(sd => sd.ServiceType == typeOfTMarker);
        if (alreadyRegistered)
        {
            ThrowAlreadyRegistered(typeOfTMarker.Name);
        }
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> indicating that services have already been registered.
    /// </summary>
    /// <param name="markerTypeName">The name of the marker type that was already registered.</param>
    /// <exception cref="InvalidOperationException">Always thrown to indicate duplicate registration.</exception>
    [DoesNotReturn]
    internal static void ThrowAlreadyRegistered(string markerTypeName) =>
        throw new InvalidOperationException(
            $"The services have already been registered. The marker type is '{markerTypeName}'."
        );
}
