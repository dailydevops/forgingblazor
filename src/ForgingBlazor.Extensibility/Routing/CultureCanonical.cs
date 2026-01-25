namespace NetEvolve.ForgingBlazor;

/// <summary>
/// Determines whether canonical URLs include the culture as a prefix segment.
/// </summary>
public enum CultureCanonical
{
    /// <summary>
    /// Canonical URLs do not include a culture prefix (e.g., "/about").
    /// </summary>
    WithoutPrefix,

    /// <summary>
    /// Canonical URLs include a culture prefix (e.g., "/en/about").
    /// </summary>
    WithPrefix,
}
