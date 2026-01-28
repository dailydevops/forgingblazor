namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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
    public async Task<Stream> GetAssetAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var blobClient = _containerClient.GetBlobClient(path);

        if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            throw new FileNotFoundException($"Asset file not found: {path}", path);
        }

        var response = await blobClient
            .DownloadStreamingAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return response.Value.Content;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetAssetsAsync(
        string folder,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(folder);

        var normalizedFolder = folder.TrimStart('/');
        var prefix = string.IsNullOrEmpty(normalizedFolder) ? null : $"{normalizedFolder}/";
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
    public async Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var blobClient = _containerClient.GetBlobClient(path);
        return await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SaveAssetAsync(string path, Stream content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(content);

        var blobClient = _containerClient.GetBlobClient(path);

        var contentType = GetContentType(path);
        _ = await blobClient
            .UploadAsync(
                content,
                new BlobHttpHeaders { ContentType = contentType },
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteAssetAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var blobClient = _containerClient.GetBlobClient(path);
        _ = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToUpperInvariant();
        return extension switch
        {
            ".JPG" or ".JPEG" => "image/jpeg",
            ".PNG" => "image/png",
            ".GIF" => "image/gif",
            ".SVG" => "image/svg+xml",
            ".WEBP" => "image/webp",
            ".PDF" => "application/pdf",
            ".JSON" => "application/json",
            ".XML" => "application/xml",
            _ => "application/octet-stream",
        };
    }
}
