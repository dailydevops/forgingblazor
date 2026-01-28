namespace NetEvolve.ForgingBlazor.Content;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Internal service for checking content expiration using <see cref="TimeProvider"/> to check <c>expiredAt</c> field against current time and exclude expired content.
/// </summary>
internal sealed class ContentExpirationService
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentExpirationService"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider for obtaining current time.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="timeProvider"/> is null.</exception>
    public ContentExpirationService(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);

        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Checks if the specified content descriptor has expired.
    /// </summary>
    /// <param name="descriptor">The content descriptor to check.</param>
    /// <returns><see langword="true"/> if the content has expired; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptor"/> is null.</exception>
    public bool IsExpired(ContentDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        // If ExpiredAt is null, the content never expires
        if (descriptor.ExpiredAt is null)
        {
            return false;
        }

        // Get current time from TimeProvider
        var currentTime = _timeProvider.GetUtcNow();

        // Content is expired if ExpiredAt is in the past
        return descriptor.ExpiredAt.Value < currentTime;
    }

    /// <summary>
    /// Filters out expired content from the specified collection.
    /// </summary>
    /// <typeparam name="TDescriptor">The type of content descriptor.</typeparam>
    /// <param name="descriptors">The collection of content descriptors to filter.</param>
    /// <returns>A filtered collection excluding expired content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptors"/> is null.</exception>
    public IEnumerable<TDescriptor> FilterExpired<TDescriptor>(IEnumerable<TDescriptor> descriptors)
        where TDescriptor : ContentDescriptor
    {
        ArgumentNullException.ThrowIfNull(descriptors);

        return descriptors.Where(d => !IsExpired(d));
    }
}
