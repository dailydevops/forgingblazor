namespace NetEvolve.ForgingBlazor.Storage;

using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring asset storage.
/// </summary>
public static class AssetStorageExtensions
{
    /// <summary>
    /// Adds asset storage configuration to the ForgingBlazor application.
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    /// <param name="configure">The configuration action for asset storage.</param>
    /// <returns>The application builder for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or <paramref name="configure"/> is <see langword="null"/>.</exception>
    public static IForgingBlazorApplicationBuilder AddAssetStorage(
        this IForgingBlazorApplicationBuilder builder,
        Action<IAssetStorageBuilder> configure
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var storageBuilder = new AssetStorageBuilder(builder.Services);
        configure(storageBuilder);

        return builder;
    }
}
