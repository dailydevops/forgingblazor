namespace NetEvolve.ForgingBlazor.Options;

using System.Text;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides base configuration options for content segments in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TPageType">
/// The page type that inherits from <see cref="PageBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This abstract class serves as the foundation for content configuration options,
/// defining common properties shared by all content types including pages and blog posts.
/// </para>
/// <para>
/// Derived classes such as <see cref="BlogContentOptions{TBlogType}"/>, <see cref="DefaultContentOptions{TPageType}"/>,
/// and <see cref="SegmentContentOptions{TPageType}"/> extend this base class with type-specific configuration options.
/// </para>
/// </remarks>
/// <seealso cref="PageBase"/>
/// <seealso cref="BlogContentOptions{TBlogType}"/>
/// <seealso cref="DefaultContentOptions{TPageType}"/>
/// <seealso cref="SegmentContentOptions{TPageType}"/>
public abstract class ContentOptionsBase<TPageType>
    where TPageType : PageBase
{
    /// <summary>
    /// Gets the <see cref="Type"/> of the content being configured.
    /// </summary>
    /// <value>
    /// A <see cref="Type"/> instance representing <typeparamref name="TPageType"/>.
    /// </value>
    /// <remarks>
    /// This property is used internally to identify and process the specific page type
    /// associated with these configuration options during content generation and validation.
    /// </remarks>
    public Type ContentType { get; } = typeof(TPageType);

    /// <summary>
    /// Gets or sets the character encoding used for reading and writing content files.
    /// </summary>
    /// <value>
    /// An <see cref="Encoding"/> instance representing the file encoding. The default value is <see cref="Encoding.UTF8"/>.
    /// </value>
    /// <remarks>
    /// UTF-8 encoding is used by default as it supports all Unicode characters and is widely compatible.
    /// Change this property if content files use a different encoding (e.g., UTF-16, ASCII, or locale-specific encodings).
    /// </remarks>
    public Encoding FileEncoding { get; set; } = Encoding.UTF8;
}
