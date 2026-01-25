namespace NetEvolve.ForgingBlazor;

using System;

/// <summary>
/// Provides a fluent builder to configure dynamic content routing.
/// </summary>
public interface IRoutingBuilder
{
    /// <summary>
    /// Configures root-level routing options and defaults.
    /// </summary>
    /// <param name="configure">The configuration callback for the root settings.</param>
    /// <returns>The <see cref="IRootConfiguration"/> instance for further configuration.</returns>
    IRootConfiguration ConfigureRoot(Action<IRootConfiguration> configure);

    /// <summary>
    /// Maps a content segment.
    /// </summary>
    /// <param name="name">The segment name (URL path segment).</param>
    /// <param name="configure">The configuration callback for the segment.</param>
    /// <returns>The configured <see cref="ISegmentConfiguration"/>.</returns>
    ISegmentConfiguration MapSegment(string name, Action<ISegmentConfiguration> configure);

    /// <summary>
    /// Maps a specific page by its slug.
    /// </summary>
    /// <param name="slug">The page slug.</param>
    /// <param name="configure">The configuration callback for the page.</param>
    /// <returns>The configured <see cref="IPageConfiguration"/>.</returns>
    IPageConfiguration MapPage(string slug, Action<IPageConfiguration> configure);
}
