namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

using System.Globalization;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NetEvolve.ForgingBlazor.Content.Parsing;

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
    public async Task<TDescriptor?> GetContentAsync<TDescriptor>(
        string segmentPath,
        string slug,
        CultureInfo culture,
        CancellationToken cancellationToken = default
    )
        where TDescriptor : ContentDescriptor, new()
    {
        ArgumentNullException.ThrowIfNull(segmentPath);
        ArgumentNullException.ThrowIfNull(slug);
        ArgumentNullException.ThrowIfNull(culture);

        var blobPath = BuildContentPath(segmentPath, slug, culture);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        if (!await blobClient.ExistsAsync(cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        var response = await blobClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
        var markdownContent = response.Value.Content.ToString();

        return ContentParser.Parse<TDescriptor>(markdownContent);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TDescriptor>> GetContentsAsync<TDescriptor>(
        string segmentPath,
        CultureInfo culture,
        CancellationToken cancellationToken = default
    )
        where TDescriptor : ContentDescriptor, new()
    {
        ArgumentNullException.ThrowIfNull(segmentPath);
        ArgumentNullException.ThrowIfNull(culture);

        var cultureFolder = GetCultureFolder(culture);
        var normalizedSegment = segmentPath.TrimStart('/');
        var prefix = $"{normalizedSegment}/{cultureFolder}/";
        var results = new List<TDescriptor>();

        await foreach (
            var blobItem in _containerClient
                .GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken)
                .ConfigureAwait(false)
        )
        {
            if (blobItem.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                var blobClient = _containerClient.GetBlobClient(blobItem.Name);
                var response = await blobClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
                var markdownContent = response.Value.Content.ToString();
                var descriptor = ContentParser.Parse<TDescriptor>(markdownContent);
                results.Add(descriptor);
            }
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
    public async Task SaveContentAsync(string path, string markdown, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(markdown);

        var blobClient = _containerClient.GetBlobClient(path);

        var bytes = Encoding.UTF8.GetBytes(markdown);
        using var stream = new MemoryStream(bytes);

        _ = await blobClient
            .UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = "text/markdown" },
                cancellationToken: cancellationToken
            )
            .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteContentAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);

        var blobClient = _containerClient.GetBlobClient(path);
        _ = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static string BuildContentPath(string segmentPath, string slug, CultureInfo culture)
    {
        var cultureFolder = GetCultureFolder(culture);
        var normalizedSegment = segmentPath.TrimStart('/');
        var fileName = $"{slug}.md";
        return $"{normalizedSegment}/{cultureFolder}/{fileName}";
    }

    private static string GetCultureFolder(CultureInfo culture) => culture.Name.ToUpperInvariant();
}
