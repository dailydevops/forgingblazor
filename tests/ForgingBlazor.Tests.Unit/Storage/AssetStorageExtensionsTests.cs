namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System;
using System.Runtime.Versioning;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Storage;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Assertions.Extensions;
using TUnit.Core;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public sealed class AssetStorageExtensionsTests
{
    [Test]
    public async Task AddAssetStorage_WithValidConfiguration_ConfiguresStorage()
    {
        var builder = new ForgingBlazorApplicationBuilder(Array.Empty<string>());

        var result = builder.AddAssetStorage(storageBuilder =>
        {
            _ = storageBuilder.UseFileSystem(options =>
            {
                _ = options.WithBasePath("test-assets");
            });
        });

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(builder);
            _ = await Assert.That(builder.Services.Any(s => s.ServiceType == typeof(IAssetStorageProvider))).IsTrue();
        }
    }

    [Test]
    public async Task AddAssetStorage_WhenBuilderNull_ThrowsArgumentNullException() =>
        _ = await Assert
            .That(() => AssetStorageExtensions.AddAssetStorage(null!, _ => { }))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("builder");

    [Test]
    public async Task AddAssetStorage_WhenConfigureNull_ThrowsArgumentNullException()
    {
        var builder = new ForgingBlazorApplicationBuilder(Array.Empty<string>());

        _ = await Assert
            .That(() => builder.AddAssetStorage(null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("configure");
    }
}
