namespace NetEvolve.ForgingBlazor.Configurations;

using System.Reflection;
using Microsoft.Extensions.Configuration;
using NetEvolve.ForgingBlazor.Extensibility;

internal static class ConfigurationLoader
{
    public static IConfiguration Load(string? environment, string projectPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);

        if (string.IsNullOrWhiteSpace(environment))
        {
            environment = Defaults.Environment;
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetFullPath(projectPath))
            .AddJsonFile("forgingblazor.json", optional: true, reloadOnChange: true)
            .AddYamlFile("forgingblazor.yaml", optional: true, reloadOnChange: true)
            .AddYamlFile("forgingblazor.yml", optional: true, reloadOnChange: true)
            .AddYamlFile($"forgingblazor.{environment}.yaml", optional: true, reloadOnChange: true)
            .AddYamlFile($"forgingblazor.{environment}.yml", optional: true, reloadOnChange: true)
            .AddJsonFile($"forgingblazor.{environment}.json", optional: true, reloadOnChange: true);

        return configuration.Build();
    }
}
