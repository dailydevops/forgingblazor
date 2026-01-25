namespace NetEvolve.ForgingBlazor.Storage;

using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring content storage.
/// </summary>
public static class ContentStorageExtensions
{
    /// <summary>
    /// Adds content storage configuration to the ForgingBlazor application.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <param name="configure">The configuration action for content storage.</param>
    /// <returns>The application builder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="configure"/> is <see langword="null"/>.</exception>
    public static IForgingBlazorApplicationBuilder AddContentStorage(
        this IForgingBlazorApplicationBuilder builder,
        Action<IContentStorageBuilder> configure
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var storageBuilder = new ContentStorageBuilder(builder.Services);
        configure(storageBuilder);

        return builder;
    }
}
