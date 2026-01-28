namespace NetEvolve.ForgingBlazor.Storage.Validation;

using System;
using System.IO;
using Microsoft.Extensions.Options;

/// <summary>
/// Validates storage configuration at startup implementing <see cref="IValidateOptions{TOptions}"/>.
/// Validates: at least one storage provider configured, base paths accessible.
/// </summary>
internal sealed class StorageConfigurationValidation : IValidateOptions<StorageConfiguration>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, StorageConfiguration options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Validate at least one storage provider is configured
        if (string.IsNullOrWhiteSpace(options.ContentStorageBasePath))
        {
            return ValidateOptionsResult.Fail("Content storage base path must be configured.");
        }

        // Validate base path is accessible
        if (!Directory.Exists(options.ContentStorageBasePath))
        {
            return ValidateOptionsResult.Fail(
                $"Content storage base path '{options.ContentStorageBasePath}' does not exist or is not accessible."
            );
        }

        // Validate asset storage base path if configured
        if (!string.IsNullOrWhiteSpace(options.AssetStorageBasePath) && !Directory.Exists(options.AssetStorageBasePath))
        {
            return ValidateOptionsResult.Fail(
                $"Asset storage base path '{options.AssetStorageBasePath}' does not exist or is not accessible."
            );
        }

        return ValidateOptionsResult.Success;
    }
}
