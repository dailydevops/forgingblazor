namespace NetEvolve.ForgingBlazor.Storage;

using System;

/// <summary>
/// Options for configuring file system storage.
/// </summary>
internal sealed class FileSystemStorageOptions : IFileSystemStorageOptions
{
    /// <summary>
    /// Gets the base path for file system storage.
    /// </summary>
    internal string BasePath { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether file system watching is enabled.
    /// </summary>
    internal bool EnableWatch { get; private set; }

    /// <inheritdoc />
    public IFileSystemStorageOptions WithBasePath(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Base path cannot be empty or whitespace.", nameof(path));
        }

        BasePath = path;
        return this;
    }

    /// <inheritdoc />
    public IFileSystemStorageOptions WatchForChanges()
    {
        EnableWatch = true;
        return this;
    }
}
