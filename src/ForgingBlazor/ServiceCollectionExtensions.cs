namespace NetEvolve.ForgingBlazor;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
    /// Registers a configuration of type <typeparamref name="TConfiguration"/> with validation of type <typeparamref name="TValidation"/>.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type to register. Must be a reference type.</typeparam>
    /// <typeparam name="TValidation">The validation type that implements <see cref="IConfigureOptions{TOptions}"/> and <see cref="IValidateOptions{TOptions}"/>. Must be a reference type.</typeparam>
    /// <param name="services">The service collection to register the configuration with.</param>
    /// <returns>The <see cref="IServiceCollection"/> for method chaining.</returns>
    internal static IServiceCollection RegisterConfiguration<TConfiguration, TValidation>(
        this IServiceCollection services
    )
        where TConfiguration : class
        where TValidation : class, IConfigureOptions<TConfiguration>, IValidateOptions<TConfiguration>
    {
        _ = services
            .ConfigureOptions<TValidation>()
            .AddOptionsWithValidateOnStart<TConfiguration, TValidation>()
            .ValidateDataAnnotations();

        return services;
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
