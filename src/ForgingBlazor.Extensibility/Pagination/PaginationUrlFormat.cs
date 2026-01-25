namespace NetEvolve.ForgingBlazor;

/// <summary>
/// Defines how pagination should be represented in URLs.
/// </summary>
public enum PaginationUrlFormat
{
    /// <summary>
    /// Uses numeric page segments (e.g., "/posts/2").
    /// </summary>
    Numeric,

    /// <summary>
    /// Uses a prefix for pagination (e.g., "/posts/page-2").
    /// </summary>
    Prefixed,
}
