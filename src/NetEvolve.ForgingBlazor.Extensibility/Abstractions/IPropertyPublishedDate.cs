namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a page property that tracks when the page was published or made available.
/// </summary>
/// <remarks>
/// This interface is commonly used for blog posts, articles, and time-sensitive content where publication date is relevant.
/// The date is stored with timezone information to accurately represent when content became available.
/// </remarks>
public interface IPropertyPublishedDate
{
    /// <summary>
    /// Gets or sets the date and time when the blog post was published.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the publication date and time with timezone information,
    /// or <see langword="null"/> if the publication date has not been set.
    /// </value>
    /// <remarks>
    /// This property is used for sorting blog posts chronologically, displaying publication dates,
    /// and filtering content by date ranges.
    /// </remarks>
    DateTimeOffset? PublishDate { get; set; }
}
