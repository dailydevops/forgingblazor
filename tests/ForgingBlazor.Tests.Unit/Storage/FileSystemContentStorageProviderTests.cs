namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using NetEvolve.ForgingBlazor.Storage;
using NetEvolve.ForgingBlazor.Storage.FileSystem;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="FileSystemContentStorageProvider"/>.
/// </summary>
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public sealed class FileSystemContentStorageProviderTests : IDisposable
{
    private readonly string _testDirectory;
    private FileSystemStorageOptions _options = null!;

    public FileSystemContentStorageProviderTests()
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
    public async Task GetContentAsync_WhenFileExists_ReturnsContent()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");
        var segmentPath = "blog";
        var slug = "test-post";
        var markdown = """
            ---
            title: Test Post
            slug: test-post
            publisheddate: 2026-01-25
            ---
            # Test Content
            """;

        var culturePath = Path.Combine(_testDirectory, segmentPath, "EN-US");
        _ = Directory.CreateDirectory(culturePath);
        await File.WriteAllTextAsync(Path.Combine(culturePath, $"{slug}.md"), markdown);

        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        var result = await provider.GetContentAsync<TestContentDescriptor>(
            segmentPath,
            slug,
            culture,
            CancellationToken.None
        );

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsNotNull();
            _ = await Assert.That(result!.Title).IsEqualTo("Test Post");
            _ = await Assert.That(result.Slug).IsEqualTo("test-post");
        }
    }

    [Test]
    public async Task GetContentAsync_WhenFileDoesNotExist_ReturnsNull()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("en-US");
        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        var result = await provider.GetContentAsync<TestContentDescriptor>(
            "blog",
            "nonexistent",
            culture,
            CancellationToken.None
        );

        // Assert
        _ = await Assert.That(result).IsNull();
    }

    [Test]
    public async Task GetContentsAsync_WhenMultipleFilesExist_ReturnsAll()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("de-DE");
        var segmentPath = "posts";
        var culturePath = Path.Combine(_testDirectory, segmentPath, "DE-DE");
        _ = Directory.CreateDirectory(culturePath);

        var markdown1 = """
            ---
            title: Post 1
            slug: post-one
            publisheddate: 2026-01-25
            ---
            Content 1
            """;
        var markdown2 = """
            ---
            title: Post 2
            slug: post-two
            publisheddate: 2026-01-24
            ---
            Content 2
            """;

        await File.WriteAllTextAsync(Path.Combine(culturePath, "post-one.md"), markdown1);
        await File.WriteAllTextAsync(Path.Combine(culturePath, "post-two.md"), markdown2);

        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        var results = await provider.GetContentsAsync<TestContentDescriptor>(
            segmentPath,
            culture,
            CancellationToken.None
        );

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(results.Count).IsEqualTo(2);
            _ = await Assert.That(results.Any(x => x.Slug == "post-one")).IsTrue();
            _ = await Assert.That(results.Any(x => x.Slug == "post-two")).IsTrue();
        }
    }

    [Test]
    public async Task GetContentsAsync_WhenNoCultureFolder_ReturnsEmpty()
    {
        // Arrange
        var culture = CultureInfo.GetCultureInfo("fr-FR");
        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        var results = await provider.GetContentsAsync<TestContentDescriptor>("blog", culture, CancellationToken.None);

        // Assert
        _ = await Assert.That(results.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ExistsAsync_WhenFileExists_ReturnsTrue()
    {
        // Arrange
        var path = Path.Combine("blog", "test.md");
        var fullPath = Path.Combine(_testDirectory, path);
        var directory = Path.GetDirectoryName(fullPath)!;
        _ = Directory.CreateDirectory(directory);
        await File.WriteAllTextAsync(fullPath, "content");

        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        var exists = await provider.ExistsAsync(path);

        // Assert
        _ = await Assert.That(exists).IsTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenFileDoesNotExist_ReturnsFalse()
    {
        // Arrange
        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        var exists = await provider.ExistsAsync("nonexistent.md");

        // Assert
        _ = await Assert.That(exists).IsFalse();
    }

    [Test]
    public async Task SaveContentAsync_CreatesFile()
    {
        // Arrange
        var path = Path.Combine("blog", "new-post.md");
        var markdown = "# New Post";

        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        await provider.SaveContentAsync(path, markdown);

        // Assert
        var fullPath = Path.Combine(_testDirectory, path);
        _ = await Assert.That(File.Exists(fullPath)).IsTrue();

        var content = await File.ReadAllTextAsync(fullPath);
        _ = await Assert.That(content).IsEqualTo(markdown);
    }

    [Test]
    public async Task DeleteContentAsync_RemovesFile()
    {
        // Arrange
        var path = Path.Combine("blog", "to-delete.md");
        var fullPath = Path.Combine(_testDirectory, path);
        var directory = Path.GetDirectoryName(fullPath)!;
        _ = Directory.CreateDirectory(directory);
        await File.WriteAllTextAsync(fullPath, "content");

        using var provider = new FileSystemContentStorageProvider(_options);

        // Act
        await provider.DeleteContentAsync(path);

        // Assert
        _ = await Assert.That(File.Exists(fullPath)).IsFalse();
    }

    [Test]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        // Arrange
        var provider = new FileSystemContentStorageProvider(_options);

        // Act & Assert - should not throw
        provider.Dispose();
        provider.Dispose();
    }
}
