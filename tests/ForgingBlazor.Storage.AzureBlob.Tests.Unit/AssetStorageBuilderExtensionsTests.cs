namespace NetEvolve.ForgingBlazor.Storage.AzureBlob;

using NetEvolve.ForgingBlazor;
using NSubstitute;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class AssetStorageBuilderExtensionsTests
{
    [Test]
    public async Task UseAzureBlobStorage_NullBuilder_ThrowsArgumentNullException()
    {
        // Arrange
        IAssetStorageBuilder builder = null!;

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("builder", () => builder.UseAzureBlobStorage(_ => { }));
    }

    [Test]
    public async Task UseAzureBlobStorage_NullConfigure_ThrowsArgumentNullException()
    {
        // Arrange
        var builder = Substitute.For<IAssetStorageBuilder>();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("configure", () => builder.UseAzureBlobStorage(null!));
    }

    [Test]
    public async Task UseAzureBlobStorage_ValidParameters_CallsConfigure()
    {
        // Arrange
        var builder = Substitute.For<IAssetStorageBuilder>();
        var configureCalled = false;
        IAzureBlobStorageOptions? capturedOptions = null;

        // Act
        var result = builder.UseAzureBlobStorage(options =>
        {
            _ = options
                .WithConnectionString(
                    "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=key;EndpointSuffix=core.windows.net"
                )
                .WithContainerName("content");
            configureCalled = true;
            capturedOptions = options;
        });

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result).IsSameReferenceAs(builder);
            _ = await Assert.That(configureCalled).IsTrue();
            _ = await Assert.That(capturedOptions).IsNotNull();
            _ = await Assert.That(capturedOptions).IsTypeOf<AzureBlobStorageOptions>();
        }
    }
}
