namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using NetEvolve.ForgingBlazor.Storage;
using NetEvolve.ForgingBlazor.Storage.FileSystem;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="FileSystemAssetStorageProvider"/>.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public sealed class FileSystemAssetStorageProviderTests : IDisposable
{
    private readonly string _testDirectory;
    private FileSystemStorageOptions _options = null!;

    public FileSystemAssetStorageProviderTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "ForgingBlazor.Tests", Guid.NewGuid().ToString());
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
    public async Task GetAssetAsync_WhenFileExists_ReturnsStream()
    {
        // Arrange
        var path = Path.Combine("images", "test.jpg");
        var fullPath = Path.Combine(_testDirectory, path);
        var directory = Path.GetDirectoryName(fullPath)!;
        _ = Directory.CreateDirectory(directory);

        var testContent = "fake image content"u8.ToArray();
        await File.WriteAllBytesAsync(fullPath, testContent);

        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act
        var stream = await provider.GetAssetAsync(path);

        // Assert
        _ = await Assert.That(stream).IsNotNull();
        using (stream)
        {
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var content = ms.ToArray();
            _ = await Assert.That(content).IsEquivalentTo(testContent);
        }
    }

    [Test]
    public async Task GetAssetAsync_WhenFileDoesNotExist_ThrowsFileNotFoundException()
    {
        // Arrange
        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act & Assert
        _ = await Assert
            .That(async () => await provider.GetAssetAsync("nonexistent.jpg"))
            .Throws<FileNotFoundException>();
    }

    [Test]
    public async Task GetAssetsAsync_WhenMultipleFilesExist_ReturnsAll()
    {
        // Arrange
        var folder = "images";
        var imagesPath = Path.Combine(_testDirectory, folder);
        _ = Directory.CreateDirectory(imagesPath);

        await File.WriteAllTextAsync(Path.Combine(imagesPath, "image1.jpg"), "content1");
        await File.WriteAllTextAsync(Path.Combine(imagesPath, "image2.png"), "content2");

        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act
        var assets = await provider.GetAssetsAsync(folder);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(assets.Count).IsEqualTo(2);
            _ = await Assert.That(assets.Contains("image1.jpg")).IsTrue();
            _ = await Assert.That(assets.Contains("image2.png")).IsTrue();
        }
    }

    [Test]
    public async Task GetAssetsAsync_WhenNoFiles_ReturnsEmpty()
    {
        // Arrange
        var folder = "empty";
        _ = Directory.CreateDirectory(Path.Combine(_testDirectory, folder));

        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act
        var assets = await provider.GetAssetsAsync(folder);

        // Assert
        _ = await Assert.That(assets.Count).IsEqualTo(0);
    }

    [Test]
    public async Task SaveAssetAsync_CreatesFile()
    {
        // Arrange
        var path = Path.Combine("documents", "test.pdf");
        var testContent = "PDF content"u8.ToArray();

        using var provider = new FileSystemAssetStorageProvider(_options);
        using var stream = new MemoryStream(testContent);

        // Act
        await provider.SaveAssetAsync(path, stream);

        // Assert
        var fullPath = Path.Combine(_testDirectory, path);
        _ = await Assert.That(File.Exists(fullPath)).IsTrue();

        var savedContent = await File.ReadAllBytesAsync(fullPath);
        _ = await Assert.That(savedContent).IsEquivalentTo(testContent);
    }

    [Test]
    public async Task SaveAssetAsync_CreatesDirectoryIfNotExists()
    {
        // Arrange
        var path = Path.Combine("deeply", "nested", "folder", "file.txt");
        var testContent = Encoding.UTF8.GetBytes("content");

        using var provider = new FileSystemAssetStorageProvider(_options);
        using var stream = new MemoryStream(testContent);

        // Act
        await provider.SaveAssetAsync(path, stream);

        // Assert
        var fullPath = Path.Combine(_testDirectory, path);
        _ = await Assert.That(File.Exists(fullPath)).IsTrue();
    }

    [Test]
    public async Task DeleteAssetAsync_RemovesFile()
    {
        // Arrange
        var path = "to-delete.txt";
        var fullPath = Path.Combine(_testDirectory, path);
        await File.WriteAllTextAsync(fullPath, "content");

        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act
        await provider.DeleteAssetAsync(path);

        // Assert
        _ = await Assert.That(File.Exists(fullPath)).IsFalse();
    }

    [Test]
    public async Task DeleteAssetAsync_WhenFileDoesNotExist_DoesNotThrow()
    {
        // Arrange
        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act & Assert - should not throw
        await provider.DeleteAssetAsync("nonexistent.txt");
    }

    [Test]
    public async Task ExistsAsync_WhenFileExists_ReturnsTrue()
    {
        // Arrange
        var path = "existing.txt";
        var fullPath = Path.Combine(_testDirectory, path);
        await File.WriteAllTextAsync(fullPath, "content");

        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act
        var exists = await provider.ExistsAsync(path);

        // Assert
        _ = await Assert.That(exists).IsTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenFileDoesNotExist_ReturnsFalse()
    {
        // Arrange
        using var provider = new FileSystemAssetStorageProvider(_options);

        // Act
        var exists = await provider.ExistsAsync("nonexistent.txt");

        // Assert
        _ = await Assert.That(exists).IsFalse();
    }

    [Test]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var provider = new FileSystemAssetStorageProvider(_options);

        // Act & Assert - should not throw
        provider.Dispose();
        provider.Dispose();
    }
}
