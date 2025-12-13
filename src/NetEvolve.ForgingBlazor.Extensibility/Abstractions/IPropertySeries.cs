namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a page property that associates content with a named series or collection of related pages.
/// </summary>
/// <remarks>
/// Series are used to group related content that should be read or presented in a specific order,
/// such as tutorial parts, article series, or multi-part guides.
/// </remarks>
public interface IPropertySeries
{
    /// <summary>
    /// Gets or sets the name of the series this page belongs to.
    /// </summary>
    /// <value>
    /// A string identifying the series name, or <see langword="null"/> if the page is not part of a series.
    /// Pages with the same series name are typically grouped together and presented with navigation between parts.
    /// </value>
    string? Series { get; set; }
}
