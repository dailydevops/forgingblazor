namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Represents the main application entry point that can be executed asynchronously.
/// </summary>
/// <remarks>
/// This interface defines the contract for applications built using the ForgingBlazor framework.
/// Implementations encapsulate the application's execution logic and lifecycle.
/// </remarks>
public interface IApplication
{
    /// <summary>
    /// Runs the application asynchronously and returns an exit code indicating the result.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to request cancellation of the application execution.</param>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.
    /// The task result contains an exit code where 0 typically indicates success, and non-zero values indicate various error conditions.
    /// </returns>
    /// <remarks>
    /// Implementations should respect the cancellation token and return a non-zero exit code (typically 130) when cancellation is requested.
    /// </remarks>
    ValueTask<int> RunAsync(CancellationToken cancellationToken = default);
}
