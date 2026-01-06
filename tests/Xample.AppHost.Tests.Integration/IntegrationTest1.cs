namespace NetEvolve.Xample.AppHost.Tests.Integration;

using System.Net;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class IntegrationTest1
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Test]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var cancellationToken = TestContext.Current!.Execution.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Xample_AppHost>(
            cancellationToken
        );
        _ = appHost.Services.AddLogging(logging =>
        {
            _ = logging
                .SetMinimumLevel(LogLevel.Debug)
                // Override the logging filters from the app's configuration
                .AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug)
                .AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        });
        _ = appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
            _ = clientBuilder.AddStandardResilienceHandler()
        );

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        using var httpClient = app.CreateHttpClient(ProjectNames.Xample);
        _ = await app
            .ResourceNotifications.WaitForResourceHealthyAsync(ProjectNames.Xample, cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken)
            .ConfigureAwait(false);
#pragma warning disable CA2234 // Pass system uri objects instead of strings
        using var response = await httpClient.GetAsync("/", cancellationToken);
#pragma warning restore CA2234 // Pass system uri objects instead of strings

        // Assert
        _ = await Assert.That(response.StatusCode).EqualTo(HttpStatusCode.OK);
    }
}
