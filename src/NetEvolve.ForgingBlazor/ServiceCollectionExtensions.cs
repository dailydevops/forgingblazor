namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Core.Models;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;
using NetEvolve.ForgingBlazor.Services;

internal static class ServiceCollectionExtensions
{
    internal static bool IsServiceTypeRegistered<T>(this IServiceCollection builder)
        where T : class => builder.Any(x => x.ServiceType == typeof(T));
}
