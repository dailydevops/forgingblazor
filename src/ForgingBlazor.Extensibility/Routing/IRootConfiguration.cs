namespace NetEvolve.ForgingBlazor;

using System;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Defines root-level routing configuration including culture, defaults, and home page.
/// </summary>
public interface IRootConfiguration
{
    /// <summary>
    /// Configures culture options and supported cultures for the site.
    /// </summary>
    /// <param name="configure">The configuration callback to set culture options.</param>
    /// <returns>The <see cref="ICultureConfiguration"/> instance.</returns>
    ICultureConfiguration WithCulture(Action<ICultureConfiguration> configure);

    /// <summary>
    /// Sets the default content descriptor type for routing where specific type is not provided.
    /// </summary>
    /// <typeparam name="T">The content descriptor type.</typeparam>
    /// <returns>The <see cref="IRootConfiguration"/> for chaining.</returns>
    IRootConfiguration WithDefaultContentType<T>()
        where T : ContentDescriptor;

    /// <summary>
    /// Sets the default component to render content index pages.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>The <see cref="IRootConfiguration"/> for chaining.</returns>
    IRootConfiguration WithDefaultComponent<T>()
        where T : IComponent;

    /// <summary>
    /// Sets the default layout for routed pages.
    /// </summary>
    /// <typeparam name="T">The layout component type.</typeparam>
    /// <returns>The <see cref="IRootConfiguration"/> for chaining.</returns>
    IRootConfiguration WithDefaultLayout<T>()
        where T : LayoutComponentBase;

    /// <summary>
    /// Sets the content descriptor type for the site's home page.
    /// </summary>
    /// <typeparam name="T">The content descriptor type to use for the home page.</typeparam>
    /// <returns>The <see cref="IRootConfiguration"/> for chaining.</returns>
    IRootConfiguration WithHomePage<T>()
        where T : ContentDescriptor;
}
