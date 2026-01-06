namespace NetEvolve.ForgingBlazor;

/// <summary>
/// Provides default constant values for configuration and validation across the ForgingBlazor application.
/// </summary>
public static class Defaults
{
    /// <summary>
    /// Defines the minimum allowed page size for pagination operations.
    /// </summary>
    /// <value>The minimum page size is 1.</value>
    public const int PageSizeMinimum = 1;

    /// <summary>
    /// Defines the maximum allowed page size for pagination operations.
    /// </summary>
    /// <value>The maximum page size is 100.</value>
    public const int PageSizeMaximum = 100;

    /// <summary>
    /// Defines the maximum allowed length for URL or route segments.
    /// </summary>
    /// <value>The maximum segment length is 70 characters.</value>
    public const int SegmentLengthMaximum = 70;

    /// <summary>
    /// Defines the minimum allowed length for URL or route segments.
    /// </summary>
    /// <value>The minimum segment length is 3 characters.</value>
    public const int SegmentLengthMinimum = 3;
}
