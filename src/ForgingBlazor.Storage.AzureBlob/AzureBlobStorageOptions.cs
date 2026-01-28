namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

/// <summary>
/// Implementation of Azure Blob Storage configuration options.
/// </summary>
internal sealed class AzureBlobStorageOptions : IAzureBlobStorageOptions
{
    /// <summary>
    /// Gets the connection string for the Azure Storage account.
    /// </summary>
    public string? ConnectionString { get; private set; }

    /// <summary>
    /// Gets the name of the blob container.
    /// </summary>
    public string? ContainerName { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this storage provider is the publishing target.
    /// </summary>
    public bool IsPublishingTarget { get; private set; }

    /// <inheritdoc/>
    public IAzureBlobStorageOptions WithConnectionString(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ConnectionString = connectionString;
        return this;
    }

    /// <inheritdoc/>
    public IAzureBlobStorageOptions WithContainerName(string containerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);
        ContainerName = containerName;
        return this;
    }

    /// <inheritdoc/>
    public IAzureBlobStorageOptions AsPublishingTarget()
    {
        IsPublishingTarget = true;
        return this;
    }
}
