namespace NetEvolve.ForgingBlazor.Commands;

using System;
using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Configurations;
using NetEvolve.ForgingBlazor.Extensibility;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Commands;

/// <summary>
/// Provides the <b>build</b> command implementation for building and generating static content from a Forging Blazor application.
/// </summary>
/// <remarks>
/// <para>
/// This sealed class implements the <c>build</c> command that processes pages and generates static site output based on
/// configuration options. It registers standard options for environment, drafts, future content, logging, and output paths.
/// </para>
/// <para>
/// The command uses the service provider to transfer services and invokes the content register to collect and process all registered content.
/// </para>
/// </remarks>
/// <seealso cref="CommandOptions"/>
/// <seealso cref="IStartUpMarker"/>
internal sealed class CommandBuild : Command, IStartUpMarker
{
    /// <summary>
    /// Stores the service provider for transferring services during command execution.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBuild"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">
    /// The <see cref="IServiceProvider"/> instance providing access to registered application services.
    /// This is used to transfer services for command execution.
    /// </param>
    public CommandBuild(IServiceProvider serviceProvider)
        : base("build", "Builds and generates static content for a Forging Blazor application.")
    {
        _serviceProvider = serviceProvider;

        Add(CommandOptions.ContentPath);
        Add(CommandOptions.Environment);
        Add(CommandOptions.IncludeDrafts);
        Add(CommandOptions.IncludeFuture);
        Add(CommandOptions.ProjectPath);
        Add(CommandOptions.OutputPath);

        SetAction(ExecuteAsync);
    }

    /// <summary>
    /// Executes the <c>build</c> command asynchronously.
    /// </summary>
    /// <param name="parseResult">
    /// The <see cref="ParseResult"/> containing parsed command-line arguments.
    /// Cannot be <see langword="null"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to request cancellation of the build operation.
    /// Defaults to <see cref="CancellationToken.None"/> if not specified.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation.
    /// The task result contains an exit code: 0 for success, 1 for failure.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs the following steps:
    /// <list type="number">
    /// <item><description>Transfers all non-startup services from the parent service provider</description></item>
    /// <item><description>Builds a new service provider from the transferred services</description></item>
    /// <item><description>Retrieves the <see cref="IContentRegister"/> service</description></item>
    /// <item><description>Invokes <see cref="IContentRegister.CollectAsync"/> asynchronously</description></item>
    /// <item><description>Returns 0 on success, 1 on any exception</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private async Task<int> ExecuteAsync(ParseResult parseResult, CancellationToken cancellationToken)
    {
        var environment = parseResult.GetValue(CommandOptions.Environment);
        var projectPath = parseResult.GetValue(CommandOptions.ProjectPath);

        ArgumentException.ThrowIfNullOrWhiteSpace(environment);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);

        var services = new ServiceCollection()
            .TransferAllServices(_serviceProvider)
            .AddSingleton(_ => ConfigurationLoader.Load(environment, projectPath));

        _ = services
            .ConfigureOptions<SiteConfigurationConfigure>()
            .Configure<SiteConfiguration>(siteconfig =>
            {
                var contentPath = parseResult.GetValue(CommandOptions.ContentPath);
                if (!string.IsNullOrWhiteSpace(contentPath))
                {
                    siteconfig.ContentPath = contentPath;
                }
            });

        var serviceProvider = services.BuildServiceProvider();

        var register = serviceProvider.GetRequiredService<IContentRegister>();

        try
        {
            await register.CollectAsync(cancellationToken).ConfigureAwait(false);

            return 0;
        }
        catch (Exception)
        {
            return 1;
        }
    }
}
