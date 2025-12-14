namespace NetEvolve.ForgingBlazor.Extensibility;

/// <summary>
/// Provides default constant values for the
/// fully qualified type <c>NetEvolve.ForgingBlazor.Extensibility.Defaults</c>.
/// These values define the default application environment and common
/// relative paths used across the ForgingBlazor extensibility layer.
/// </summary>
public static class Defaults
{
    /// <summary>
    /// The default application environment name.
    /// </summary>
    public const string Environment = "Production";

    /// <summary>
    /// The default relative path for asset files.
    /// </summary>
    public const string PathAssets = "assets";
    /// <summary>
    /// The default relative path for content files.
    /// </summary>
    public const string PathContent = "content";
    /// <summary>
    /// The default relative path for data files.
    /// </summary>
    public const string PathData = "data";
    /// <summary>
    /// The default relative path to the static web root.
    /// </summary>
    public const string PathStatic = "wwwroot";
}
