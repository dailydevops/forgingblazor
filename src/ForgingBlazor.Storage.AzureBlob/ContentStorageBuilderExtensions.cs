namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

/// <summary>
/// Extension methods for <see cref="IContentStorageBuilder"/> to configure Azure Blob Storage.
/// </summary>
public static class ContentStorageBuilderExtensions
{
    /// <summary>
    /// Configures Azure Blob Storage as the content storage provider.
    /// </summary>
    /// <param name="builder">The content storage builder.</param>
    /// <param name="configure">The configuration action for Azure Blob Storage options.</param>
    /// <returns>The content storage builder for method chaining.</returns>
    public static IContentStorageBuilder UseAzureBlobStorage(
        this IContentStorageBuilder builder,
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
