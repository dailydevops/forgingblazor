namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Internal marker interface used to identify the content registration service during the startup phase.
/// This interface is used to register the core content registration implementation in the dependency
/// injection container and enables the filtering mechanism to exclude startup-specific services from
/// the command execution pipeline.
/// </summary>
/// <remarks>
/// This is an intentionally empty marker interface that serves as a metadata tag for service identification.
/// It should be implemented by the internal content registration service that handles content-related
/// initialization during application startup.
/// </remarks>
public interface IContentRegister
{
    /// <summary>
    /// Asynchronously collects content from all registered content segments using their associated collectors.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the collection operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous content collection operation.
    /// </returns>
    ValueTask CollectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Registers a page with the content register.
    /// </summary>
    /// <typeparam name="TPageType">
    /// The page type that inherits from <see cref="PageBase"/>.
    /// </typeparam>
    /// <param name="page">
    /// The page instance to register. Cannot be <see langword="null"/>.
    /// </param>
    void Register<TPageType>(TPageType page)
        where TPageType : PageBase;
}
