namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Testcontainers.Azurite;
using TUnit.Core.Interfaces;

/// <summary>
/// Shared Azurite container fixture for all blob storage tests.
/// </summary>
public sealed class AzuriteFixture : IAsyncInitializer, IAsyncDisposable
{
    private readonly AzuriteContainer _container = new AzuriteBuilder(
        /*dockerimage*/"mcr.microsoft.com/azure-storage/azurite:3.35.0"
    )
        .WithLogger(NullLogger.Instance)
        .WithCommand("--skipApiVersionCheck")
        .Build();

    internal string ConnectionString => _container.GetConnectionString();

    public async ValueTask DisposeAsync() => await _container.DisposeAsync().ConfigureAwait(false);

    public async Task InitializeAsync() => await _container.StartAsync().ConfigureAwait(false);
}
