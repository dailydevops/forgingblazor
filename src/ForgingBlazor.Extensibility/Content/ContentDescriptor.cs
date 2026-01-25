namespace NetEvolve.ForgingBlazor;

using System;

/// <summary>
/// Represents the core, minimal description of a content item used by the dynamic content system.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><description><see cref="Title"/>: A human-friendly title for the content.</description></item>
/// <item><description><see cref="Slug"/>: A URL-safe identifier used for routing (e.g., "getting-started").</description></item>
/// <item><description><see cref="PublishedDate"/>: The date and time the content is considered published.</description></item>
/// <item><description><see cref="Draft"/>: Indicates whether the content is a draft and should not be publicly visible.</description></item>
/// <item><description><see cref="ExpiredAt"/>: The optional date and time when the content expires and should no longer be served.</description></item>
/// <item><description><see cref="Body"/>: The Markdown body of the content, if any.</description></item>
/// <item><description><see cref="BodyHtml"/>: The rendered HTML body of the content, if available.</description></item>
/// </list>
/// This type is a POCO and intentionally contains no behavior.
/// </remarks>
public class ContentDescriptor
{
    /// <summary>
    /// A human-friendly title for the content.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// A URL-safe identifier used for routing (e.g., "getting-started").
    /// </summary>
    public required string Slug { get; set; }

    /// <summary>
    /// The date and time the content is considered published.
    /// </summary>
    public DateTimeOffset PublishedDate { get; set; }

    /// <summary>
    /// Indicates whether the content is a draft and should not be publicly visible.
    /// </summary>
    public bool Draft { get; set; }

    /// <summary>
    /// The optional date and time when the content expires and should no longer be served.
    /// </summary>
    public DateTimeOffset? ExpiredAt { get; set; }

    /// <summary>
    /// The Markdown body of the content, if any.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// The rendered HTML body of the content, if available.
    /// </summary>
    public string? BodyHtml { get; set; }
}
