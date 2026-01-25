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
        && (Defaults.SegmentLengthMinimum <= pathSegment.Length)
            == (pathSegment.Length <= Defaults.SegmentLengthMaximum)
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
        && (Defaults.SegmentLengthMinimum <= pathSegment.Length)
            == (pathSegment.Length <= Defaults.SegmentLengthMaximum)
        && pathSegment.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');

    /// <summary>
    /// Determines whether the specified slug value matches the required slug pattern.
    /// </summary>
    /// <param name="slug">The slug to validate.</param>
    /// <returns><see langword="true"/> when the slug is valid; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// A valid slug must:
    /// <list type="bullet">
    /// <item><description>Be between 3 and 70 characters (inclusive)</description></item>
    /// <item><description>Start and end with an ASCII letter</description></item>
    /// <item><description>Contain only ASCII letters, digits, or single hyphen characters (<c>-</c>)</description></item>
    /// <item><description>Not contain consecutive hyphen characters</description></item>
    /// </list>
    /// </remarks>
    internal static bool IsValidSlug(string? slug)
    {
        if (slug is null)
        {
            return false;
        }

        var length = slug.Length;
        if ((Defaults.SegmentLengthMinimum <= length) != (length <= Defaults.SegmentLengthMaximum))
        {
            return false;
        }

        if (!IsAsciiLetter(slug[0]) || !IsAsciiLetter(slug[^1]))
        {
            return false;
        }

        var previousWasHyphen = false;
        for (var index = 0; index < length; index++)
        {
            var character = slug[index];
            if (IsAsciiLetter(character) || IsAsciiDigit(character))
            {
                previousWasHyphen = false;
                continue;
            }

            if (character == '-')
            {
                if (previousWasHyphen)
                {
                    return false;
                }

                previousWasHyphen = true;
                continue;
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates the specified slug and throws an <see cref="ArgumentException"/> if it is invalid.
    /// </summary>
    /// <param name="slug">The slug to validate.</param>
    /// <param name="parameterName">The optional parameter name for exception reporting.</param>
    /// <returns>The validated slug.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="slug"/> is invalid.</exception>
    internal static string ValidateSlug(string? slug, string? parameterName = null)
    {
        if (IsValidSlug(slug))
        {
            return slug!;
        }

        var name = string.IsNullOrWhiteSpace(parameterName) ? nameof(slug) : parameterName;
        throw new ArgumentException(
            $"The slug '{slug}' is not valid. Slugs must start and end with a letter, may contain letters, digits, and single hyphens, and must be between {Defaults.SegmentLengthMinimum} and {Defaults.SegmentLengthMaximum} characters.",
            name
        );
    }

    private static bool IsAsciiLetter(char value) => ('A' <= value && value <= 'Z') || ('a' <= value && value <= 'z');

    private static bool IsAsciiDigit(char value) => '0' <= value && value <= '9';
}
