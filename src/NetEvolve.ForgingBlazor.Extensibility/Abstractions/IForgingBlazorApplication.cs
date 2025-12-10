namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using Microsoft.Extensions.DependencyInjection;

public interface IForgingBlazorApplication
{
    IServiceCollection Services { get; }

    ValueTask<int> RunAsync(CancellationToken cancellationToken = default);
}
