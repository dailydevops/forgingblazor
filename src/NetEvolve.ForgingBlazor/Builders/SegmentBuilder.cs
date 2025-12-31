namespace NetEvolve.ForgingBlazor.Builders;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;
using NetEvolve.ForgingBlazor.Services;

/// <summary>
/// Provides the default implementation of <see cref="ISegmentBuilder{TPageType}"/> for configuring content segments in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TPageType">
/// The page type that inherits from <see cref="PageBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This sealed class enables configuration of content segments with custom validation rules and options.
/// It provides a fluent API for registering validators and configuring segment-specific services.
/// </para>
/// <para>
/// Instances of this class are created through the <see cref="ApplicationBuilderExtensions.AddSegment{TPageType}(IApplicationBuilder, string)"/> extension method
/// and should not be instantiated directly.
/// </para>
/// </remarks>
/// <seealso cref="ISegmentBuilder{TPageType}"/>
/// <seealso cref="PageBase"/>
internal sealed class SegmentBuilder<TPageType> : ISegmentBuilder<TPageType>
    where TPageType : PageBase
{
    /// <summary>
    /// Stores the parent application builder instance.
    /// </summary>
    /// <remarks>
    /// This field holds a reference to the parent <see cref="IApplicationBuilder"/> for registering segment-specific services.
    /// </remarks>
    private readonly IApplicationBuilder _builder;

    /// <summary>
    /// Stores the URL segment identifier for this content section.
    /// </summary>
    /// <remarks>
    /// This field holds the segment identifier (e.g., <c>blog</c>, <c>docs</c>) that determines
    /// the URL path and content directory for pages in this segment.
    /// </remarks>
    private readonly string _segment;

    /// <summary>
    /// Gets the service collection for registering additional services related to the segment.
    /// </summary>
    /// <value>
    /// An <see cref="IServiceCollection"/> instance from the parent <see cref="IApplicationBuilder"/>.
    /// </value>
    public IServiceCollection Services => _builder.Services;

    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentBuilder{TPageType}"/> class with the specified application builder and segment.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IApplicationBuilder"/> instance that represents the parent application being configured.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="segment">
    /// The URL segment identifier for this content section (e.g., <c>blog</c>, <c>docs</c>).
    /// Cannot be <see langword="null"/> or whitespace.
    /// </param>
    /// <remarks>
    /// <para>
    /// This constructor performs the following operations:
    /// <list type="number">
    /// <item><description>Validates that <paramref name="builder"/> is not <see langword="null"/></description></item>
    /// <item><description>Validates that <paramref name="segment"/> is not <see langword="null"/> or whitespace</description></item>
    /// <item><description>Registers a new <see cref="ContentRegistration{TPageType}"/> with segment and priority settings</description></item>
    /// <item><description>Registers a keyed singleton <see cref="IContentCollector"/> instance with the segment as key</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="segment"/> is <see langword="null"/> or whitespace.</exception>
    internal SegmentBuilder(IApplicationBuilder builder, string segment)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(segment);

        _builder = builder;
        _segment = segment;

        Services
            .AddSingleton<IContentRegistration>(new ContentRegistration<TPageType> { Segment = segment, Priority = 0 })
            .TryAddKeyedSingleton<IContentCollector, MarkdownContentCollector<TPageType>>(segment);
    }

    /// <summary>
    /// Adds a validation rule for pages in this segment.
    /// </summary>
    /// <param name="validation">
    /// The <see cref="IValidation{TPageType}"/> instance that defines validation logic for pages.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="ISegmentBuilder{TPageType}"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method registers the validation as a keyed singleton service using the segment identifier as the key.
    /// The validation will be applied to pages within this specific segment during content processing.
    /// </para>
    /// <para>
    /// Multiple validation rules can be added to the same segment by calling this method multiple times.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="validation"/> is <see langword="null"/>.</exception>
    /// <seealso cref="IValidation{T}"/>
    public ISegmentBuilder<TPageType> WithValidation(IValidation<TPageType> validation)
    {
        ArgumentNullException.ThrowIfNull(validation);

        _ = _builder.Services.AddKeyedSingleton(_segment, validation);

        return this;
    }
}
