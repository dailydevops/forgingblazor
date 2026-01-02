namespace NetEvolve.ForgingBlazor.Tests.Integration.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetEvolve.ForgingBlazor.Configurations;

public sealed class ConfigurationLoaderTests
{
    [Test]
    [MethodDataSource(nameof(GetConfigurationData))]
    public async ValueTask Load_ConfigurationFile_Expected(string projectPath, string? environment)
    {
        var services = new ServiceCollection()
            .AddSingleton(_ => ConfigurationLoader.Load(environment, projectPath))
            .ConfigureOptions<SiteConfigurationConfigure>();
        var serviceProvider = services.BuildServiceProvider();

        var siteConfiguration = serviceProvider.GetService<IOptions<SiteConfiguration>>();

        _ = await Verify(new { siteConfiguration = siteConfiguration?.Value, environment })
            .DontIgnoreEmptyCollections()
            .UseParameters(projectPath, environment)
            .HashParameters();
    }

    internal static IEnumerable<(string, string?)> GetConfigurationData =>
        [
            ("_setup", null),
            ("_setup/JsonOnly", "Development"),
            ("_setup/JsonOnly", ""),
            ("_setup/Mixed", "Development"),
            ("_setup/Mixed", "   "),
            ("_setup/YamlOnly", "Development"),
            ("_setup/YmlOnly", "Development"),
        ];
}
