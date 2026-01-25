namespace NetEvolve.ForgingBlazor.Routing.Configurations;

using System;
using global::NetEvolve.ForgingBlazor;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Implements the page configuration fluent API.
/// </summary>
internal sealed class PageConfiguration : IPageConfiguration
{
    private readonly PageConfigurationBuilderState _page;

    /// <summary>
    /// Initializes a new instance of the <see cref="PageConfiguration"/> class.
    /// </summary>
    /// <param name="page">The page configuration state.</param>
    internal PageConfiguration(PageConfigurationBuilderState page)
    {
        ArgumentNullException.ThrowIfNull(page);
        _page = page;
    }

    /// <inheritdoc />
    public IPageConfiguration WithContentType<T>()
        where T : ContentDescriptor
    {
        _page.ContentType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public IPageConfiguration WithComponent<T>()
        where T : IComponent
    {
        _page.ComponentType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public IPageConfiguration WithLayout<T>()
        where T : LayoutComponentBase
    {
        _page.LayoutType = typeof(T);
        return this;
    }

    /// <inheritdoc />
    public IPageConfiguration WithMetadata(Action<IMetadataConfiguration> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var configuration = new MetadataConfiguration(_page.Metadata);
        configure(configuration);
        return this;
    }
}
