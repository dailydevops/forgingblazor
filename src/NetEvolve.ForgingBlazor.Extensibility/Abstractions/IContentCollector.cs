namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Defines a content collector that processes and collects content from a specific source for ForgingBlazor applications.
/// </summary>
/// <remarks>
/// <para>
/// Content collectors are responsible for discovering, parsing, and registering content items (such as pages or blog posts)
/// from various sources. Implementations of this interface define how content is collected for specific segment types.
/// </para>
/// <para>
/// Multiple collectors can be registered for the same segment, and they are executed in priority order
/// (highest priority first).
/// </para>
/// </remarks>
/// <seealso cref="IContentRegister"/>
/// <seealso cref="IContentRegistration"/>
public interface IContentCollector
{
    /// <summary>
    /// Gets the priority of this content collector in the collection pipeline.
    /// </summary>
    /// <value>
    /// An integer value where higher numbers indicate higher priority.
    /// Collectors with higher priority are executed before those with lower priority.
    /// </value>
    int Priority { get; }

    /// <summary>
    /// Asynchronously collects content from the source and registers it with the content register.
    /// </summary>
    /// <param name="contentRegister">
    /// The <see cref="IContentRegister"/> instance to register collected content with.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="registration">
    /// The <see cref="IContentRegistration"/> instance containing configuration for the content segment.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the collection operation.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous content collection operation.
    /// </returns>
    ValueTask CollectAsync(
        IContentRegister contentRegister,
        IContentRegistration registration,
        CancellationToken cancellationToken
    );
}
