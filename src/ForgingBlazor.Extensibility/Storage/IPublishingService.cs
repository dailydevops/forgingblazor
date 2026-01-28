namespace NetEvolve.ForgingBlazor;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Coordinates publishing operations for content and assets to make them available for serving.
/// </summary>
public interface IPublishingService
{
    /// <summary>
    /// Determines whether any changes require publishing.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns><c>true</c> if publishing is required; otherwise <c>false</c>.</returns>
    Task<bool> RequiresPublishingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes pending changes.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    Task PublishAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverts or removes published artifacts, making content unavailable.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    Task UnpublishAsync(CancellationToken cancellationToken = default);
}
