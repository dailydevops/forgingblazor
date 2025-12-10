namespace NetEvolve.ForgingBlazor.Extensibility.Models;

using YamlDotNet.Serialization;

public abstract record PageBase
{
    [YamlMember(
        Alias = "slug",
        Description = "The unique identifier for the page used in URLs. Must be URL-friendly and contain no spaces or special characters. This value should be lowercase and use hyphens to separate words."
    )]
    public string Slug { get; set; }

    [YamlMember(
        Alias = "title",
        Description = "The primary display title of the page. This appears as the main heading and in page metadata. Should be descriptive and concise for optimal readability."
    )]
    public string Title { get; set; }

    [YamlMember(
        Alias = "linkTitle",
        Description = "An optional shortened or alternative title used specifically for navigation links and breadcrumbs. When not specified, the main title is used as fallback. Useful for long titles that need truncation in UI elements."
    )]
    public string? LinkTitle { get; set; }
}
