namespace NetEvolve.ForgingBlazor;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility;

/// <summary>
/// Internal implementation of <see cref="IForgingBlazorApplicationBuilder"/> that configures and builds a ForgingBlazor application.
/// </summary>
internal sealed class ForgingBlazorApplicationBuilder : IForgingBlazorApplicationBuilder
{
    /// <summary>
    /// The underlying ASP.NET Core web application builder.
    /// </summary>
    private readonly WebApplicationBuilder _builder;

    /// <summary>
    /// Gets the service collection for registering application services.
    /// </summary>
    internal IServiceCollection Services => _builder.Services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgingBlazorApplicationBuilder"/> class with the specified command-line arguments.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="args"/> is null.</exception>
    public ForgingBlazorApplicationBuilder([NotNull] string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        _builder = WebApplication.CreateBuilder(args);

        AddConfigurationSettings(_builder);
    }

    /// <summary>
    /// Builds and returns a configured <see cref="IForgingBlazorApplication"/> instance.
    /// </summary>
    /// <returns>A configured <see cref="ForgingBlazorApplication"/> instance ready to run.</returns>
    public IForgingBlazorApplication Build() => new ForgingBlazorApplication(_builder.Build());

    /// <summary>
    /// Configures the application's configuration settings from various sources including JSON, YAML, environment variables, and user secrets.
    /// </summary>
    private static void AddConfigurationSettings(WebApplicationBuilder builder)
    {
        var environment = builder.Environment.EnvironmentName;
        // Add configuration files and environment variables
        // The later sources override the earlier ones
        _ = builder
            .Configuration.AddJsonFile("forgingblazor.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"forgingblazor.{environment}.json", optional: true, reloadOnChange: true)
            .AddYamlFile("forgingblazor.yaml", optional: true, reloadOnChange: true)
            .AddYamlFile($"forgingblazor.{environment}.yaml", optional: true, reloadOnChange: true)
            .AddYamlFile("forgingblazor.yml", optional: true, reloadOnChange: true)
            .AddYamlFile($"forgingblazor.{environment}.yml", optional: true, reloadOnChange: true)
            // Prefix all environment variables with "FORGINGBLAZOR_"
            .AddEnvironmentVariables("FORGINGBLAZOR_");

        // Add user secrets in development environment
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is not null)
        {
            _ = builder.Configuration.AddUserSecrets(entryAssembly, optional: true, reloadOnChange: true);
        }
    }
}
