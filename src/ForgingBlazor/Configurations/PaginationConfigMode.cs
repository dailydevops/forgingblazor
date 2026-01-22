namespace NetEvolve.ForgingBlazor.Configurations;

/// <summary>
/// Defines how paginated routes are formatted when generating navigation links.
/// </summary>
public enum PaginationConfigMode
{
    /// <summary>
    /// Uses the default format without a prefix or folder: <c>/{0}</c>.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Adds a prefix to the page segment: <c>/posts-{0}</c>.
    /// </summary>
    Prefix,

    /// <summary>
    /// Places the page segment in a dedicated folder path: <c>/posts/{0}</c>.
    /// </summary>
    Folder,
}
