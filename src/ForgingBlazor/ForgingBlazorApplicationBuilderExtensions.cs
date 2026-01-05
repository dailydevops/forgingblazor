namespace NetEvolve.ForgingBlazor;

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Configurations;
using NetEvolve.ForgingBlazor.Extensibility;

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
    extension(IForgingBlazorApplicationBuilder builder)
    {
        /// <summary>
        /// Adds hosting services required for running a Blazor application, including Razor pages, Razor components, and SignalR.
        /// </summary>
        /// <returns>The <see cref="IForgingBlazorApplicationBuilder"/> for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the builder is not of type <see cref="ForgingBlazorApplicationBuilder"/> or when hosting services have already been registered.</exception>
        public IForgingBlazorApplicationBuilder AddHostingServices()
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

            _ = services.AddSingleton<MarkerHostingServicesRegistered>().AddRazorPages();
            _ = services.AddRazorComponents().AddInteractiveServerComponents();

            _ = services.AddSignalR(o => o.StreamBufferCapacity = 1024 * 1024);

            return builder;
        }

        /// <summary>
        /// Adds configuration services and validation for ForgingBlazor application settings.
        /// </summary>
        /// <returns>The <see cref="IForgingBlazorApplicationBuilder"/> for method chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the builder is not of type <see cref="ForgingBlazorApplicationBuilder"/> or when configuration services have already been registered.</exception>
        public IForgingBlazorApplicationBuilder AddConfigurations()
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

            services.ThrowIfAlreadyRegistered<MarkerConfigurationServicesRegistered>();

            _ = services
            // Marker to prevent multiple registrations
            .AddSingleton<MarkerConfigurationServicesRegistered>();

            // Add AdministrationConfig
            _ = services
                .AddOptionsWithValidateOnStart<AdministrationConfig, AdministrationConfigValidation>()
                .ValidateDataAnnotations();

            return builder;
        }
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
