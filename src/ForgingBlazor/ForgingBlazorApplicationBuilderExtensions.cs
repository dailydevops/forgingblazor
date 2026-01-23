namespace NetEvolve.ForgingBlazor;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Configurations;

/// <summary>
/// Provides extension methods for <see cref="IForgingBlazorApplicationBuilder"/> to configure hosting and application services.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "As designed.")]
[SuppressMessage(
    "Minor Code Smell",
    "S2325:Methods and properties that don't access instance data should be static",
    Justification = "As designed."
)]
[SuppressMessage(
    "Major Code Smell",
    "S125:Sections of code should not be commented out",
    Justification = "As designed."
)]
public static class ForgingBlazorApplicationBuilderExtensions
{
    /// <summary>
    /// Adds hosting services required for running a Blazor application, including Razor pages, Razor components, and SignalR.
    /// </summary>
    /// <param name="builder">The ForgingBlazor application builder to configure.</param>
    /// <returns>The <see cref="IForgingBlazorApplicationBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the builder is not of type <see cref="ForgingBlazorApplicationBuilder"/> or when hosting services have already been registered.</exception>
    public static IForgingBlazorApplicationBuilder AddHostingServices(this IForgingBlazorApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (builder is not ForgingBlazorApplicationBuilder forgingBlazorApplicationBuilder)
        {
            // BUG: If we call ThrowInvalidBuilderTypeException() here, the compiler complains that it cannot prove
            // that the method will not return, even though it is annotated with [DoesNotReturn].
            // ThrowInvalidBuilderTypeException();
            throw new InvalidOperationException(
                $"The {nameof(AddHostingServices)} extension method can only be used with the `{nameof(ForgingBlazorApplicationBuilder)}` type."
            );
        }

        var services = forgingBlazorApplicationBuilder.Services;

        services.ThrowIfAlreadyRegistered<MarkerHostingServicesRegistered>();

        _ = services.AddRazorPages();
        _ = services.AddRazorComponents().AddInteractiveServerComponents();

        _ = services.AddSignalR(o => o.StreamBufferCapacity = 1024 * 1024);

        return builder;
    }

    /// <summary>
    /// Adds configuration services and validation for ForgingBlazor application settings.
    /// </summary>
    /// <param name="builder">The ForgingBlazor application builder to configure.</param>
    /// <returns>The <see cref="IForgingBlazorApplicationBuilder"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the builder is not of type <see cref="ForgingBlazorApplicationBuilder"/> or when configuration services have already been registered.</exception>
    public static IForgingBlazorApplicationBuilder AddConfigurations(this IForgingBlazorApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        if (builder is not ForgingBlazorApplicationBuilder forgingBlazorApplicationBuilder)
        {
            // BUG: If we call ThrowInvalidBuilderTypeException() here, the compiler complains that it cannot prove
            // that the method will not return, even though it is annotated with [DoesNotReturn].
            // ThrowInvalidBuilderTypeException();
            throw new InvalidOperationException(
                $"The {nameof(AddConfigurations)} extension method can only be used with the `{nameof(ForgingBlazorApplicationBuilder)}` type."
            );
        }

        var services = forgingBlazorApplicationBuilder.Services;

        services.ThrowIfAlreadyRegistered<MarkerConfigurationServicesRegistered>();

        _ = services
            .RegisterConfiguration<AdministrationConfig, AdministrationConfigValidation>() // Add AdministrationConfig
            .RegisterConfiguration<PaginationConfig, PaginationConfigValidation>() // Add PaginationConfig
            .RegisterConfiguration<SiteConfig, SiteConfigValidation>(); // Add SiteConfig

        return builder;
    }

    /// <summary>
    /// Throws an <see cref="InvalidOperationException"/> indicating that the extension method can only be used with <see cref="ForgingBlazorApplicationBuilder"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown to indicate invalid builder type.</exception>
    [DoesNotReturn]
    private static void ThrowInvalidBuilderTypeException() =>
        throw new InvalidOperationException(
            $"The {nameof(AddHostingServices)} extension method can only be used with the `{nameof(ForgingBlazorApplicationBuilder)}` type."
        );

    /// <summary>
    /// Marker type to prevent duplicate registration of hosting services.
    /// </summary>
    private sealed partial class MarkerHostingServicesRegistered;

    /// <summary>
    /// Marker type to prevent duplicate registration of configuration services.
    /// </summary>
    private sealed partial class MarkerConfigurationServicesRegistered;
}
