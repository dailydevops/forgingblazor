using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NetEvolve.ForgingBlazor.Extensibility.Storage;

namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

/// <summary>
/// Azure Blob Storage implementation of <see cref="IAssetStorageProvider"/>.
/// </summary>
internal sealed class AzureBlobAssetStorageProvider : IAssetStorageProvider
{
    private readonly BlobContainerClient _containerClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobAssetStorageProvider"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string for the Azure Storage account.</param>
    /// <param name="containerName">The name of the blob container.</param>
    public AzureBlobAssetStorageProvider(string connectionString, string containerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    /// <inheritdoc/>
    public async Task<byte[]?> GetAssetAsync(
        string path,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var blobClient = _containerClient.GetBlobClient(path);

        if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        var response = await blobClient
            .DownloadContentAsync(cancellationToken)
            .ConfigureAwait(false);
        return response.Value.Content.ToArray();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetAssetsAsync(
        string? prefix = null,
        CancellationToken cancellationToken = default
    )
    {
        var results = new List<string>();

        await foreach (
            var blobItem in _containerClient
                .GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken)
                .ConfigureAwait(false)
        )
        {
            results.Add(blobItem.Name);
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(
        string path,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var blobClient = _containerClient.GetBlobClient(path);
        return await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SaveAssetAsync(
        string path,
        byte[] content,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(content);

        var blobClient = _containerClient.GetBlobClient(path);

        using var stream = new MemoryStream(content);

        var contentType = GetContentType(path);
        await blobClient
            .UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteAssetAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var blobClient = _containerClient.GetBlobClient(path);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".json" => "application/json",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }
}
