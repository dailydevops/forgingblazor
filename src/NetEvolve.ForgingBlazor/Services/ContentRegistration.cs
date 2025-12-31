namespace NetEvolve.ForgingBlazor.Services;

using System;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides a generic implementation of <see cref="IContentRegistration"/> for custom page types.
/// </summary>
/// <typeparam name="TPageType">The page type being registered, which must derive from <see cref="PageBase"/>.</typeparam>
/// <remarks>
/// This class allows registration of custom page types with the ForgingBlazor framework.
/// It serves as a reusable registration container for any page type that inherits from <see cref="PageBase"/>.
/// </remarks>
/// <seealso cref="IContentRegistration"/>
/// <seealso cref="DefaultContentRegistration{TPageType}"/>
internal sealed class ContentRegistration<TPageType> : IContentRegistration
    where TPageType : PageBase
{
    /// <summary>
    /// Gets the type of the page registered with the ForgingBlazor framework.
    /// </summary>
    /// <value>
    /// A <see cref="Type"/> instance representing <typeparamref name="TPageType"/>.
    /// </value>
    public Type PageType { get; } = typeof(TPageType);

    /// <summary>
    /// Gets or sets the URL segment identifier for this content registration.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the URL path segment where content is located.
    /// Default value is an empty string.
    /// </value>
    /// <remarks>
    /// The segment defines the subdirectory within the content path where files for this
    /// page type are located. For example, a segment value of "blog" would look for content
    /// in the "Content/blog" directory.
    /// </remarks>
    public string Segment { get; set; } = "";

    /// <summary>
    /// Gets or sets the list of paths to exclude from content collection for this registration.
    /// </summary>
    /// <value>
    /// A read-only list of relative paths to exclude, or <see langword="null"/> if no exclusions are defined.
    /// </value>
    /// <remarks>
    /// This property allows fine-grained control over which subdirectories within the segment
    /// should be skipped during content collection.
    /// </remarks>
    public IReadOnlyList<string>? ExcludePaths { get; set; }

    /// <summary>
    /// Gets or sets the priority of this content registration in the collection pipeline.
    /// </summary>
    /// <value>
    /// An integer value where higher numbers indicate higher priority.
    /// Default value is 0.
    /// </value>
    /// <remarks>
    /// Content registrations are processed in descending priority order. Higher priority registrations
    /// are executed before lower priority ones, allowing specific content types to be collected first.
    /// </remarks>
    public int Priority { get; set; }
}
