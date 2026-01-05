namespace NetEvolve.ForgingBlazor.Configurations;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents pagination settings used to format and size paged endpoints.
/// </summary>
public sealed class PaginationConfig
{
    /// <summary>
    /// Gets or sets the routing mode that controls how page links are composed.
    /// </summary>
    [DefaultValue(PaginationMode.Default)]
    public PaginationMode Mode { get; set; } = PaginationMode.Default;

    /// <summary>
    /// Gets or sets the number of items returned per page.
    /// </summary>
    [Range(1, 100)]
    [DefaultValue(10)]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets an optional custom base path for paginated routes.
    /// </summary>
    [MaxLength(70)]
    public string? Path { get; set; }
}
