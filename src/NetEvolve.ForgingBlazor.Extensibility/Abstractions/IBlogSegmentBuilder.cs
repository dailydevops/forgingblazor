namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Defines a builder pattern for configuring blog content segments in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TBlogType">
/// The blog post type that inherits from <see cref="BlogPostBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This interface extends <see cref="ISegmentBuilder{TPageType}"/> with blog-specific configuration capabilities.
/// It provides a fluent API for adding validation rules and configuring blog segment behavior.
/// </para>
/// <para>
/// Instances implementing this interface are typically created through application builder extension methods
/// such as <c>AddBlogSegment</c> and should not be instantiated directly.
/// </para>
/// </remarks>
/// <seealso cref="ISegmentBuilder{TPageType}"/>
/// <seealso cref="BlogPostBase"/>
public interface IBlogSegmentBuilder<TBlogType> : ISegmentBuilder<TBlogType>
    where TBlogType : BlogPostBase
{
    /// <summary>
    /// Adds a validation rule for blog posts in this segment.
    /// </summary>
    /// <param name="validation">
    /// The <see cref="IValidation{TBlogType}"/> instance that defines validation logic for blog posts.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IBlogSegmentBuilder{TBlogType}"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// This method signature hides the base interface method to return the blog-specific builder type,
    /// enabling continued use of blog-specific configuration methods in a fluent chain.
    /// </remarks>
    new IBlogSegmentBuilder<TBlogType> WithValidation(IValidation<TBlogType> validation);
}
