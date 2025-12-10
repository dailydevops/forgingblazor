namespace NetEvolve.ForgingBlazor;

using NetEvolve.ForgingBlazor.Core.Models;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides extension methods for <see cref="NetEvolve.ForgingBlazor.Extensibility.Abstractions.IApplicationBuilder"/> to configure default page handling.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the application with default pages using the standard <see cref="NetEvolve.ForgingBlazor.Core.Models.Page"/> implementation.
    /// This method registers the necessary services and sets up the default page infrastructure for the application.
    /// </summary>
    /// <param name="builder">The <see cref="NetEvolve.ForgingBlazor.Extensibility.Abstractions.IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The same <see cref="NetEvolve.ForgingBlazor.Extensibility.Abstractions.IApplicationBuilder"/> instance for method chaining.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static IApplicationBuilder WithDefaultPages(this IApplicationBuilder builder) =>
        builder.WithDefaultPages<Page>();

    /// <summary>
    /// Configures the application with default pages using a custom page type that derives from <see cref="NetEvolve.ForgingBlazor.Extensibility.Models.PageBase"/>.
    /// This generic method allows you to specify a custom page implementation while automatically registering all required ForgingBlazor services.
    /// The method validates the builder argument and ensures the service collection is properly configured before returning.
    /// </summary>
    /// <typeparam name="TPageType">The custom page type that must inherit from <see cref="NetEvolve.ForgingBlazor.Extensibility.Models.PageBase"/>.</typeparam>
    /// <param name="builder">The <see cref="NetEvolve.ForgingBlazor.Extensibility.Abstractions.IApplicationBuilder"/> instance to configure.</param>
    /// <returns>The same <see cref="NetEvolve.ForgingBlazor.Extensibility.Abstractions.IApplicationBuilder"/> instance for method chaining.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    public static IApplicationBuilder WithDefaultPages<TPageType>(this IApplicationBuilder builder)
        where TPageType : PageBase
    {
        ArgumentNullException.ThrowIfNull(builder);
        _ = builder.Services.AddForgingBlazorServices();
        return builder;
    }
}
