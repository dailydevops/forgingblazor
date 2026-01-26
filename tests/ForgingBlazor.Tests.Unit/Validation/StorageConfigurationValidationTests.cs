namespace NetEvolve.ForgingBlazor.Tests.Unit.Validation;

using System;
using System.IO;
using NetEvolve.ForgingBlazor.Storage.Validation;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="StorageConfigurationValidation"/>.
/// Tests storage provider configuration validation including path validation,
/// directory existence checks, and accessibility verification.
/// </summary>
public sealed class StorageConfigurationValidationTests
{
    [Test]
    public async Task Validate_WithValidConfiguration_ReturnsSuccess()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var options = new StorageConfiguration { ContentStorageBasePath = tempDir };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            using (Assert.Multiple())
            {
                _ = await Assert.That(result.Succeeded).IsTrue();
                _ = await Assert.That(result.Failed).IsFalse();
            }
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public void Validate_WithNullOptions_ThrowsArgumentNullException()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>("options", () => validator.Validate(null, null!));
    }

    [Test]
    public async Task Validate_WithNullContentStorageBasePath_ReturnsFail()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var options = new StorageConfiguration { ContentStorageBasePath = null };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("Content storage base path must be configured");
        }
    }

    [Test]
    public async Task Validate_WithEmptyContentStorageBasePath_ReturnsFail()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var options = new StorageConfiguration { ContentStorageBasePath = string.Empty };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("Content storage base path must be configured");
        }
    }

    [Test]
    public async Task Validate_WithWhitespaceContentStorageBasePath_ReturnsFail()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var options = new StorageConfiguration { ContentStorageBasePath = "   " };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("Content storage base path must be configured");
        }
    }

    [Test]
    public async Task Validate_WithNonExistentContentStorageBasePath_ReturnsFail()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var options = new StorageConfiguration { ContentStorageBasePath = nonExistentPath };

        // Act
        var result = validator.Validate(null, options);

        // Assert
        using (Assert.Multiple())
        {
            _ = await Assert.That(result.Succeeded).IsFalse();
            _ = await Assert.That(result.Failed).IsTrue();
            _ = await Assert.That(result.FailureMessage).Contains("does not exist or is not accessible");
            _ = await Assert.That(result.FailureMessage).Contains(nonExistentPath);
        }
    }

    [Test]
    public async Task Validate_WithValidContentAndAssetPaths_ReturnsSuccess()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var contentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var assetDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(contentDir);
        Directory.CreateDirectory(assetDir);

        try
        {
            var options = new StorageConfiguration
            {
                ContentStorageBasePath = contentDir,
                AssetStorageBasePath = assetDir,
            };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            using (Assert.Multiple())
            {
                _ = await Assert.That(result.Succeeded).IsTrue();
                _ = await Assert.That(result.Failed).IsFalse();
            }
        }
        finally
        {
            if (Directory.Exists(contentDir))
            {
                Directory.Delete(contentDir, true);
            }
            if (Directory.Exists(assetDir))
            {
                Directory.Delete(assetDir, true);
            }
        }
    }

    [Test]
    public async Task Validate_WithNonExistentAssetStorageBasePath_ReturnsFail()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var contentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var nonExistentAssetPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(contentDir);

        try
        {
            var options = new StorageConfiguration
            {
                ContentStorageBasePath = contentDir,
                AssetStorageBasePath = nonExistentAssetPath,
            };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            using (Assert.Multiple())
            {
                _ = await Assert.That(result.Succeeded).IsFalse();
                _ = await Assert.That(result.Failed).IsTrue();
                _ = await Assert.That(result.FailureMessage).Contains("Asset storage base path");
                _ = await Assert.That(result.FailureMessage).Contains("does not exist or is not accessible");
                _ = await Assert.That(result.FailureMessage).Contains(nonExistentAssetPath);
            }
        }
        finally
        {
            if (Directory.Exists(contentDir))
            {
                Directory.Delete(contentDir, true);
            }
        }
    }

    [Test]
    public async Task Validate_WithEmptyAssetStorageBasePath_ReturnsSuccess()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var contentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(contentDir);

        try
        {
            var options = new StorageConfiguration
            {
                ContentStorageBasePath = contentDir,
                AssetStorageBasePath = string.Empty,
            };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            using (Assert.Multiple())
            {
                _ = await Assert.That(result.Succeeded).IsTrue();
                _ = await Assert.That(result.Failed).IsFalse();
            }
        }
        finally
        {
            if (Directory.Exists(contentDir))
            {
                Directory.Delete(contentDir, true);
            }
        }
    }

    [Test]
    public async Task Validate_WithNullAssetStorageBasePath_ReturnsSuccess()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var contentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(contentDir);

        try
        {
            var options = new StorageConfiguration { ContentStorageBasePath = contentDir, AssetStorageBasePath = null };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            using (Assert.Multiple())
            {
                _ = await Assert.That(result.Succeeded).IsTrue();
                _ = await Assert.That(result.Failed).IsFalse();
            }
        }
        finally
        {
            if (Directory.Exists(contentDir))
            {
                Directory.Delete(contentDir, true);
            }
        }
    }

    [Test]
    public async Task Validate_WithWhitespaceAssetStorageBasePath_ReturnsSuccess()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var contentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(contentDir);

        try
        {
            var options = new StorageConfiguration
            {
                ContentStorageBasePath = contentDir,
                AssetStorageBasePath = "   ",
            };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            using (Assert.Multiple())
            {
                _ = await Assert.That(result.Succeeded).IsTrue();
                _ = await Assert.That(result.Failed).IsFalse();
            }
        }
        finally
        {
            if (Directory.Exists(contentDir))
            {
                Directory.Delete(contentDir, true);
            }
        }
    }

    [Test]
    public async Task Validate_WithSameContentAndAssetPaths_ReturnsSuccess()
    {
        // Arrange
        var validator = new StorageConfigurationValidation();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var options = new StorageConfiguration { ContentStorageBasePath = tempDir, AssetStorageBasePath = tempDir };

            // Act
            var result = validator.Validate(null, options);

            // Assert
            using (Assert.Multiple())
            {
                _ = await Assert.That(result.Succeeded).IsTrue();
                _ = await Assert.That(result.Failed).IsFalse();
            }
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
