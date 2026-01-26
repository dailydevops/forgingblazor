namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NetEvolve.ForgingBlazor.Storage;
using NetEvolve.ForgingBlazor.Storage.FileSystem;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="FileSystemWatcherService"/>.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public sealed class FileSystemWatcherServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private FileSystemStorageOptions _options = null!;

    public FileSystemWatcherServiceTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "ForgingBlazor.Tests.Watcher", Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(_testDirectory);
    }

    [Before(Test)]
    public void Setup()
    {
        _options = new FileSystemStorageOptions();
        _ = _options.WithBasePath(_testDirectory);
    }

    [After(Test)]
    public void Cleanup()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    public void Dispose() => Cleanup();

    [Test]
    public async Task StartAsync_WhenWatchingDisabled_DoesNotThrow()
    {
        // Arrange - WatchForChanges not called means watching is disabled
        using var service = new FileSystemWatcherService(_options, NullLogger<FileSystemWatcherService>.Instance);

        // Act & Assert - Should not throw
        await service.StartAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Test]
    public async Task StartAsync_WhenWatchingEnabled_StartsSuccessfully()
    {
        // Arrange
        _ = _options.WatchForChanges();
        using var service = new FileSystemWatcherService(_options, NullLogger<FileSystemWatcherService>.Instance);

        // Act & Assert - Should not throw
        await service.StartAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Test]
    public async Task StopAsync_WhenCalled_StopsSuccessfully()
    {
        // Arrange
        _ = _options.WatchForChanges();
        using var service = new FileSystemWatcherService(_options, NullLogger<FileSystemWatcherService>.Instance);

        await service.StartAsync(CancellationToken.None).ConfigureAwait(false);

        // Act & Assert - Should not throw
        await service.StopAsync(CancellationToken.None).ConfigureAwait(false);
    }

    [Test]
    public async Task OnFileChanged_WhenHandlerRegistered_InvokesHandler()
    {
        // Arrange
        _ = _options.WatchForChanges();
        using var service = new FileSystemWatcherService(_options, NullLogger<FileSystemWatcherService>.Instance);

        var changedFiles = new List<string>();
        service.OnFileChanged(filePath => changedFiles.Add(filePath));

        await service.StartAsync(CancellationToken.None).ConfigureAwait(false);

        // Act
        var testFile = Path.Combine(_testDirectory, "test.md");
        await File.WriteAllTextAsync(testFile, "test content").ConfigureAwait(false);

        // Give the watcher and debounce timer time to process
        await Task.Delay(500).ConfigureAwait(false);

        // Assert
        _ = await Assert.That(changedFiles.Count).IsGreaterThan(0);
    }
}
