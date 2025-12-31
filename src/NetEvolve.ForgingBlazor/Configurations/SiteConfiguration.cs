namespace NetEvolve.ForgingBlazor.Configurations;

using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NetEvolve.ForgingBlazor.Extensibility;
using YamlDotNet.Serialization;

/// <summary>
/// Provides configuration settings for the ForgingBlazor site.
/// </summary>
/// <remarks>
/// This class holds the core configuration options for a ForgingBlazor application,
/// including paths and other site-wide settings.
/// </remarks>
public class SiteConfiguration
{
    /// <summary>
    /// Gets or sets the root path where content files are stored for the ForgingBlazor application.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the file system path where markdown content and other
    /// content files are located. If <see langword="null"/>, a default path (typically 'Content') is used.
    /// </value>
    /// <remarks>
    /// This path is relative to the application's working directory. It should point to a directory
    /// containing subdirectories for different content segments (pages, blog posts, etc.).
    /// </remarks>
    [DefaultValue(DefaultPaths.Content)]
    [Description(ContentPathDescription)]
    [JsonPropertyName("contentpath")]
    [YamlMember(Alias = "contentpath", Description = ContentPathDescription)]
    public string? ContentPath { get; set; } = DefaultPaths.Content;
    private const string ContentPathDescription = """
        Defines the root path where content files are stored for the ForgingBlazor application.
        """;
}

internal sealed class SiteConfigurationConfigure : IConfigureOptions<SiteConfiguration>
{
    private readonly IConfiguration _configuration;

    public SiteConfigurationConfigure(IConfiguration configuration) => _configuration = configuration;

    public void Configure(SiteConfiguration options) => _configuration.Bind(options);
}
