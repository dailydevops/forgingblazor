namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

internal static class ServiceCollectionExtensions
{
    internal static bool IsServiceTypeRegistered<T>(this IServiceCollection builder)
        where T : class => builder.Any(x => x.ServiceType == typeof(T));
}
