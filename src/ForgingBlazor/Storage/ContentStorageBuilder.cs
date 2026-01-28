namespace NetEvolve.ForgingBlazor.Storage;

using System;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Storage.FileSystem;

/// <summary>
/// Builder for configuring content storage providers.
/// </summary>
internal sealed class ContentStorageBuilder : IContentStorageBuilder
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentStorageBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection to register storage providers with.</param>
    internal ContentStorageBuilder(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        _services = services;
    }

    /// <inheritdoc />
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public IContentStorageBuilder UseFileSystem(Action<IFileSystemStorageOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var options = new FileSystemStorageOptions();
        configure(options);

        _ = _services.AddSingleton<IContentStorageProvider>(sp => new FileSystemContentStorageProvider(options));

        return this;
    }
}
