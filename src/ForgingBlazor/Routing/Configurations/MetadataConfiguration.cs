namespace NetEvolve.ForgingBlazor.Routing.Configurations;

using System;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Routing;

/// <summary>
/// Implements the metadata configuration fluent API.
/// </summary>
internal sealed class MetadataConfiguration : IMetadataConfiguration
{
    private readonly MetadataConfigurationBuilderState _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataConfiguration"/> class.
    /// </summary>
    /// <param name="metadata">The metadata configuration state.</param>
    internal MetadataConfiguration(MetadataConfigurationBuilderState metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _metadata = metadata;
    }

    /// <inheritdoc />
    public IMetadataConfiguration ExtendWith<T>(string fieldName, T defaultValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);

        if (defaultValue is not null && defaultValue is not T)
        {
            throw new ArgumentException(
                "The default value is not assignable to the metadata field type.",
                nameof(defaultValue)
            );
        }

        _metadata.AddField(fieldName, typeof(T), defaultValue);
        return this;
    }
}
