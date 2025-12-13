namespace NetEvolve.ForgingBlazor.Options;

using System.Text;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides configuration options for blog content segments in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TBlogType">
/// The blog post type that inherits from <see cref="BlogPostBase"/>.
/// </typeparam>
/// <remarks>
/// This sealed class extends <see cref="ContentOptionsBase{TPageType}"/> with blog-specific configuration options,
/// including pagination settings that control how blog posts are displayed and navigated.
/// </remarks>
/// <seealso cref="ContentOptionsBase{TPageType}"/>
/// <seealso cref="BlogPostBase"/>
public sealed class BlogContentOptions<TBlogType> : ContentOptionsBase<TBlogType>
    where TBlogType : BlogPostBase
{
    /// <summary>
    /// Gets or sets the number of pagination links to display on either side of the current page.
    /// </summary>
    /// <value>
    /// An <see cref="int"/> value representing the pagination display count. The default value is 5.
    /// </value>
    /// <remarks>
    /// For example, if set to 5 and the current page is 10, pagination links will display pages 5-15.
    /// </remarks>
    public int PaginationDisplayCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets the format string template used to generate pagination URLs or display text.
    /// </summary>
    /// <value>
    /// A <see cref="CompositeFormat"/> instance representing the pagination format template.
    /// The default value is "{0}", which represents the page number.
    /// </value>
    /// <remarks>
    /// <para>
    /// This format string is used with <see cref="string.Format(IFormatProvider, string, object[])"/> to generate pagination elements.
    /// The placeholder {0} will be replaced with the page number.
    /// </para>
    /// <para>
    /// Example formats:
    /// <list type="bullet">
    /// <item><description>"{0}" - Simple page number</description></item>
    /// <item><description>"page/{0}" - URL-style format</description></item>
    /// <item><description>"Page {0}" - Display text with label</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public CompositeFormat PaginationFormat { get; set; } = CompositeFormat.Parse("{0}");
}
