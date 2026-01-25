namespace NetEvolve.ForgingBlazor;

/// <summary>
/// Options for configuring file system-backed content and asset storage providers.
/// </summary>
public interface IFileSystemStorageOptions
{
    /// <summary>
    /// Sets the base path on the file system used for storage.
    /// </summary>
    /// <param name="path">The base directory path.</param>
    /// <returns>The <see cref="IFileSystemStorageOptions"/> for chaining.</returns>
    IFileSystemStorageOptions WithBasePath(string path);

    /// <summary>
    /// Enables or disables change monitoring for file system-backed storage.
    /// </summary>
    /// <returns>The <see cref="IFileSystemStorageOptions"/> for chaining.</returns>
    IFileSystemStorageOptions WatchForChanges();
}
