namespace NetEvolve.ForgingBlazor.Routing.Configurations;

using System;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Routing;

/// <summary>
/// Implements the pagination configuration fluent API.
/// </summary>
internal sealed class PaginationConfiguration : IPaginationConfiguration
{
    private readonly PaginationConfigurationBuilderState _pagination;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationConfiguration"/> class.
    /// </summary>
    /// <param name="pagination">The pagination configuration state.</param>
    internal PaginationConfiguration(PaginationConfigurationBuilderState pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);
        _pagination = pagination;
    }

    /// <inheritdoc />
    public IPaginationConfiguration PageSize(int size = Defaults.PageSizeDefault)
    {
        if (size < Defaults.PageSizeMinimum || size > Defaults.PageSizeMaximum)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                size,
                $"Page size must be between {Defaults.PageSizeMinimum} and {Defaults.PageSizeMaximum}."
            );
        }

        _pagination.PageSize = size;
        return this;
    }

    /// <inheritdoc />
    public IPaginationConfiguration UrlFormat(
        PaginationUrlFormat format = PaginationUrlFormat.Numeric,
        string? prefix = null
    )
    {
        _pagination.Format = format;

        if (format == PaginationUrlFormat.Prefixed)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException(
                    "A prefix must be provided when using the prefixed pagination format.",
                    nameof(prefix)
                );
            }

            _pagination.Prefix = prefix;
        }
        else
        {
            _pagination.Prefix = null;
        }

        return this;
    }
}
