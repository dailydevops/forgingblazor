namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Services;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddForgingBlazorServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (!services.IsServiceTypeRegistered<MarkerCoreServices>())
        {
            _ = services
                .AddSingleton<MarkerCoreServices>()
                // Register all Commands
                .AddSingleton<RootCommand, CommandCli>()
                .AddSingleton<Command, CommandBuild>()
                .AddSingleton<Command, CommandCreate>()
                .AddSingleton<Command, CommandExample>()
                .AddSingleton<Command, CommandServe>()
                // Register core services
                .AddSingleton<IContentRegister, ForgingBlazorContentRegister>();
        }

        return services;
    }

    internal static IServiceCollection TransferAllServices(
        this IServiceCollection services,
        IServiceProvider serviceProvider
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var typeStartUpMarker = typeof(IStartUpMarker);
        var typeEnumerableServiceDescriptor = typeof(IEnumerable<ServiceDescriptor>);

        foreach (var descriptors in serviceProvider.GetServices<ServiceDescriptor>())
        {
            var serviceType = descriptors.ServiceType;
            var implementationType = descriptors.ImplementationType;

            if (
                serviceType.IsAssignableTo(typeStartUpMarker)
                || implementationType?.IsAssignableTo(typeStartUpMarker) == true
                || serviceType.IsAssignableTo(typeEnumerableServiceDescriptor)
                || implementationType?.IsAssignableTo(typeEnumerableServiceDescriptor) == true
            )
            {
                continue;
            }

            services.Add(descriptors);
        }

        return services;
    }

    private sealed class MarkerCoreServices : IStartUpMarker;

    internal static bool IsServiceTypeRegistered<T>(this IServiceCollection builder)
        where T : class => builder.Any(x => x.ServiceType == typeof(T));
}
