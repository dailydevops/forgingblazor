namespace NetEvolve.ForgingBlazor;

using System;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Routing;

/// <summary>
/// Provides extension methods for registering dynamic content routing.
/// </summary>
public static class RoutingBuilderExtensions
{
    /// <summary>
    /// Adds dynamic content routing configuration to the application builder.
    /// </summary>
    /// <param name="builder">The ForgingBlazor application builder.</param>
    /// <param name="configure">The configuration delegate for routing.</param>
    /// <returns>The original <see cref="IForgingBlazorApplicationBuilder"/> to support chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="configure"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the extension is invoked on an unsupported builder type or when routing has already been configured.</exception>
    public static IForgingBlazorApplicationBuilder AddRouting(
        this IForgingBlazorApplicationBuilder builder,
        Action<IRoutingBuilder> configure
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        if (builder is not ForgingBlazorApplicationBuilder forgingBlazorApplicationBuilder)
        {
            throw new InvalidOperationException(
                $"The {nameof(AddRouting)} extension method can only be used with the `{nameof(ForgingBlazorApplicationBuilder)}` type."
            );
        }

        var services = forgingBlazorApplicationBuilder.Services;
        services.ThrowIfAlreadyRegistered<MarkerRoutingServicesRegistered>();

        var routingBuilder = new RoutingBuilder();
        configure(routingBuilder);

        var configuration = routingBuilder.Build();
        _ = services.AddSingleton(configuration);

        return builder;
    }

    /// <summary>
    /// Marker type used to ensure routing services are not registered multiple times.
    /// </summary>
    private sealed partial class MarkerRoutingServicesRegistered;
}
