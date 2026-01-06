namespace NetEvolve.ForgingBlazor.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Microsoft.Extensions.Options.ValidateOptionsResult;

/// <summary>
/// Binds, validates, and normalizes pagination configuration sourced from application settings.
/// </summary>
internal sealed class PaginationConfigValidation
    : IConfigureOptions<PaginationConfig>,
        IValidateOptions<PaginationConfig>,
        IPostConfigureOptions<PaginationConfig>
{
    /// <summary>
    /// Backing configuration provider used to populate pagination options.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationConfigValidation"/> class.
    /// </summary>
    /// <param name="configuration">The configuration root containing the pagination section.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is null.</exception>
    public PaginationConfigValidation(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _configuration = configuration;
    }

    /// <summary>
    /// Binds the <paramref name="options"/> instance to the <c>pagination</c> configuration section.
    /// </summary>
    /// <param name="options">The pagination options instance to populate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public void Configure(PaginationConfig options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _configuration.Bind("pagination", options);
    }

    /// <summary>
    /// Performs validation of the supplied pagination options.
    /// </summary>
    /// <param name="name">The named options instance; unused in this validation.</param>
    /// <param name="options">The pagination options to validate.</param>
    /// <returns>A successful validation result because no additional checks are required.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public ValidateOptionsResult Validate(string? name, PaginationConfig options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if ((Defaults.PageSizeMinimum <= options.PageSize) != (options.PageSize <= Defaults.PageSizeMaximum))
        {
            return Fail("Configuration(Pagination.PageSize): The page size must be between 1 and 100.");
        }

        if (!string.IsNullOrWhiteSpace(options.Path) && !Check.IsValidPathSegment(options.Path))
        {
            return Fail("Configuration(Pagination.Path): The pagination path must be a valid URL path segment.");
        }

        return Success;
    }

    /// <summary>
    /// Normalizes the pagination configuration after binding to ensure empty paths are treated as <see langword="null"/>.
    /// </summary>
    /// <param name="name">The named options instance; unused in post-configuration.</param>
    /// <param name="options">The pagination options to normalize.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public void PostConfigure(string? name, PaginationConfig options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(options.Path))
        {
            options.Path = null;
        }
    }
}
