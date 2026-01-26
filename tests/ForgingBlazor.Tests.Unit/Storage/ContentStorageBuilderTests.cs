namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Storage;
using NetEvolve.ForgingBlazor.Storage.FileSystem;
using TUnit.Assertions.Extensions;
using TUnit.Core;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public sealed class ContentStorageBuilderTests
{
    [Test]
    public async Task UseFileSystem_WithValidConfiguration_RegistersProvider()
    {
        var services = new ServiceCollection();
        var builder = new ContentStorageBuilder(services);

        var result = builder.UseFileSystem(options =>
        {
            _ = options.WithBasePath("test-content");
        });

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(builder);
            _ = await Assert.That(services.Count).IsEqualTo(1);

            var descriptor = services[0];
            _ = await Assert.That(descriptor.ServiceType).IsEqualTo(typeof(IContentStorageProvider));
            _ = await Assert.That(descriptor.Lifetime).IsEqualTo(ServiceLifetime.Singleton);
        }
    }

    [Test]
    public void UseFileSystem_WhenConfigureNull_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();
        var builder = new ContentStorageBuilder(services);

        _ = Assert.Throws<ArgumentNullException>("configure", () => builder.UseFileSystem(null!));
    }

    [Test]
    public void Constructor_WhenServicesNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>("services", () => _ = new ContentStorageBuilder(null!));

    [Test]
    public async Task UseFileSystem_RegistersFileSystemContentStorageProvider()
    {
        var services = new ServiceCollection();
        var builder = new ContentStorageBuilder(services);

        _ = builder.UseFileSystem(options =>
        {
            _ = options.WithBasePath("content");
        });

        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IContentStorageProvider>();

        using (Assert.Multiple())
        {
            _ = await Assert.That(provider).IsNotNull();
            _ = await Assert.That(provider).IsTypeOf<FileSystemContentStorageProvider>();
        }
    }
}
