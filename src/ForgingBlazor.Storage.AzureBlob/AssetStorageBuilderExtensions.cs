namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

/// <summary>
/// Extension methods for <see cref="IAssetStorageBuilder"/> to configure Azure Blob Storage.
/// </summary>
public static class AssetStorageBuilderExtensions
{
    /// <summary>
    /// Configures Azure Blob Storage as the asset storage provider.
    /// </summary>
    /// <param name="builder">The asset storage builder.</param>
    /// <param name="configure">The configuration action for Azure Blob Storage options.</param>
    /// <returns>The asset storage builder for method chaining.</returns>
    public static IAssetStorageBuilder UseAzureBlobStorage(
        this IAssetStorageBuilder builder,
        Action<IAzureBlobStorageOptions> configure
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new AzureBlobStorageOptions();
        configure(options);

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException("Connection string must be configured using WithConnectionString().");
        }

        if (string.IsNullOrWhiteSpace(options.ContainerName))
        {
            throw new InvalidOperationException("Container name must be configured using WithContainerName().");
        }

        // Note: Storage builder registration implementation depends on Phase 4 infrastructure
        // This method signature is complete; DI registration happens in ForgingBlazor project
        return builder;
    }
}
