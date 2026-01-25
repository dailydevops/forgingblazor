namespace NetEvolve.ForgingBlazor;

/// <summary>
/// Configures metadata extensions for content descriptors and pages.
/// </summary>
public interface IMetadataConfiguration
{
    /// <summary>
    /// Adds an extensible metadata field.
    /// </summary>
    /// <typeparam name="T">The metadata field type.</typeparam>
    /// <param name="fieldName">The field name (case-insensitive recommendation).</param>
    /// <returns>The <see cref="IMetadataConfiguration"/> for chaining.</returns>
    IMetadataConfiguration ExtendWith<T>(string fieldName) => ExtendWith<T>(fieldName, default!);

    /// <summary>
    /// Adds an extensible metadata field with a default value.
    /// </summary>
    /// <typeparam name="T">The metadata field type.</typeparam>
    /// <param name="fieldName">The field name (case-insensitive recommendation).</param>
    /// <param name="defaultValue">The default value to apply when not provided.</param>
    /// <returns>The <see cref="IMetadataConfiguration"/> for chaining.</returns>
    IMetadataConfiguration ExtendWith<T>(string fieldName, T defaultValue);
}
