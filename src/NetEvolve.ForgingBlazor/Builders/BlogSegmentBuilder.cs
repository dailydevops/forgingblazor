namespace NetEvolve.ForgingBlazor.Builders;

using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides the default implementation of <see cref="IBlogSegmentBuilder{TBlogPage}"/> for configuring blog segments in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TBlogPage">
/// The blog post type that inherits from <see cref="BlogPostBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This sealed class enables configuration of blog segments with custom validation rules and options.
/// It implements both <see cref="IBlogSegmentBuilder{TBlogPage}"/> and <see cref="ISegmentBuilder{TBlogPage}"/>
/// to provide blog-specific and general segment configuration capabilities.
/// </para>
/// <para>
/// Instances of this class are created through the <see cref="ApplicationBuilderExtensions.AddBlogSegment{TBlogType}(IApplicationBuilder, string)"/> extension method
/// and should not be instantiated directly.
/// </para>
/// </remarks>
/// <seealso cref="IBlogSegmentBuilder{TBlogPage}"/>
/// <seealso cref="ISegmentBuilder{TBlogPage}"/>
/// <seealso cref="BlogPostBase"/>
internal sealed class BlogSegmentBuilder<TBlogPage> : IBlogSegmentBuilder<TBlogPage>
    where TBlogPage : BlogPostBase
{
    /// <summary>
    /// Stores the parent application builder instance.
    /// </summary>
    private readonly IApplicationBuilder _builder;

    /// <summary>
    /// Stores the URL segment identifier for this blog section.
    /// </summary>
    private readonly string _segment;

    /// <summary>
    /// Gets the service collection for registering additional services related to the blog segment.
    /// </summary>
    /// <value>
    /// An <see cref="IServiceCollection"/> instance from the parent <see cref="IApplicationBuilder"/>.
    /// </value>
    public IServiceCollection Services => _builder.Services;

    /// <summary>
    /// Initializes a new instance of the <see cref="BlogSegmentBuilder{TBlogPage}"/> class with the specified application builder and segment.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance that represents the parent application being configured.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="segment">
    /// The URL segment identifier for this blog section.
    /// Cannot be <see langword="null"/> or whitespace.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segment"/> is <see langword="null"/> or whitespace.</exception>
    internal BlogSegmentBuilder(IApplicationBuilder builder, string segment)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);

        _builder = builder;
        _segment = segment;
    }

    /// <summary>
    /// Adds a validation rule for blog posts in this segment.
    /// </summary>
    /// <param name="validation">
    /// The <see cref="IValidation{TBlogPage}"/> instance that defines validation logic for blog posts.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IBlogSegmentBuilder{TBlogPage}"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method registers the validation as a keyed singleton service using the segment identifier as the key.
    /// The validation will be applied to blog posts within this specific segment during content processing.
    /// </para>
    /// <para>
    /// Multiple validation rules can be added to the same segment by calling this method multiple times.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="validation"/> is <see langword="null"/>.</exception>
    /// <seealso cref="IValidation{T}"/>
    public IBlogSegmentBuilder<TBlogPage> WithValidation(IValidation<TBlogPage> validation)
    {
        ArgumentNullException.ThrowIfNull(validation);

        _ = _builder.Services.AddKeyedSingleton(_segment, validation);

        return this;
    }

    /// <summary>
    /// Explicitly implements <see cref="ISegmentBuilder{TBlogPage}.WithValidation(IValidation{TBlogPage})"/> by delegating to the public <see cref="WithValidation(IValidation{TBlogPage})"/> method.
    /// </summary>
    /// <param name="validation">
    /// The <see cref="IValidation{TBlogPage}"/> instance that defines validation logic for blog posts.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same builder instance cast to <see cref="ISegmentBuilder{TBlogPage}"/> for method chaining.
    /// </returns>
    ISegmentBuilder<TBlogPage> ISegmentBuilder<TBlogPage>.WithValidation(IValidation<TBlogPage> validation) =>
        WithValidation(validation);
}
