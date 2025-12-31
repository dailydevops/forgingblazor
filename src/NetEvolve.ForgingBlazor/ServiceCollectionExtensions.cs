namespace NetEvolve.ForgingBlazor;

using System.CommandLine;
using Markdig;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Commands;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register and configure ForgingBlazor services.
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the core ForgingBlazor services including CLI commands and content registration.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to register services into.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method registers all core ForgingBlazor services, including:
    /// <list type="bullet">
    /// <item><description>The <see cref="RootCommand"/> (implemented by <see cref="CommandCli"/>)</description></item>
    /// <item><description>Sub-commands: <see cref="CommandBuild"/>, <see cref="CommandCreate"/>, <see cref="CommandExample"/>, <see cref="CommandServe"/></description></item>
    /// <item><description>The <see cref="IContentRegister"/> service (implemented by <see cref="ContentRegister"/>)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// If services have already been registered (identified by the presence of the <see cref="MarkerCoreServices"/> marker),
    /// the method returns immediately without registering duplicate services.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <see langword="null"/>.</exception>
    internal static IServiceCollection AddForgingBlazorServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (!services.IsServiceTypeRegistered<MarkerCoreServices>())
        {
            _ = services
                .AddSingleton<MarkerCoreServices>()
                // Register RootCommand
                .AddSingleton<RootCommand, CommandCli>()
                // Register all Standard Commands
                .AddSingleton<Command, CommandBuild>()
                .AddSingleton<Command, CommandCreate>()
                .AddSingleton<Command, CommandExample>()
                .AddSingleton<Command, CommandServe>()
                // Register core services
                .AddSingleton<IContentRegister, ContentRegister>();
        }

        return services;
    }

    /// <summary>
    /// Internal marker class used to track that core ForgingBlazor services have been registered.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This sealed class implements <see cref="IStartUpMarker"/> to prevent duplicate service registrations
    /// and identify startup-specific services during the filtering process.
    /// </para>
    /// <para>
    /// The marker is registered as a singleton in the service collection to indicate that core services
    /// have already been initialized.
    /// </para>
    /// </remarks>
    private sealed class MarkerCoreServices : IStartUpMarker;

    /// <summary>
    /// Registers markdown processing services including Markdig pipeline and YAML deserialization.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to register services into.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method registers the following services:
    /// <list type="bullet">
    /// <item><description>A <see cref="MarkdownPipeline"/> configured with advanced extensions and YAML front matter support</description></item>
    /// <item><description>An <see cref="IDeserializer"/> configured with camelCase naming convention and property ignoring</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// If services have already been registered (identified by the presence of the <see cref="MarkerMarkdownServices"/> marker),
    /// the method returns immediately without registering duplicate services.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <see langword="null"/>.</exception>
    internal static IServiceCollection AddMarkdownServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        if (!services.IsServiceTypeRegistered<MarkerMarkdownServices>())
        {
            _ = services
                .AddSingleton<MarkerMarkdownServices>()
                // Register Markdown services
                .AddSingleton(new MarkdownPipelineBuilder().UseAdvancedExtensions().UseYamlFrontMatter().Build())
                .AddSingleton(
                    new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .IgnoreUnmatchedProperties()
                        .Build()
                );
        }
        return services;
    }

    private sealed class MarkerMarkdownServices : IStartUpMarker;

    /// <summary>
    /// Determines whether a specific service type has been registered in the service collection.
    /// </summary>
    /// <typeparam name="T">
    /// The service type to check for registration. Cannot be <see langword="null"/>.
    /// </typeparam>
    /// <param name="builder">
    /// The <see cref="IServiceCollection"/> to check for service registration.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the service type <typeparamref name="T"/> is registered; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This utility method checks whether a service type has already been registered by comparing
    /// the <see cref="ServiceDescriptor.ServiceType"/> property against the provided type parameter.
    /// </para>
    /// <para>
    /// This method is commonly used to prevent duplicate service registrations and to conditionally register
    /// services only if they have not been previously registered.
    /// </para>
    /// </remarks>
    internal static bool IsServiceTypeRegistered<T>(this IServiceCollection builder)
        where T : class => builder.Any(x => x.ServiceType == typeof(T));
}
