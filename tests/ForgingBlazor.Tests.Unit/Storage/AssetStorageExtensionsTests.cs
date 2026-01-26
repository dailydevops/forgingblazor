namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System;
using System.Runtime.Versioning;
using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Storage;
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
    public void AddAssetStorage_WhenBuilderNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => AssetStorageExtensions.AddAssetStorage(null!, _ => { })
        );

    [Test]
    public void AddAssetStorage_WhenConfigureNull_ThrowsArgumentNullException()
    {
        var builder = new ForgingBlazorApplicationBuilder(Array.Empty<string>());

        _ = Assert.Throws<ArgumentNullException>("configure", () => builder.AddAssetStorage(null!));
    }
}
