namespace NetEvolve.ForgingBlazor.Content;

using System.Globalization;

/// <summary>
/// Represents a content file lookup path with culture-specific variations.
/// </summary>
/// <param name="BasePath">The base path without culture suffix (e.g., "posts/my-article").</param>
/// <param name="Extension">The file extension (e.g., ".md").</param>
/// <param name="Culture">The target culture for content lookup.</param>
internal sealed record ContentLookupPath(string BasePath, string Extension, CultureInfo Culture)
{
    /// <summary>
    /// Gets the base path without culture suffix.
    /// </summary>
    public string BasePath { get; init; } = BasePath;

    /// <summary>
    /// Gets the file extension.
    /// </summary>
    public string Extension { get; init; } = Extension;

    /// <summary>
    /// Gets the target culture.
    /// </summary>
    public CultureInfo Culture { get; init; } = Culture;

    /// <summary>
    /// Generates a file path with the specified culture suffix.
    /// </summary>
    /// <param name="cultureSuffix">The culture suffix (e.g., ".de-DE", ".en", or empty string).</param>
    /// <returns>The complete file path.</returns>
    /// <remarks>
    /// Example: BasePath="posts/my-article", Extension=".md", cultureSuffix=".de-DE"
    /// Result: "posts/my-article.de-DE.md"
    /// </remarks>
    public string GeneratePath(string cultureSuffix)
    {
        ArgumentNullException.ThrowIfNull(cultureSuffix);

        return $"{BasePath}{cultureSuffix}{Extension}";
    }
}
