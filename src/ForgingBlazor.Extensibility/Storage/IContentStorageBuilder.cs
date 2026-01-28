namespace NetEvolve.ForgingBlazor;

using System;

/// <summary>
/// Provides configuration for selecting and configuring a content storage provider.
/// </summary>
public interface IContentStorageBuilder
{
    /// <summary>
    /// Uses a file system-based storage provider for content.
    /// </summary>
    /// <param name="configure">The configuration callback for file system storage options.</param>
    /// <returns>The <see cref="IContentStorageBuilder"/> for chaining.</returns>
    IContentStorageBuilder UseFileSystem(Action<IFileSystemStorageOptions> configure);
}
