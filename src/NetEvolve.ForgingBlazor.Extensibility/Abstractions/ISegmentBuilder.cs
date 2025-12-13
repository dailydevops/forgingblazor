namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Defines a builder pattern for configuring content segments in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TPageType">
/// The page type that inherits from <see cref="PageBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This interface provides a fluent API for configuring content segments, including adding validation rules
/// and registering segment-specific services. Implementations allow customization of how pages within
/// a segment are processed and validated.
/// </para>
/// <para>
/// Instances implementing this interface are typically created through application builder extension methods
/// and should not be instantiated directly.
/// </para>
/// </remarks>
/// <seealso cref="PageBase"/>
/// <seealso cref="IValidation{TPageType}"/>
public interface ISegmentBuilder<TPageType>
    where TPageType : PageBase
{
    /// <summary>
    /// Gets the service collection for registering additional services related to the segment.
    /// </summary>
    /// <value>
    /// An <see cref="IServiceCollection"/> instance that allows registration of segment-specific services.
    /// </value>
    IServiceCollection Services { get; }

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
    /// Multiple validation rules can be added to a segment by calling this method multiple times.
    /// Each validation will be executed during content processing.
    /// </remarks>
    ISegmentBuilder<TPageType> WithValidation(IValidation<TPageType> validation);
}
