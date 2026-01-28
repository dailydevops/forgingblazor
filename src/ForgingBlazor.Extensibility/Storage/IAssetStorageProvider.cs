namespace NetEvolve.ForgingBlazor;

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Defines a storage provider for binary/static assets associated with content.
/// </summary>
public interface IAssetStorageProvider
{
    /// <summary>
    /// Gets a binary asset stream at the specified path.
    /// </summary>
    /// <param name="path">The asset path.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A readable <see cref="Stream"/> for the asset.</returns>
    Task<Stream> GetAssetAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists asset names under the specified folder.
    /// </summary>
    /// <param name="folder">The folder path.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A read-only list of asset identifiers (e.g., relative paths or file names).</returns>
    Task<IReadOnlyList<string>> GetAssetsAsync(string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether an asset exists at the specified path.
    /// </summary>
    /// <param name="path">The asset path.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns><c>true</c> if the asset exists; otherwise <c>false</c>.</returns>
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves an asset stream to the specified path.
    /// </summary>
    /// <param name="path">The asset path.</param>
    /// <param name="content">The asset content stream.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    Task SaveAssetAsync(string path, Stream content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the asset at the specified path.
    /// </summary>
    /// <param name="path">The asset path.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    Task DeleteAssetAsync(string path, CancellationToken cancellationToken = default);
}
