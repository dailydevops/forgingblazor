namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

/// <summary>
/// Provides configuration options for Azure Blob Storage.
/// </summary>
public interface IAzureBlobStorageOptions
{
    /// <summary>
    /// Configures the connection string for Azure Blob Storage.
    /// </summary>
    /// <param name="connectionString">The connection string for the Azure Storage account.</param>
    /// <returns>The current instance for method chaining.</returns>
    IAzureBlobStorageOptions WithConnectionString(string connectionString);

    /// <summary>
    /// Configures the container name for Azure Blob Storage.
    /// </summary>
    /// <param name="containerName">The name of the blob container.</param>
    /// <returns>The current instance for method chaining.</returns>
    IAzureBlobStorageOptions WithContainerName(string containerName);

    /// <summary>
    /// Marks this storage provider as the publishing target.
    /// </summary>
    /// <returns>The current instance for method chaining.</returns>
    IAzureBlobStorageOptions AsPublishingTarget();
}
