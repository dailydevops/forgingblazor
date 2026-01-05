namespace NetEvolve.ForgingBlazor;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetEvolve.ForgingBlazor.Configurations;
using NetEvolve.ForgingBlazor.Extensibility;

/// <summary>
/// Represents a ForgingBlazor application that can be configured and executed.
/// </summary>
public sealed class ForgingBlazorApplication : IForgingBlazorApplication
{
    /// <summary>
    /// The underlying ASP.NET Core web application.
    /// </summary>
    private readonly WebApplication _application;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForgingBlazorApplication"/> class with the specified web application.
    /// </summary>
    /// <param name="application">The configured ASP.NET Core web application.</param>
    internal ForgingBlazorApplication(WebApplication application) => _application = application;

    /// <summary>
    /// Creates a default ForgingBlazor application builder with hosting services and configurations pre-configured.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <returns>A configured <see cref="IForgingBlazorApplicationBuilder"/> instance with default services.</returns>
    public static IForgingBlazorApplicationBuilder CreateDefaultBuilder(string[] args) =>
        new ForgingBlazorApplicationBuilder(args).AddHostingServices().AddConfigurations();

    /// <summary>
    /// Creates an empty ForgingBlazor application builder without any pre-configured services.
    /// </summary>
    /// <param name="args">The command-line arguments passed to the application.</param>
    /// <returns>A bare <see cref="IForgingBlazorApplicationBuilder"/> instance for manual configuration.</returns>
    public static IForgingBlazorApplicationBuilder CreateEmptyBuilder(string[] args) =>
        new ForgingBlazorApplicationBuilder(args);

    /// <summary>
    /// Runs the ForgingBlazor application with the specified root component.
    /// </summary>
    /// <typeparam name="TRootComponent">The type of the root component to render. All members are dynamically accessed.</typeparam>
    public void Run<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] TRootComponent>()
    {
        ConfigureMappings<TRootComponent>(_application);

        _application.Run();
    }

    /// <summary>
    /// Asynchronously runs the ForgingBlazor application with the specified root component.
    /// </summary>
    /// <typeparam name="TRootComponent">The type of the root component to render. All members are dynamically accessed.</typeparam>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation. Default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public Task RunAsync<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] TRootComponent>(
        CancellationToken cancellationToken = default
    )
    {
        ConfigureMappings<TRootComponent>(_application);

        return _application.RunAsync();
    }

    /// <summary>
    /// Configures the application's middleware pipeline and route mappings for the Blazor application.
    /// </summary>
    /// <typeparam name="TRootComponent">The type of the root component to render. All members are dynamically accessed.</typeparam>
    /// <param name="app">The web application to configure.</param>
    private static void ConfigureMappings<
        [DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] TRootComponent
    >(WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            _ = app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            _ = app.UseHsts();
        }
        _ = app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
        _ = app.UseHttpsRedirection();

        _ = app.UseAntiforgery();

        _ = app.MapStaticAssets();
        _ = app.MapRazorComponents<TRootComponent>();
    }
}
