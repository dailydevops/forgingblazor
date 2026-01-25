namespace NetEvolve.ForgingBlazor;

using System;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Configures a specific page within routing.
/// </summary>
public interface IPageConfiguration
{
    /// <summary>
    /// Sets the content descriptor type used by the page.
    /// </summary>
    /// <typeparam name="T">The content descriptor type.</typeparam>
    /// <returns>The <see cref="IPageConfiguration"/> for chaining.</returns>
    IPageConfiguration WithContentType<T>()
        where T : ContentDescriptor;

    /// <summary>
    /// Sets the component used to render the page.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The <see cref="IPageConfiguration"/> for chaining.</returns>
    IPageConfiguration WithComponent<T>()
        where T : IComponent;

    /// <summary>
    /// Sets the layout used to render the page.
    /// </summary>
    /// <typeparam name="T">The layout component type.</typeparam>
    /// <returns>The <see cref="IPageConfiguration"/> for chaining.</returns>
    IPageConfiguration WithLayout<T>()
        where T : LayoutComponentBase;

    /// <summary>
    /// Configures metadata extensions for the page.
    /// </summary>
    /// <param name="configure">The metadata configuration callback.</param>
    /// <returns>The <see cref="IPageConfiguration"/> for chaining.</returns>
    IPageConfiguration WithMetadata(Action<IMetadataConfiguration> configure);
}
