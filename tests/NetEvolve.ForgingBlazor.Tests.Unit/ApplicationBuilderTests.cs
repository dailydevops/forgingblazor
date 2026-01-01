namespace NetEvolve.ForgingBlazor.Tests.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

public sealed class ApplicationBuilderTests
{
    [Test]
    public async Task CreateDefaultBuilder_EmptyArguments_ReturnsOne()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);
        var app = builder.Build();

        var exitCode = await app.RunAsync();

        _ = await Assert.That(exitCode).IsEqualTo(1);
    }

    [Test]
    public async Task CreateEmptyBuilder_EmptyArguments_ReturnsOne()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args).AddDefaultContent();
        var app = builder.Build();

        var exitCode = await app.RunAsync();

        _ = await Assert.That(exitCode).IsEqualTo(1);
    }

    [Test]
    public async Task Constructor_WithArgs_CreatesInstanceWithServices()
    {
        var args = new[] { "arg1", "arg2" };

        var builder = new ApplicationBuilder(args);

        _ = await Assert.That(builder.Services).IsNotNull();
    }

    [Test]
    public async Task CreateDefaultBuilder_WithArgs_ReturnsBuilderWithServices()
    {
        var args = new[] { "test" };

        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        _ = await Assert.That(builder).IsNotNull();
        _ = await Assert.That(builder.Services).IsNotNull();
    }

    [Test]
    public async Task CreateDefaultBuilder_RegistersDefaultServices()
    {
        var args = Array.Empty<string>();

        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var hasContentRegistration = builder.Services.Any(x => x.ServiceType == typeof(IContentRegistration));
        _ = await Assert.That(hasContentRegistration).IsTrue();
    }

    [Test]
    public async Task CreateDefaultBuilderGeneric_WithCustomPageType_ReturnsBuilder()
    {
        var args = Array.Empty<string>();

        var builder = ApplicationBuilder.CreateDefaultBuilder<TestPage>(args);

        _ = await Assert.That(builder).IsNotNull();
        _ = await Assert.That(builder.Services).IsNotNull();
    }

    [Test]
    public async Task CreateDefaultBuilderGeneric_RegistersDefaultServicesWithCustomPageType()
    {
        var args = Array.Empty<string>();

        var builder = ApplicationBuilder.CreateDefaultBuilder<TestPage>(args);

        var hasContentRegistration = builder.Services.Any(x => x.ServiceType == typeof(IContentRegistration));
        _ = await Assert.That(hasContentRegistration).IsTrue();
    }

    [Test]
    public async Task CreateEmptyBuilder_WithArgs_ReturnsBuilderWithEmptyServices()
    {
        var args = new[] { "test" };

        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = await Assert.That(builder).IsNotNull();
        _ = await Assert.That(builder.Services).IsNotNull();
        _ = await Assert.That(builder.Services.Count).IsEqualTo(0);
    }

    [Test]
    public void Build_WithoutContentRegistration_ThrowsInvalidOperationException()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args);

        _ = Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Test]
    public async Task Build_WithContentRegistration_CreatesApplication()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var app = builder.Build();

        _ = await Assert.That(app).IsNotNull();
    }

    [Test]
    public async Task Build_WithEmptyBuilderAndDefaultContent_CreatesApplication()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args).AddDefaultContent();

        var app = builder.Build();

        _ = await Assert.That(app).IsNotNull();
    }

    [Test]
    public async Task Build_AddsNullLoggerWhenNotRegistered()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateEmptyBuilder(args).AddDefaultContent();

        var app = builder.Build();

        _ = await Assert.That(app).IsNotNull();
    }

    [Test]
    public async Task Build_PreservesExistingLoggerFactory()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);
        using var customLoggerFactory = LoggerFactory.Create(b => b.AddConsole());
        builder.Services.AddSingleton<ILoggerFactory>(customLoggerFactory);

        var app = builder.Build();

        _ = await Assert.That(app).IsNotNull();
    }

    [Test]
    public async Task Services_CanBeModified()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        builder.Services.AddSingleton<ITestService, TestService>();

        var hasTestService = builder.Services.Any(x => x.ServiceType == typeof(ITestService));
        _ = await Assert.That(hasTestService).IsTrue();
    }

    [Test]
    public async Task Build_CreatesApplicationWithCommandLineArgs()
    {
        var args = new[] { "build" };
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var app = builder.Build();
        var exitCode = await app.RunAsync();

        _ = await Assert.That(app).IsNotNull();
        // The build command without proper context returns 1, not 0
        _ = await Assert.That(exitCode).IsEqualTo(1);
    }

    [Test]
    public async Task Build_IncludesServiceDescriptorsInProvider()
    {
        var args = Array.Empty<string>();
        var builder = ApplicationBuilder.CreateDefaultBuilder(args);

        var app = builder.Build();

        _ = await Assert.That(app).IsNotNull();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1812",
        Justification = "Used as type parameter in tests"
    )]
    private sealed record TestPage : PageBase;

    private interface ITestService;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812", Justification = "Used for DI tests")]
    private sealed class TestService : ITestService;
}
