namespace NetEvolve.ForgingBlazor.Extensibility;

/// <summary>
/// Defines the contract for building a ForgingBlazor application.
/// </summary>
public interface IForgingBlazorApplicationBuilder
{
    /// <summary>
    /// Builds and returns a configured <see cref="IForgingBlazorApplication"/> instance.
    /// </summary>
    /// <returns>A configured <see cref="IForgingBlazorApplication"/> instance ready to run.</returns>
    IForgingBlazorApplication Build();
}
