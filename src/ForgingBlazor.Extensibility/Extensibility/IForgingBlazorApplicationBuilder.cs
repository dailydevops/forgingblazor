namespace NetEvolve.ForgingBlazor.Extensibility;

using Microsoft.AspNetCore.Components;
using NetEvolve.ForgingBlazor.Routing;

/// <summary>
/// Defines the contract for building a ForgingBlazor application.
/// </summary>
public interface IForgingBlazorApplicationBuilder
{
    IForgingBlazorApplicationBuilder AddRouting(Action<ISegmentBuilder> rootBuilder, Type rootPageType);

    IForgingBlazorApplicationBuilder AddRouting<TPage>(Action<ISegmentBuilder> rootBuilder)
        where TPage : class, IComponent => AddRouting(rootBuilder, typeof(TPage));

    /// <summary>
    /// Builds and returns a configured <see cref="IForgingBlazorApplication"/> instance.
    /// </summary>
    /// <returns>A configured <see cref="IForgingBlazorApplication"/> instance ready to run.</returns>
    IForgingBlazorApplication Build();
}
