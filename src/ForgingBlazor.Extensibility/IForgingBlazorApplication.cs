namespace NetEvolve.ForgingBlazor;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines the contract for a ForgingBlazor application that can be executed.
/// </summary>
public interface IForgingBlazorApplication
{
    /// <summary>
    /// Runs the ForgingBlazor application with the specified root component.
    /// </summary>
    /// <typeparam name="TRootComponent">The type of the root component to render. All members are dynamically accessed.</typeparam>
    void Run<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] TRootComponent>();

    /// <summary>
    /// Asynchronously runs the ForgingBlazor application with the specified root component.
    /// </summary>
    /// <typeparam name="TRootComponent">The type of the root component to render. All members are dynamically accessed.</typeparam>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. Default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task RunAsync<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] TRootComponent>(
        CancellationToken cancellationToken = default
    );
}
