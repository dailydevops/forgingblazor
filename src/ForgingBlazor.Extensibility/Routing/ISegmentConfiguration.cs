namespace NetEvolve.ForgingBlazor;

using System;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Configures a content segment, including pagination, component/layout defaults, and nested segments.
/// </summary>
public interface ISegmentConfiguration
{
    /// <summary>
    /// Configures pagination behavior for the segment.
    /// </summary>
    /// <param name="configure">The configuration callback for pagination options.</param>
    /// <returns>The <see cref="ISegmentConfiguration"/> for chaining.</returns>
    ISegmentConfiguration WithPagination(Action<IPaginationConfiguration> configure);

    /// <summary>
    /// Sets the content descriptor type for pages in this segment.
    /// </summary>
    /// <typeparam name="T">The content descriptor type.</typeparam>
    /// <returns>The <see cref="ISegmentConfiguration"/> for chaining.</returns>
    ISegmentConfiguration WithContentType<T>()
        where T : ContentDescriptor;

    /// <summary>
    /// Sets the component used for the segment index page.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The <see cref="ISegmentConfiguration"/> for chaining.</returns>
    ISegmentConfiguration WithIndexComponent<T>()
        where T : IComponent;

    /// <summary>
    /// Sets the layout used for the segment index page.
    /// </summary>
    /// <typeparam name="T">The layout component type.</typeparam>
    /// <returns>The <see cref="ISegmentConfiguration"/> for chaining.</returns>
    ISegmentConfiguration WithIndexLayout<T>()
        where T : LayoutComponentBase;

    /// <summary>
    /// Sets the component used to render individual content pages.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The <see cref="ISegmentConfiguration"/> for chaining.</returns>
    ISegmentConfiguration WithPageComponent<T>()
        where T : IComponent;

    /// <summary>
    /// Sets the layout used to render individual content pages.
    /// </summary>
    /// <typeparam name="T">The layout component type.</typeparam>
    /// <returns>The <see cref="ISegmentConfiguration"/> for chaining.</returns>
    ISegmentConfiguration WithPageLayout<T>()
        where T : LayoutComponentBase;

    /// <summary>
    /// Configures metadata extensions for content in this segment.
    /// </summary>
    /// <param name="configure">The metadata configuration callback.</param>
    /// <returns>The <see cref="ISegmentConfiguration"/> for chaining.</returns>
    ISegmentConfiguration WithMetadata(Action<IMetadataConfiguration> configure);

    /// <summary>
    /// Maps a nested segment within the current segment.
    /// </summary>
    /// <param name="name">The nested segment name.</param>
    /// <param name="configure">The configuration callback for the nested segment.</param>
    /// <returns>The configured nested <see cref="ISegmentConfiguration"/>.</returns>
    ISegmentConfiguration MapSegment(string name, Action<ISegmentConfiguration> configure);
}
