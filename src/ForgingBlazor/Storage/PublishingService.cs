namespace NetEvolve.ForgingBlazor.Storage;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Internal implementation of <see cref="IPublishingService"/> with draft detection and publishing target sync.
/// </summary>
internal sealed class PublishingService : IPublishingService
{
    private readonly IContentStorageProvider? _draftStorage;
    private readonly IContentStorageProvider? _publishingStorage;

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishingService"/> class.
    /// </summary>
    /// <param name="draftStorage">The draft storage provider.</param>
    /// <param name="publishingStorage">The publishing storage provider (optional).</param>
    public PublishingService(IContentStorageProvider? draftStorage, IContentStorageProvider? publishingStorage)
    {
        _draftStorage = draftStorage;
        _publishingStorage = publishingStorage;
    }

    /// <inheritdoc />
    public Task<bool> RequiresPublishingAsync(CancellationToken cancellationToken = default)
    {
        // If no publishing storage is configured, publishing is not required
        if (_publishingStorage is null)
        {
            return Task.FromResult(false);
        }

        // If no draft storage is configured, use publishing storage directly
        if (_draftStorage is null)
        {
            return Task.FromResult(false);
        }

        // Publishing is required if both storages are configured
        // Note: actual content comparison would require knowing segments and cultures
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public Task PublishAsync(CancellationToken cancellationToken = default)
    {
        if (_publishingStorage is null)
        {
            throw new InvalidOperationException("Publishing storage is not configured.");
        }

        if (_draftStorage is null)
        {
            throw new InvalidOperationException("Draft storage is not configured.");
        }

        // TODO: Implement publishing logic when route registry provides segment/culture information
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UnpublishAsync(CancellationToken cancellationToken = default)
    {
        if (_publishingStorage is null)
        {
            throw new InvalidOperationException("Publishing storage is not configured.");
        }

        // TODO: Implement unpublishing logic when route registry provides segment/culture information
        return Task.CompletedTask;
    }
}
