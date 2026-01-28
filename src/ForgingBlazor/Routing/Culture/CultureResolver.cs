namespace NetEvolve.ForgingBlazor.Routing.Culture;

using System.Globalization;

/// <summary>
/// Resolves culture information from various input formats.
/// </summary>
/// <remarks>
/// Supports parsing culture from:
/// - Two-letter ISO 639-1 language code (e.g., "en", "de")
/// - LCID (Locale Identifier, e.g., 1033 for en-US, 1031 for de-DE)
/// - Full culture name (e.g., "en-US", "de-DE")
/// </remarks>
internal static class CultureResolver
{
    /// <summary>
    /// Resolves a culture from a two-letter ISO 639-1 language code.
    /// </summary>
    /// <param name="twoLetterCode">The two-letter ISO 639-1 language code (e.g., "en", "de").</param>
    /// <returns>The resolved <see cref="CultureInfo"/>, or <c>null</c> if the code is invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="twoLetterCode"/> is <c>null</c>.</exception>
    public static CultureInfo? FromTwoLetterCode(string twoLetterCode)
    {
        ArgumentNullException.ThrowIfNull(twoLetterCode);

        if (string.IsNullOrWhiteSpace(twoLetterCode) || twoLetterCode.Length != 2)
        {
            return null;
        }

        try
        {
            return CultureInfo.GetCultureInfo(twoLetterCode);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolves a culture from an LCID (Locale Identifier).
    /// </summary>
    /// <param name="lcid">The LCID (e.g., 1033 for en-US, 1031 for de-DE).</param>
    /// <returns>The resolved <see cref="CultureInfo"/>, or <c>null</c> if the LCID is invalid.</returns>
    public static CultureInfo? FromLcid(int lcid)
    {
        if (lcid <= 0)
        {
            return null;
        }

        try
        {
            return CultureInfo.GetCultureInfo(lcid);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolves a culture from a full culture name.
    /// </summary>
    /// <param name="cultureName">The full culture name (e.g., "en-US", "de-DE").</param>
    /// <returns>The resolved <see cref="CultureInfo"/>, or <c>null</c> if the name is invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cultureName"/> is <c>null</c>.</exception>
    public static CultureInfo? FromCultureName(string cultureName)
    {
        ArgumentNullException.ThrowIfNull(cultureName);

        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return null;
        }

        try
        {
            return CultureInfo.GetCultureInfo(cultureName);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    /// <summary>
    /// Resolves a culture from a string that may be a two-letter code, LCID, or full culture name.
    /// </summary>
    /// <param name="cultureString">The culture string to parse.</param>
    /// <returns>The resolved <see cref="CultureInfo"/>, or <c>null</c> if parsing fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cultureString"/> is <c>null</c>.</exception>
    public static CultureInfo? Resolve(string cultureString)
    {
        ArgumentNullException.ThrowIfNull(cultureString);

        if (string.IsNullOrWhiteSpace(cultureString))
        {
            return null;
        }

        // Try as LCID if it's a numeric string
        if (int.TryParse(cultureString, out var lcid))
        {
            return FromLcid(lcid);
        }

        // Try as two-letter code if length is 2
        if (cultureString.Length == 2)
        {
            return FromTwoLetterCode(cultureString);
        }

        // Try as full culture name
        return FromCultureName(cultureString);
    }
}
