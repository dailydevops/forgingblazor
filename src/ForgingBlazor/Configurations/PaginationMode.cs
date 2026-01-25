namespace NetEvolve.ForgingBlazor.Configurations;

/// <summary>
/// Defines how paginated routes are formatted when generating navigation links.
/// </summary>
public enum PaginationMode
{
    /// <summary>
    /// Numeric format: /posts/2, /posts/3
    /// </summary>
    Numeric = 0,

    /// <summary>
    /// Prefixed format: /posts/page-2, /posts/page-3
    /// </summary>
    Prefixed,
}
