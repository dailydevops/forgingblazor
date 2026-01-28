namespace NetEvolve.ForgingBlazor.Content.Parsing;

using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

/// <summary>
/// Parses YAML frontmatter from Markdown content.
/// </summary>
internal static class FrontmatterParser
{
    private const string FrontmatterDelimiter = "---";
    private static readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Extracts and parses YAML frontmatter from Markdown content.
    /// </summary>
    /// <param name="markdownContent">The Markdown content including frontmatter.</param>
    /// <returns>
    /// A tuple containing the parsed frontmatter as a dictionary and the remaining Markdown body.
    /// Returns empty dictionary if no valid frontmatter is found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="markdownContent"/> is <see langword="null"/>.</exception>
    internal static (IReadOnlyDictionary<string, object> Frontmatter, string MarkdownBody) Parse(string markdownContent)
    {
        ArgumentNullException.ThrowIfNull(markdownContent);

        using var reader = new StringReader(markdownContent);
        var firstLine = reader.ReadLine();

        if (firstLine?.Trim() != FrontmatterDelimiter)
        {
            return (new Dictionary<string, object>(), markdownContent);
        }

        var yamlLines = new List<string>();
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            if (line.Trim() == FrontmatterDelimiter)
            {
                break;
            }

            yamlLines.Add(line);
        }

        var remainingContent = reader.ReadToEnd();

        if (yamlLines.Count == 0)
        {
            return (new Dictionary<string, object>(), remainingContent);
        }

        try
        {
            var yamlContent = string.Join(Environment.NewLine, yamlLines);
            var frontmatter =
                _deserializer.Deserialize<Dictionary<string, object>>(yamlContent) ?? new Dictionary<string, object>();

            return (frontmatter, remainingContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to parse YAML frontmatter. Ensure the frontmatter is valid YAML syntax.",
                ex
            );
        }
    }
}
