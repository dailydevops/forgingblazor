namespace NetEvolve.ForgingBlazor;

/// <summary>
/// Configures pagination behavior for routed content.
/// </summary>
public interface IPaginationConfiguration
{
    /// <summary>
    /// Sets the number of items per page.
    /// </summary>
    /// <param name="size">The page size (must be a positive integer).</param>
    /// <returns>The <see cref="IPaginationConfiguration"/> for chaining.</returns>
    IPaginationConfiguration PageSize(int size = Defaults.PageSizeDefault);

    /// <summary>
    /// Sets the URL format for pagination segments.
    /// </summary>
    /// <param name="format">The pagination URL format.</param>
    /// <param name="prefix">The optional prefix for pagination segments (used with <see cref="PaginationUrlFormat.Prefixed"/>).</param>
    /// <returns>The <see cref="IPaginationConfiguration"/> for chaining.</returns>
    IPaginationConfiguration UrlFormat(PaginationUrlFormat format = PaginationUrlFormat.Numeric, string? prefix = null);
}
