namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

public class Phase1InterfaceContractTests
{
    [Test]
    public async Task IRoutingBuilder_IsPublic()
    {
        var type = typeof(IRoutingBuilder);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IRootConfiguration_IsPublic()
    {
        var type = typeof(IRootConfiguration);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task ICultureConfiguration_IsPublic()
    {
        var type = typeof(ICultureConfiguration);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task ISegmentConfiguration_IsPublic()
    {
        var type = typeof(ISegmentConfiguration);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IPageConfiguration_IsPublic()
    {
        var type = typeof(IPageConfiguration);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IPaginationConfiguration_IsPublic()
    {
        var type = typeof(IPaginationConfiguration);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IMetadataConfiguration_IsPublic()
    {
        var type = typeof(IMetadataConfiguration);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IContentStorageProvider_IsPublic()
    {
        var type = typeof(IContentStorageProvider);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IAssetStorageProvider_IsPublic()
    {
        var type = typeof(IAssetStorageProvider);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IContentStorageBuilder_IsPublic()
    {
        var type = typeof(IContentStorageBuilder);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IAssetStorageBuilder_IsPublic()
    {
        var type = typeof(IAssetStorageBuilder);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IFileSystemStorageOptions_IsPublic()
    {
        var type = typeof(IFileSystemStorageOptions);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task IPublishingService_IsPublic()
    {
        var type = typeof(IPublishingService);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsTrue();
    }

    [Test]
    public async Task ContentDescriptor_IsPublic()
    {
        var type = typeof(ContentDescriptor);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsFalse();
    }

    [Test]
    public async Task ResolvedContent_IsPublic()
    {
        var type = typeof(ResolvedContent<ContentDescriptor>);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsInterface).IsFalse();
    }

    [Test]
    public async Task CultureCanonical_IsPublic()
    {
        var type = typeof(CultureCanonical);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsEnum).IsTrue();
    }

    [Test]
    public async Task PaginationUrlFormat_IsPublic()
    {
        var type = typeof(PaginationUrlFormat);

        _ = await Assert.That(type.IsPublic).IsTrue();
        _ = await Assert.That(type.IsEnum).IsTrue();
    }
}
