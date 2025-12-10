namespace NetEvolve.ForgingBlazor;

using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

public sealed class ForgingBlazorApplicationBuilder : IForgingBlazorApplicationBuilder
{
    private readonly string[] _args;

    public IServiceCollection Services { get; init; }

    public ForgingBlazorApplicationBuilder(string[] args)
    {
        _args = args;

        Services = new ServiceCollection();
    }

    public static IForgingBlazorApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = new ForgingBlazorApplicationBuilder(args);

        return builder;
    }

    public IForgingBlazorApplication Build()
    {
        var serviceProvider = Services.BuildServiceProvider();

        return new ForgingBlazorApplication(_args, serviceProvider);
    }
}
