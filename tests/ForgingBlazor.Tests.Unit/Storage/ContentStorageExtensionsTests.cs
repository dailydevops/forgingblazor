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
public sealed class ContentStorageExtensionsTests
{
    [Test]
    public async Task AddContentStorage_WithValidConfiguration_ConfiguresStorage()
    {
        var builder = new ForgingBlazorApplicationBuilder(Array.Empty<string>());

        var result = builder.AddContentStorage(storageBuilder =>
        {
            _ = storageBuilder.UseFileSystem(options =>
            {
                _ = options.WithBasePath("test-content");
            });
        });

        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsEqualTo(builder);
            _ = await Assert.That(builder.Services.Any(s => s.ServiceType == typeof(IContentStorageProvider))).IsTrue();
        }
    }

    [Test]
    public async Task AddContentStorage_WhenBuilderNull_ThrowsArgumentNullException() =>
        _ = await Assert
            .That(() => ContentStorageExtensions.AddContentStorage(null!, _ => { }))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("builder");

    [Test]
    public async Task AddContentStorage_WhenConfigureNull_ThrowsArgumentNullException()
    {
        var builder = new ForgingBlazorApplicationBuilder(Array.Empty<string>());

        _ = await Assert
            .That(() => builder.AddContentStorage(null!))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("configure");
    }
}
