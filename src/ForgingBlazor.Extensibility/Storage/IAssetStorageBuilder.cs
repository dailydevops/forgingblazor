namespace NetEvolve.ForgingBlazor;

using System;

/// <summary>
/// Provides configuration for selecting and configuring an asset storage provider.
/// </summary>
public interface IAssetStorageBuilder
{
    /// <summary>
    /// Uses a file system-based storage provider for assets.
    /// </summary>
    /// <param name="configure">The configuration callback for file system storage options.</param>
    /// <returns>The <see cref="IAssetStorageBuilder"/> for chaining.</returns>
    IAssetStorageBuilder UseFileSystem(Action<IFileSystemStorageOptions> configure);
}
