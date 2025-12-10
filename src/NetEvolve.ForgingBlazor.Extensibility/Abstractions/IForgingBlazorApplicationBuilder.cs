namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using Microsoft.Extensions.DependencyInjection;

public interface IForgingBlazorApplicationBuilder
{
    IForgingBlazorApplication Build();
    IServiceCollection Services { get; }
}
