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
}
