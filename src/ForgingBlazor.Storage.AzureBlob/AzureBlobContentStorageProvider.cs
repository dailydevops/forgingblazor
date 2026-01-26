using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NetEvolve.ForgingBlazor.Extensibility.Storage;

namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

/// <summary>
/// Azure Blob Storage implementation of <see cref="IContentStorageProvider"/>.
/// </summary>
internal sealed class AzureBlobContentStorageProvider : IContentStorageProvider
{
    private readonly BlobContainerClient _containerClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobContentStorageProvider"/> class.
    /// </summary>
    /// <param name="connectionString">The connection string for the Azure Storage account.</param>
    /// <param name="containerName">The name of the blob container.</param>
    public AzureBlobContentStorageProvider(string connectionString, string containerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerName);

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    /// <inheritdoc/>
    public async Task<string?> GetContentAsync(
        string segment,
        string slug,
        string culture,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);

        var blobPath = BuildBlobPath(segment, slug, culture);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        var response = await blobClient
            .DownloadContentAsync(cancellationToken)
            .ConfigureAwait(false);
        return response.Value.Content.ToString();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetContentsAsync(
        string segment,
        string? culture = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);

        var prefix = string.IsNullOrWhiteSpace(culture)
            ? $"{segment}/"
            : $"{segment}/{culture}/";
        var results = new List<string>();

        await foreach (
            var blobItem in _containerClient
                .GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken)
                .ConfigureAwait(false)
        )
        {
            if (blobItem.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(blobItem.Name);
            }
        }

        return results;
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(
        string segment,
        string slug,
        string culture,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);

        var blobPath = BuildBlobPath(segment, slug, culture);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        return await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SaveContentAsync(
        string segment,
        string slug,
        string culture,
        string content,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);
        ArgumentNullException.ThrowIfNull(content);

        var blobPath = BuildBlobPath(segment, slug, culture);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        var bytes = Encoding.UTF8.GetBytes(content);
        using var stream = new MemoryStream(bytes);

        await blobClient
            .UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = "text/markdown" },
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteContentAsync(
        string segment,
        string slug,
        string culture,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);

        var blobPath = BuildBlobPath(segment, slug, culture);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static string BuildBlobPath(string segment, string slug, string culture) =>
        $"{segment}/{culture}/{slug}.md";
}
