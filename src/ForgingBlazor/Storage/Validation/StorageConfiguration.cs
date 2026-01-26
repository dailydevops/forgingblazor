namespace NetEvolve.ForgingBlazor.Storage.Validation;

/// <summary>
/// Represents storage configuration options.
/// </summary>
internal sealed class StorageConfiguration
{
    /// <summary>
    /// Gets or sets the content storage base path.
    /// </summary>
    public string? ContentStorageBasePath { get; set; }

    /// <summary>
    /// Gets or sets the asset storage base path.
    /// </summary>
    public string? AssetStorageBasePath { get; set; }
}
