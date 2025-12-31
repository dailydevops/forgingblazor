namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Defines a contract for registering content page types with the ForgingBlazor framework.
/// This interface enables the registration and discovery of page types that will be processed
/// and rendered by the framework during content generation.
/// </summary>
public interface IContentRegistration
{
    /// <summary>
    /// Gets the <see cref="Type"/> of the page to be registered with the ForgingBlazor framework.
    /// </summary>
    /// <value>
    /// A <see cref="Type"/> representing the page class that will be used for content generation.
    /// This type should typically derive from <see cref="PageBase"/>.
    /// </value>
    Type PageType { get; }

    /// <summary>
    /// Gets the URL segment identifier for this content registration.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the URL path segment where content is located.
    /// An empty string indicates this is the default (root) content registration.
    /// </value>
    string Segment { get; }

    /// <summary>
    /// Gets the list of paths to exclude from content collection for this registration.
    /// </summary>
    /// <value>
    /// A read-only list of relative paths to exclude, or <see langword="null"/> if no exclusions are defined.
    /// </value>
    IReadOnlyList<string>? ExcludePaths { get; }

    /// <summary>
    /// Gets the priority of this content registration in the collection pipeline.
    /// </summary>
    /// <value>
    /// An integer value where higher numbers indicate higher priority.
    /// Registrations with higher priority are processed before those with lower priority.
    /// </value>
    int Priority { get; }
}

/// <summary>
/// Defines a content registration that represents the default (fallback) content handling.
/// </summary>
/// <remarks>
/// Default registrations have the lowest priority and are responsible for handling content
/// that doesn't belong to any specific segment. They exclude paths that have specific segment registrations.
/// </remarks>
public interface IDefaultRegistration : IContentRegistration
{
    /// <summary>
    /// Sets the paths to exclude from default content collection.
    /// </summary>
    /// <param name="excludePaths">
    /// The enumerable collection of segment paths to exclude.
    /// Cannot be <see langword="null"/>.
    /// </param>
    void SetExcludePaths(IEnumerable<string> excludePaths);
}
