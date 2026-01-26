namespace NetEvolve.ForgingBlazor.Storage;

using System;
using System.Collections.Concurrent;

/// <summary>
/// Internal service managing user confirmation state for publishing operations.
/// </summary>
internal sealed class PublishingConfirmationService
{
    private readonly ConcurrentDictionary<string, bool> _confirmations = new();

    /// <summary>
    /// Grants confirmation for publishing the specified content path.
    /// </summary>
    /// <param name="contentPath">The content path to confirm.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contentPath"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="contentPath"/> is empty or whitespace.</exception>
    public void GrantConfirmation(string contentPath)
    {
        ArgumentNullException.ThrowIfNull(contentPath);

        if (string.IsNullOrWhiteSpace(contentPath))
        {
            throw new ArgumentException("Content path cannot be empty or whitespace.", nameof(contentPath));
        }

        _confirmations[contentPath] = true;
    }

    /// <summary>
    /// Checks if confirmation has been granted for the specified content path.
    /// </summary>
    /// <param name="contentPath">The content path to check.</param>
    /// <returns><see langword="true"/> if confirmation has been granted; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contentPath"/> is null.</exception>
    public bool IsConfirmed(string contentPath)
    {
        ArgumentNullException.ThrowIfNull(contentPath);

        return _confirmations.TryGetValue(contentPath, out var confirmed) && confirmed;
    }

    /// <summary>
    /// Clears confirmation for the specified content path.
    /// </summary>
    /// <param name="contentPath">The content path to clear confirmation for.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="contentPath"/> is null.</exception>
    public void ClearConfirmation(string contentPath)
    {
        ArgumentNullException.ThrowIfNull(contentPath);

        _ = _confirmations.TryRemove(contentPath, out _);
    }

    /// <summary>
    /// Clears all confirmations.
    /// </summary>
    public void ClearAllConfirmations() => _confirmations.Clear();
}
