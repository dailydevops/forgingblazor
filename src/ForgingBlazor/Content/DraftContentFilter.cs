namespace NetEvolve.ForgingBlazor.Content;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Internal service for filtering content based on <c>draft</c> property and environment (development vs production).
/// </summary>
internal sealed class DraftContentFilter
{
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="DraftContentFilter"/> class.
    /// </summary>
    /// <param name="environment">The host environment.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="environment"/> is null.</exception>
    public DraftContentFilter(IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(environment);

        _environment = environment;
    }

    /// <summary>
    /// Checks if the specified content descriptor should be filtered out based on its draft status.
    /// </summary>
    /// <param name="descriptor">The content descriptor to check.</param>
    /// <returns><see langword="true"/> if the content should be filtered out; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> is null.</exception>
    public bool ShouldFilter(ContentDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        // In development, show all content (including drafts)
        if (_environment.IsDevelopment())
        {
            return false;
        }

        // In production, filter out draft content
        return descriptor.Draft;
    }

    /// <summary>
    /// Filters out draft content from the specified collection based on environment.
    /// </summary>
    /// <typeparam name="TDescriptor">The type of content descriptor.</typeparam>
    /// <param name="descriptors">The collection of content descriptors to filter.</param>
    /// <returns>A filtered collection excluding draft content in production.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptors"/> is null.</exception>
    public IEnumerable<TDescriptor> FilterDrafts<TDescriptor>(IEnumerable<TDescriptor> descriptors)
        where TDescriptor : ContentDescriptor
    {
        ArgumentNullException.ThrowIfNull(descriptors);

        // In development, return all content
        if (_environment.IsDevelopment())
        {
            return descriptors;
        }

        // In production, filter out drafts
        return descriptors.Where(d => !d.Draft);
    }
}
