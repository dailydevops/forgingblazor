namespace NetEvolve.ForgingBlazor.Content;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using global::NetEvolve.ForgingBlazor;

/// <summary>
/// Factory for creating and populating <see cref="ContentDescriptor"/> instances from frontmatter data.
/// </summary>
internal static class ContentDescriptorFactory
{
    /// <summary>
    /// Creates a content descriptor instance of the specified type and populates it with frontmatter data.
    /// </summary>
    /// <typeparam name="TDescriptor">The content descriptor type to instantiate.</typeparam>
    /// <param name="frontmatter">The frontmatter data to map to the descriptor.</param>
    /// <param name="markdownBody">The raw Markdown body content.</param>
    /// <param name="htmlBody">The rendered HTML body content.</param>
    /// <returns>A populated content descriptor instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the descriptor cannot be instantiated.</exception>
    /// <exception cref="ContentValidationException">Thrown when required properties cannot be set.</exception>
    internal static TDescriptor Create<TDescriptor>(
        IReadOnlyDictionary<string, object> frontmatter,
        string markdownBody,
        string htmlBody
    )
        where TDescriptor : ContentDescriptor, new()
    {
        ArgumentNullException.ThrowIfNull(frontmatter);
        ArgumentNullException.ThrowIfNull(markdownBody);
        ArgumentNullException.ThrowIfNull(htmlBody);

        var descriptor = new TDescriptor { Body = markdownBody, BodyHtml = htmlBody };

        MapFrontmatterToProperties(descriptor, frontmatter);

        return descriptor;
    }

    private static void MapFrontmatterToProperties(
        ContentDescriptor descriptor,
        IReadOnlyDictionary<string, object> frontmatter
    )
    {
        var descriptorType = descriptor.GetType();
        var properties = descriptorType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(static p => p.CanWrite)
            .ToArray();

        foreach (var kvp in frontmatter)
        {
            var propertyName = kvp.Key;
            var value = kvp.Value;

            var property = properties.FirstOrDefault(p =>
                string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase)
            );

            if (property is null)
            {
                continue;
            }

            try
            {
                var convertedValue = ConvertValue(value, property.PropertyType);
                property.SetValue(descriptor, convertedValue);
            }
            catch (Exception ex)
            {
                throw new ContentValidationException(
                    $"Failed to set property '{property.Name}' on content descriptor. Value: {value}",
                    ex
                );
            }
        }
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value is null)
        {
            return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) is null
                ? Activator.CreateInstance(targetType)
                : null;
        }

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(DateTimeOffset))
        {
            return value switch
            {
                DateTimeOffset dto => dto,
                DateTime dt => new DateTimeOffset(dt),
                string str => DateTimeOffset.Parse(str, System.Globalization.CultureInfo.InvariantCulture),
                _ => throw new InvalidCastException(
                    $"Cannot convert value of type '{value.GetType().Name}' to DateTimeOffset."
                ),
            };
        }

        if (underlyingType == typeof(bool))
        {
            return value switch
            {
                bool b => b,
                string str => bool.Parse(str),
                _ => Convert.ToBoolean(value, System.Globalization.CultureInfo.InvariantCulture),
            };
        }

        if (underlyingType.IsEnum)
        {
            return value is string str
                ? Enum.Parse(underlyingType, str, ignoreCase: true)
                : Enum.ToObject(underlyingType, value);
        }

        if (value.GetType() == underlyingType)
        {
            return value;
        }

        return Convert.ChangeType(value, underlyingType, System.Globalization.CultureInfo.InvariantCulture);
    }
}
