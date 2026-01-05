namespace NetEvolve.ForgingBlazor.Configurations;

/// <summary>
/// Configuration settings for the administration area of the ForgingBlazor application.
/// </summary>
internal sealed class AdministrationConfig
{
    /// <summary>
    /// Gets or sets a value indicating whether the administration area is enabled.
    /// </summary>
    /// <value>The default value is <see langword="true" />.</value>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the URL path segment used to access the administration area.
    /// </summary>
    /// <value>The default value is "_admin".</value>
    public string PathSegment { get; set; } = "_admin";
}
