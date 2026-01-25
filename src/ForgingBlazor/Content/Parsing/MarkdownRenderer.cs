namespace NetEvolve.ForgingBlazor.Content.Parsing;

using System;
using Markdig;

/// <summary>
/// Renders Markdown content to HTML using Markdig.
/// </summary>
internal static class MarkdownRenderer
{
    private static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseTaskLists()
        .UseAutoLinks()
        .UseEmojiAndSmiley()
        .UseGridTables()
        .UsePipeTables()
        .Build();

    /// <summary>
    /// Renders Markdown content to HTML.
    /// </summary>
    /// <param name="markdown">The Markdown content to render.</param>
    /// <returns>The rendered HTML content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="markdown"/> is <see langword="null"/>.</exception>
    internal static string Render(string markdown)
    {
        ArgumentNullException.ThrowIfNull(markdown);

        try
        {
            return Markdown.ToHtml(markdown, _pipeline);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to render Markdown content. Ensure the Markdown syntax is valid.",
                ex
            );
        }
    }
}
