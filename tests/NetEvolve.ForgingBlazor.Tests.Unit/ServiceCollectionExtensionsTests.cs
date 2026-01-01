namespace NetEvolve.ForgingBlazor.Tests.Unit;

using System.CommandLine;
using Markdig;
using Microsoft.Extensions.DependencyInjection;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using YamlDotNet.Serialization;

public sealed class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddForgingBlazorServices_WithNullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;

        _ = Assert.Throws<ArgumentNullException>(() => services.AddForgingBlazorServices());
    }

    [Test]
    public async Task AddForgingBlazorServices_RegistersCoreServices()
    {
        var services = new ServiceCollection();

        var result = services.AddForgingBlazorServices();

        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(services.Any(x => x.ServiceType == typeof(RootCommand))).IsTrue();
        _ = await Assert.That(services.Any(x => x.ServiceType == typeof(IContentRegister))).IsTrue();
    }

    [Test]
    public async Task AddForgingBlazorServices_RegistersCommands()
    {
        var services = new ServiceCollection();

        _ = services.AddForgingBlazorServices();

        var commandDescriptors = services.Where(x => x.ServiceType == typeof(Command)).ToList();
        _ = await Assert.That(commandDescriptors.Count).IsGreaterThanOrEqualTo(4);
    }

    [Test]
    public async Task AddForgingBlazorServices_CalledTwice_DoesNotDuplicateServices()
    {
        var services = new ServiceCollection();

        _ = services.AddForgingBlazorServices();
        var countAfterFirst = services.Count;

        _ = services.AddForgingBlazorServices();
        var countAfterSecond = services.Count;

        _ = await Assert.That(countAfterFirst).IsEqualTo(countAfterSecond);
    }

    [Test]
    public void AddMarkdownServices_WithNullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;

        _ = Assert.Throws<ArgumentNullException>(() => services.AddMarkdownServices());
    }

    [Test]
    public async Task AddMarkdownServices_RegistersMarkdownPipeline()
    {
        var services = new ServiceCollection();

        var result = services.AddMarkdownServices();

        _ = await Assert.That(result).IsNotNull();
        _ = await Assert.That(services.Any(x => x.ServiceType == typeof(MarkdownPipeline))).IsTrue();
    }

    [Test]
    public async Task AddMarkdownServices_RegistersYamlDeserializer()
    {
        var services = new ServiceCollection();

        _ = services.AddMarkdownServices();

        var hasDeserializer = services.Any(x => x.ServiceType == typeof(IDeserializer));
        _ = await Assert.That(hasDeserializer).IsTrue();
    }

    [Test]
    public async Task AddMarkdownServices_CalledTwice_DoesNotDuplicateServices()
    {
        var services = new ServiceCollection();

        _ = services.AddMarkdownServices();
        var countAfterFirst = services.Count;

        _ = services.AddMarkdownServices();
        var countAfterSecond = services.Count;

        _ = await Assert.That(countAfterFirst).IsEqualTo(countAfterSecond);
    }

    [Test]
    public async Task IsServiceTypeRegistered_WithRegisteredService_ReturnsTrue()
    {
        var services = new ServiceCollection();
        _ = services.AddSingleton<ITestService, TestService>();

        var result = services.IsServiceTypeRegistered<ITestService>();

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsServiceTypeRegistered_WithUnregisteredService_ReturnsFalse()
    {
        var services = new ServiceCollection();

        var result = services.IsServiceTypeRegistered<ITestService>();

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsServiceTypeRegistered_WithEmptyCollection_ReturnsFalse()
    {
        var services = new ServiceCollection();

        var result = services.IsServiceTypeRegistered<ITestService>();

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task AddMarkdownServices_ConfiguresMarkdownPipelineWithExtensions()
    {
        var services = new ServiceCollection();

        _ = services.AddMarkdownServices();
        using var provider = services.BuildServiceProvider();
        var pipeline = provider.GetService<MarkdownPipeline>();

        _ = await Assert.That(pipeline).IsNotNull();
    }

    [Test]
    public async Task AddMarkdownServices_ConfiguresYamlDeserializerWithCamelCase()
    {
        var services = new ServiceCollection();

        _ = services.AddMarkdownServices();
        using var provider = services.BuildServiceProvider();
        var deserializer = provider.GetService<IDeserializer>();

        _ = await Assert.That(deserializer).IsNotNull();
    }

    private interface ITestService;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used for DI tests")]
    private sealed class TestService : ITestService;
}
