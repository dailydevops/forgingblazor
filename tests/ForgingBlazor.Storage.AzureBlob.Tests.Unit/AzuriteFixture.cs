namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

using System;
using System.Threading.Tasks;
using Testcontainers.Azurite;
using TUnit.Core;

/// <summary>
/// Shared Azurite container fixture for all blob storage tests.
/// </summary>
internal static class AzuriteFixture
{
    private static AzuriteContainer? _container;
    private static string _connectionString = string.Empty;

    public static string ConnectionString => _connectionString;

    [Before(Assembly)]
    public static async Task StartAzuriteAsync()
    {
        _container = new AzuriteBuilder()
            .WithImage("mcr.microsoft.com/azure-storage/azurite:3.33.0")
            .WithCommand("azurite-blob", "--blobHost", "0.0.0.0", "--skipApiVersionCheck")
            .Build();

        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();

        // Give Azurite time to fully initialize
        await Task.Delay(TimeSpan.FromSeconds(2));
    }

    [After(Assembly)]
    public static async Task StopAzuriteAsync()
    {
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }
}
