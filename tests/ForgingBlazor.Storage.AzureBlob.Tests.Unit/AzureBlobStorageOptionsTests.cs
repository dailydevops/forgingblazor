namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class AzureBlobStorageOptionsTests
{
    [Test]
    public async Task WithConnectionString_SetsConnectionString()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();
        const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=key;EndpointSuffix=core.windows.net";

        // Act
        var result = options.WithConnectionString(connectionString);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsSameReferenceAs(options);
            _ = await Assert.That(options.ConnectionString).IsEqualTo(connectionString);
        }
    }

    [Test]
    public async Task WithConnectionString_NullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("connectionString", () => options.WithConnectionString(null!));
    }

    [Test]
    public async Task WithConnectionString_EmptyValue_ThrowsArgumentException()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>("connectionString", () => options.WithConnectionString(string.Empty));
    }

    [Test]
    public async Task WithConnectionString_WhitespaceValue_ThrowsArgumentException()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>("connectionString", () => options.WithConnectionString("   "));
    }

    [Test]
    public async Task WithContainerName_SetsContainerName()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();
        const string containerName = "content";

        // Act
        var result = options.WithContainerName(containerName);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsSameReferenceAs(options);
            _ = await Assert.That(options.ContainerName).IsEqualTo(containerName);
        }
    }

    [Test]
    public async Task WithContainerName_NullValue_ThrowsArgumentNullException()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("containerName", () => options.WithContainerName(null!));
    }

    [Test]
    public async Task WithContainerName_EmptyValue_ThrowsArgumentException()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>("containerName", () => options.WithContainerName(string.Empty));
    }

    [Test]
    public async Task WithContainerName_WhitespaceValue_ThrowsArgumentException()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>("containerName", () => options.WithContainerName("   "));
    }

    [Test]
    public async Task AsPublishingTarget_SetsIsPublishingTarget()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act
        var result = options.AsPublishingTarget();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsSameReferenceAs(options);
            _ = await Assert.That(options.IsPublishingTarget).IsTrue();
        }
    }

    [Test]
    public async Task DefaultState_IsPublishingTargetIsFalse()
    {
        // Arrange & Act
        var options = new AzureBlobStorageOptions();

        // Assert
        _ = await Assert.That(options.IsPublishingTarget).IsFalse();
    }

    [Test]
    public async Task FluentChaining_Works()
    {
        // Arrange
        var options = new AzureBlobStorageOptions();

        // Act
        var result = options
            .WithConnectionString(
                "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=key;EndpointSuffix=core.windows.net"
            )
            .WithContainerName("content")
            .AsPublishingTarget();

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsSameReferenceAs(options);
            _ = await Assert.That(options.ConnectionString).IsNotNull();
            _ = await Assert.That(options.ContainerName).IsEqualTo("content");
            _ = await Assert.That(options.IsPublishingTarget).IsTrue();
        }
    }
}
