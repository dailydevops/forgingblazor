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
public sealed class ContentStorageExtensionsTests
{
    [Test]
    public async Task AddContentStorage_WithValidConfiguration_ConfiguresStorage()
    {
        var builder = new ForgingBlazorApplicationBuilder(Array.Empty<string>());

        var result = builder.AddContentStorage(storageBuilder =>
        {
            _ = storageBuilder.UseFileSystem(options => _ = options.WithBasePath("test-content"));
        });

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(builder);
            _ = await Assert.That(builder.Services.Any(s => s.ServiceType == typeof(IContentStorageProvider))).IsTrue();
        }
    }

    [Test]
    public void AddContentStorage_WhenBuilderNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "builder",
            () => ContentStorageExtensions.AddContentStorage(null!, _ => { })
        );

    [Test]
    public void AddContentStorage_WhenConfigureNull_ThrowsArgumentNullException()
    {
        var builder = new ForgingBlazorApplicationBuilder(Array.Empty<string>());

        _ = Assert.Throws<ArgumentNullException>("configure", () => builder.AddContentStorage(null!));
    }
}
