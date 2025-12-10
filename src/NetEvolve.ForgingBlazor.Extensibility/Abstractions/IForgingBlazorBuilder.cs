namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

public interface IForgingBlazorBuilder
{
    IServiceCollection Services { get; }
}
