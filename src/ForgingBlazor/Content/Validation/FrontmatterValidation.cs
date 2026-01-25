namespace NetEvolve.ForgingBlazor.Content.Validation;

using System;
using System.Collections.Generic;
using global::NetEvolve.ForgingBlazor;

/// <summary>
/// Validates YAML frontmatter data for content files.
/// </summary>
internal static class FrontmatterValidation
{
    private static readonly HashSet<string> _requiredFields = ["title", "slug", "publisheddate"];

    /// <summary>
    /// Validates that all required frontmatter fields are present and valid.
    /// </summary>
    /// <param name="frontmatter">The frontmatter dictionary to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="frontmatter"/> is <see langword="null"/>.</exception>
    /// <exception cref="ContentValidationException">Thrown when required fields are missing or invalid.</exception>
    internal static void ValidateRequiredFields(IReadOnlyDictionary<string, object> frontmatter)
    {
        ArgumentNullException.ThrowIfNull(frontmatter);

        foreach (var requiredField in _requiredFields)
        {
            if (
                !frontmatter.TryGetValue(requiredField, out var value)
                || value is null
                || (value is string strValue && string.IsNullOrWhiteSpace(strValue))
            )
            {
                throw new ContentValidationException(
                    $"Required frontmatter field '{requiredField}' is missing or empty."
                );
            }
        }

        ValidateSlugField(frontmatter);
        ValidateDateField(frontmatter, "publisheddate");

        if (frontmatter.TryGetValue("expiredat", out var expiredAtValue) && expiredAtValue is not null)
        {
            ValidateDateField(frontmatter, "expiredat");
        }
    }

    private static void ValidateSlugField(IReadOnlyDictionary<string, object> frontmatter)
    {
        if (!frontmatter.TryGetValue("slug", out var slugValue) || slugValue is not string slug)
        {
            throw new ContentValidationException("slug", typeof(string), slugValue);
        }

        try
        {
            _ = Check.ValidateSlug(slug, "slug");
        }
        catch (ArgumentException ex)
        {
            throw new ContentValidationException($"The 'slug' field contains an invalid value. {ex.Message}", ex);
        }
    }

    private static void ValidateDateField(IReadOnlyDictionary<string, object> frontmatter, string fieldName)
    {
        if (!frontmatter.TryGetValue(fieldName, out var dateValue))
        {
            throw new ContentValidationException($"Required frontmatter field '{fieldName}' is missing.");
        }

        if (dateValue is DateTimeOffset)
        {
            return;
        }

        if (dateValue is DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                throw new ContentValidationException(
                    $"The '{fieldName}' field must have a specified timezone. Use DateTimeOffset or specify DateTimeKind."
                );
            }

            return;
        }

        if (dateValue is string dateString)
        {
            if (!DateTimeOffset.TryParse(dateString, System.Globalization.CultureInfo.InvariantCulture, out _))
            {
                throw new ContentValidationException(fieldName, typeof(DateTimeOffset), dateValue);
            }

            return;
        }

        throw new ContentValidationException(fieldName, typeof(DateTimeOffset), dateValue);
    }
}
