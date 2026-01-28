namespace NetEvolve.ForgingBlazor;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Defines the contract for building a ForgingBlazor application.
/// </summary>
public interface IForgingBlazorApplicationBuilder
{
    /// <summary>
    /// Gets the service collection for configuring application services.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Builds and returns a configured <see cref="IForgingBlazorApplication"/> instance.
    /// </summary>
    /// <returns>A configured <see cref="IForgingBlazorApplication"/> instance ready to run.</returns>
    IForgingBlazorApplication Build();
}
