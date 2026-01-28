namespace NetEvolve.ForgingBlazor;

using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a storage provider for content descriptors and their Markdown sources.
/// </summary>
public interface IContentStorageProvider
{
    /// <summary>
    /// Retrieves a specific content item by segment path, slug, and culture.
    /// </summary>
    /// <typeparam name="TDescriptor">The content descriptor type.</typeparam>
    /// <param name="segmentPath">The segment path (e.g., "/blog").</param>
    /// <param name="slug">The content slug.</param>
    /// <param name="culture">The target culture.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>The content descriptor if found; otherwise <c>null</c>.</returns>
    Task<TDescriptor?> GetContentAsync<TDescriptor>(
        string segmentPath,
        string slug,
        CultureInfo culture,
        CancellationToken cancellationToken = default
    )
        where TDescriptor : ContentDescriptor, new();

    /// <summary>
    /// Retrieves all content items under a segment for a given culture.
    /// </summary>
    /// <typeparam name="TDescriptor">The content descriptor type.</typeparam>
    /// <param name="segmentPath">The segment path (e.g., "/blog").</param>
    /// <param name="culture">The target culture.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A read-only list of content descriptors.</returns>
    Task<IReadOnlyList<TDescriptor>> GetContentsAsync<TDescriptor>(
        string segmentPath,
        CultureInfo culture,
        CancellationToken cancellationToken = default
    )
        where TDescriptor : ContentDescriptor, new();

    /// <summary>
    /// Checks whether a path exists in content storage.
    /// </summary>
    /// <param name="path">The content path.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns><c>true</c> if the path exists; otherwise <c>false</c>.</returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a Markdown source document to the specified path.
    /// </summary>
    /// <param name="path">The content path.</param>
    /// <param name="markdown">The Markdown content to save.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    Task SaveContentAsync(string path, string markdown, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes content at the specified path.
    /// </summary>
    /// <param name="path">The content path.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    Task DeleteContentAsync(string path, CancellationToken cancellationToken = default);
}
