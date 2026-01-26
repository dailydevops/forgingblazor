namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using DotNet.Testcontainers.Containers;
using NetEvolve.ForgingBlazor.Content;
using Testcontainers.Azurite;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Integration tests for <see cref="AzureBlobContentStorageProvider"/> using Azurite container via Testcontainers.
/// </summary>
/// <remarks>
/// These tests automatically start and manage an Azurite container for testing.
/// No manual setup is required.
/// </remarks>
public sealed class AzureBlobContentStorageProviderTests
{
    private const string TestContainerName = "content-test";

    [Before(Test)]
    public async Task SetupAsync()
    {
        await CreateContainerIfNotExistsAsync();
    }

    [After(Test)]
    public async Task CleanupAsync()
    {
        await DeleteContainerAsync();
    }

    [Test]
    public async Task Constructor_WithValidConnectionString_CreatesProvider()
    {
        // Arrange & Act
        var provider = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, TestContainerName);

        // Assert
        _ = await Assert.That(provider).IsNotNull();
    }

    [Test]
    public void Constructor_WithNullConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
#pragma warning disable CA1806 // Do not ignore method results
        _ = Assert.Throws<ArgumentException>(() => _ = new AzureBlobContentStorageProvider(null!, TestContainerName));
#pragma warning restore CA1806 // Do not ignore method results
    }

    [Test]
    public void Constructor_WithEmptyConnectionString_ThrowsArgumentException()
    {
        // Act & Assert
#pragma warning disable CA1806 // Do not ignore method results
        _ = Assert.Throws<ArgumentException>(() =>
            _ = new AzureBlobContentStorageProvider(string.Empty, TestContainerName)
        );
#pragma warning restore CA1806 // Do not ignore method results
    }

    [Test]
    public void Constructor_WithNullContainerName_ThrowsArgumentException()
    {
        // Act & Assert
#pragma warning disable CA1806 // Do not ignore method results
        _ = Assert.Throws<ArgumentException>(() =>
            _ = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, null!)
        );
#pragma warning restore CA1806 // Do not ignore method results
    }

    [Test]
    public async Task GetContentAsync_WhenBlobExists_ReturnsContent()
    {
        // Arrange
        var provider = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, TestContainerName);
        var culture = CultureInfo.GetCultureInfo("en-US");
        var segmentPath = "blog";
        var slug = "test-post";
        var markdown = """
            ---
            title: Test Post
            slug: test-post
            publisheddate: 2026-01-25T10:00:00Z
            ---
            # Test Content
            This is a test post.
            """;

        await UploadBlobAsync($"{segmentPath}/EN-US/{slug}.md", markdown);

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
            _ = await Assert.That(result.Body).Contains("This is a test post");
        }
    }

    [Test]
    public async Task GetContentAsync_WhenBlobDoesNotExist_ReturnsNull()
    {
        // Arrange
        var provider = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, TestContainerName);
        var culture = CultureInfo.GetCultureInfo("en-US");

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
    public async Task GetContentsAsync_WhenMultipleBlobsExist_ReturnsAll()
    {
        // Arrange
        var provider = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, TestContainerName);
        var culture = CultureInfo.GetCultureInfo("en-US");
        var segmentPath = "blog";

        var markdown1 = """
            ---
            title: Post 1
            slug: post-1
            publisheddate: 2026-01-25T10:00:00Z
            ---
            # Post 1
            """;
        var markdown2 = """
            ---
            title: Post 2
            slug: post-2
            publisheddate: 2026-01-25T11:00:00Z
            ---
            # Post 2
            """;

        await UploadBlobAsync($"{segmentPath}/EN-US/post-1.md", markdown1);
        await UploadBlobAsync($"{segmentPath}/EN-US/post-2.md", markdown2);

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
            _ = await Assert.That(results.Any(x => x.Slug == "post-1")).IsTrue();
            _ = await Assert.That(results.Any(x => x.Slug == "post-2")).IsTrue();
        }
    }

    [Test]
    public async Task GetContentsAsync_WhenNoBlobsExist_ReturnsEmpty()
    {
        // Arrange
        var provider = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, TestContainerName);
        var culture = CultureInfo.GetCultureInfo("en-US");

        // Act
        var results = await provider.GetContentsAsync<TestContentDescriptor>("empty", culture, CancellationToken.None);

        // Assert
        _ = await Assert.That(results.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ExistsAsync_WhenBlobExists_ReturnsTrue()
    {
        // Arrange
        var provider = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, TestContainerName);
        var path = "test/file.md";
        await UploadBlobAsync(path, "test content");

        // Act
        var result = await provider.ExistsAsync(path, CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenBlobDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var provider = new AzureBlobContentStorageProvider(AzuriteFixture.ConnectionString, TestContainerName);

        // Act
        var result = await provider.ExistsAsync("nonexistent.md", CancellationToken.None);

        // Assert
        _ = await Assert.That(result).IsFalse();
    }

    private static async Task CreateContainerIfNotExistsAsync()
    {
        var client = new BlobServiceClient(AzuriteFixture.ConnectionString);
        var containerClient = client.GetBlobContainerClient(TestContainerName);
        _ = await containerClient.CreateIfNotExistsAsync();
    }

    private static async Task DeleteContainerAsync()
    {
        var client = new BlobServiceClient(AzuriteFixture.ConnectionString);
        var containerClient = client.GetBlobContainerClient(TestContainerName);
        _ = await containerClient.DeleteIfExistsAsync();
    }

    private static async Task UploadBlobAsync(string blobPath, string content)
    {
        var client = new BlobServiceClient(AzuriteFixture.ConnectionString);
        var containerClient = client.GetBlobContainerClient(TestContainerName);
        var blobClient = containerClient.GetBlobClient(blobPath);
        await blobClient.UploadAsync(BinaryData.FromString(content), overwrite: true);
    }

    private sealed class TestContentDescriptor : ContentDescriptor
    {
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public TestContentDescriptor()
        {
            Title = string.Empty;
            Slug = string.Empty;
            PublishedDate = DateTimeOffset.MinValue;
        }
    }
}
