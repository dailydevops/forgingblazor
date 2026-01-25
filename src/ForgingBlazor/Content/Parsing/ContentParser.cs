namespace NetEvolve.ForgingBlazor.Content.Parsing;

using System;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Content.Validation;

/// <summary>
/// Orchestrates the parsing of Markdown content with YAML frontmatter into content descriptors.
/// </summary>
internal static class ContentParser
{
    /// <summary>
    /// Parses Markdown content with frontmatter into a content descriptor.
    /// </summary>
    /// <typeparam name="TDescriptor">The content descriptor type to create.</typeparam>
    /// <param name="markdownContent">The complete Markdown content including frontmatter.</param>
    /// <returns>A populated content descriptor instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="markdownContent"/> is <see langword="null"/>.</exception>
    /// <exception cref="ContentValidationException">Thrown when frontmatter validation fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when parsing fails.</exception>
    internal static TDescriptor Parse<TDescriptor>(string markdownContent)
        where TDescriptor : ContentDescriptor, new()
    {
        ArgumentNullException.ThrowIfNull(markdownContent);

        var (frontmatter, markdownBody) = FrontmatterParser.Parse(markdownContent);

        FrontmatterValidation.ValidateRequiredFields(frontmatter);

        var htmlBody = MarkdownRenderer.Render(markdownBody);

        return ContentDescriptorFactory.Create<TDescriptor>(frontmatter, markdownBody, htmlBody);
    }
}
