namespace NetEvolve.ForgingBlazor.Configurations;

using System.ComponentModel;

/// <summary>
/// Represents pagination settings used to format and size paged endpoints.
/// </summary>
public sealed class PaginationConfig
{
    /// <summary>
    /// Gets or sets the routing mode that controls how page links are composed.
    /// </summary>
    [DefaultValue(PaginationConfigMode.Default)]
    public PaginationConfigMode Mode { get; set; } = PaginationConfigMode.Default;

    /// <summary>
    /// Gets or sets the number of items returned per page.
    /// </summary>
    [DefaultValue(Defaults.PageSizeDefault)]
    public int PageSize { get; set; } = Defaults.PageSizeDefault;

    /// <summary>
    /// Gets or sets an optional custom base path for paginated routes.
    /// </summary>
    public string? Path { get; set; }
}
