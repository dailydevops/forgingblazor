namespace NetEvolve.ForgingBlazor.Services;

using System;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides the default implementation of <see cref="IDefaultRegistration"/> for registering the default page type.
/// </summary>
/// <typeparam name="TPageType">The default page type being registered, which must derive from <see cref="PageBase"/>.</typeparam>
/// <remarks>
/// This class is used to register the default page type when the application is configured with default pages.
/// It differs from <see cref="ContentRegistration{TPageType}"/> only semantically to identify it as the default registration.
/// </remarks>
/// <seealso cref="IContentRegistration"/>
/// <seealso cref="ContentRegistration{TPageType}"/>
internal sealed class DefaultContentRegistration<TPageType> : IDefaultRegistration
    where TPageType : PageBase
{
    /// <summary>
    /// Gets the type of the default page registered with the ForgingBlazor framework.
    /// </summary>
    /// <value>
    /// A <see cref="Type"/> instance representing <typeparamref name="TPageType"/>.
    /// </value>
    public Type PageType { get; } = typeof(TPageType);

    /// <summary>
    /// Gets the URL segment identifier for the default content.
    /// </summary>
    /// <value>
    /// An empty <see cref="string"/> representing that this is the default (root) content registration.
    /// </value>
    /// <remarks>
    /// Default registrations use an empty string as the segment identifier to indicate they handle
    /// content that doesn't belong to any specific segment.
    /// </remarks>
    public string Segment { get; } = "";

    /// <summary>
    /// Gets the list of paths to exclude from default content collection.
    /// </summary>
    /// <value>
    /// A read-only list of segment paths that should be excluded from this default registration.
    /// </value>
    /// <remarks>
    /// This property is set by the <see cref="SetExcludePaths"/> method and contains the segments
    /// of all other registered content types, preventing the default collector from processing
    /// content that belongs to specific segments.
    /// </remarks>
    public IReadOnlyList<string>? ExcludePaths { get; internal set; }

    /// <summary>
    /// Gets the priority of this content registration in the collection pipeline.
    /// </summary>
    /// <value>
    /// Returns -1, indicating this is the lowest priority registration and should be executed last.
    /// </value>
    /// <remarks>
    /// Default registrations have the lowest priority to ensure they only collect content that wasn't
    /// handled by more specific segment registrations.
    /// </remarks>
    public int Priority => -1;

    /// <summary>
    /// Sets the paths to exclude from default content collection.
    /// </summary>
    /// <param name="excludePaths">
    /// The enumerable collection of segment paths to exclude.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <remarks>
    /// This method is called during content registration initialization to configure which paths
    /// should be skipped by the default content collector. Typically, this includes all paths
    /// that have specific segment registrations.
    /// </remarks>
    public void SetExcludePaths(IEnumerable<string> excludePaths) => ExcludePaths = [.. excludePaths];
}
