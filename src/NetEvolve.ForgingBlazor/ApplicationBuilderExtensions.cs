namespace NetEvolve.ForgingBlazor;

using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Core.Models;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;
using NetEvolve.ForgingBlazor.Services;

/// <summary>
/// Provides extension methods for <see cref="IApplicationBuilder"/> to configure default page handling.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the application with default pages using the standard <see cref="Page"/> implementation.
    /// This method registers the necessary services and sets up the default page infrastructure for the application.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static IApplicationBuilder WithDefaultPages(this IApplicationBuilder builder) =>
        builder.WithDefaultPages<Page>();

    /// <summary>
    /// Configures the application with default pages using a custom page type that derives from <see cref="PageBase"/>.
    /// This generic method allows you to specify a custom page implementation while automatically registering all required ForgingBlazor services.
    /// The method validates the builder argument and ensures the service collection is properly configured before returning.
    /// </summary>
    /// <typeparam name="TPageType">The custom page type that must inherit from <see cref="PageBase"/>.</typeparam>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static IApplicationBuilder WithDefaultPages<TPageType>(this IApplicationBuilder builder)
        where TPageType : PageBase
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Services.IsServiceTypeRegistered<DefaultContentMarker>())
        {
            throw new InvalidOperationException(
                "Default pages have already been registered. Multiple registrations are not allowed."
            );
        }

        _ = builder
            .Services.AddForgingBlazorServices()
            .AddSingleton<DefaultContentMarker>()
            .AddSingleton<IContentRegistration, DefaultContentRegistration<TPageType>>();

        return builder;
    }

    private sealed class DefaultContentMarker : IStartUpMarker;
}
