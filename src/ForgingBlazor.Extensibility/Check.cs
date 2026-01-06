namespace NetEvolve.ForgingBlazor;

/// <summary>
/// Provides validation methods for common input patterns.
/// </summary>
internal static partial class Check
{
    /// <summary>
    /// Validates whether a <see cref="string"/> is a valid path segment.
    /// </summary>
    /// <param name="pathSegment">The path segment to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="pathSegment"/> is valid; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A valid path segment must satisfy the following requirements:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Must not be <see langword="null"/> or empty</description></item>
    /// <item><description>Must start with an alphanumeric character (checked using <see cref="char.IsLetterOrDigit(char)"/>)</description></item>
    /// <item><description>Must end with an alphanumeric character (checked using <see cref="char.IsLetterOrDigit(char)"/>)</description></item>
    /// <item><description>May contain only alphanumeric characters, hyphens (<c>-</c>), or underscores (<c>_</c>)</description></item>
    /// <item><description>Must have a length between 3 and 70 characters (inclusive)</description></item>
    /// </list>
    /// </remarks>
    internal static bool IsValidPathSegment(string? pathSegment) =>
        pathSegment is not null
        && (3 <= pathSegment.Length) == (pathSegment.Length <= 70)
        && char.IsLetterOrDigit(pathSegment[0])
        && pathSegment.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
        && char.IsLetterOrDigit(pathSegment[^1]);

    /// <summary>
    /// Validates whether a <see cref="string"/> is a valid administration segment.
    /// </summary>
    /// <param name="pathSegment">The administration segment to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="pathSegment"/> is valid; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// A valid administration segment must satisfy the following requirements:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Must not be <see langword="null"/> or empty</description></item>
    /// <item><description>May contain only alphanumeric characters (checked using <see cref="char.IsLetterOrDigit(char)"/>), hyphens (<c>-</c>), or underscores (<c>_</c>)</description></item>
    /// <item><description>Must have a length between 3 and 70 characters (inclusive)</description></item>
    /// </list>
    /// <para>
    /// Unlike <see cref="IsValidPathSegment(string?)"/>, this method does not require the segment to start or end with an alphanumeric character.
    /// </para>
    /// </remarks>
    internal static bool IsValidAdminSegment(string? pathSegment) =>
        pathSegment is not null
        && (3 <= pathSegment.Length) == (pathSegment.Length <= 70)
        && pathSegment.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
}
