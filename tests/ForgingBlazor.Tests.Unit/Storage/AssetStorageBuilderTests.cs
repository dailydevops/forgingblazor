namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System;
using System.Runtime.Versioning;
using global::NetEvolve.ForgingBlazor.Storage;
using global::NetEvolve.ForgingBlazor.Storage.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Assertions.Extensions;
using TUnit.Core;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public sealed class AssetStorageBuilderTests
{
    [Test]
    public async Task UseFileSystem_WithValidConfiguration_RegistersProvider()
    {
        var services = new ServiceCollection();
        var builder = new AssetStorageBuilder(services);

        var result = builder.UseFileSystem(options =>
        {
            _ = options.WithBasePath("test-assets");
        });

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(builder);
            _ = await Assert.That(services.Count).IsEqualTo(1);

            var descriptor = services[0];
            _ = await Assert.That(descriptor.ServiceType).IsEqualTo(typeof(IAssetStorageProvider));
            _ = await Assert.That(descriptor.Lifetime).IsEqualTo(ServiceLifetime.Singleton);
        }
    }

    [Test]
    public async Task UseFileSystem_WhenConfigureNull_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();
        var builder = new AssetStorageBuilder(services);

        _ = await Assert
            .That(() => builder.UseFileSystem(null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("configure");
    }

    [Test]
    public async Task Constructor_WhenServicesNull_ThrowsArgumentNullException() =>
        _ = await Assert
            .That(() => new AssetStorageBuilder(null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("services");

    [Test]
    public async Task UseFileSystem_RegistersFileSystemAssetStorageProvider()
    {
        var services = new ServiceCollection();
        var builder = new AssetStorageBuilder(services);

        _ = builder.UseFileSystem(options =>
        {
            _ = options.WithBasePath("assets");
        });

        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IAssetStorageProvider>();

        using (Assert.Multiple())
        {
            _ = await Assert.That(provider).IsNotNull();
            _ = await Assert.That(provider).IsTypeOf<FileSystemAssetStorageProvider>();
        }
    }
}
