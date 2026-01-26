namespace NetEvolve.ForgingBlazor.Tests.Unit.Content.Validation;

using System;
using System.Collections.Generic;
using global::NetEvolve.ForgingBlazor;
using global::NetEvolve.ForgingBlazor.Content.Validation;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class FrontmatterValidationTests
{
    [Test]
    public async Task ValidateRequiredFields_WhenAllFieldsPresent_DoesNotThrow()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "test-slug",
            ["publisheddate"] = DateTimeOffset.UtcNow,
        };

        _ = await Assert.That(() => FrontmatterValidation.ValidateRequiredFields(frontmatter)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateRequiredFields_WhenTitleMissing_ThrowsContentValidationException()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["slug"] = "test-slug",
            ["publisheddate"] = DateTimeOffset.UtcNow,
        };

        var exception = Assert.Throws<ContentValidationException>(() =>
            FrontmatterValidation.ValidateRequiredFields(frontmatter)
        );

        _ = await Assert.That(exception.Message).IsEqualTo("Required frontmatter field 'title' is missing or empty.");
    }

    [Test]
    public async Task ValidateRequiredFields_WhenSlugMissing_ThrowsContentValidationException()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["publisheddate"] = DateTimeOffset.UtcNow,
        };

        var exception = Assert.Throws<ContentValidationException>(() =>
            FrontmatterValidation.ValidateRequiredFields(frontmatter)
        );

        _ = await Assert.That(exception.Message).IsEqualTo("Required frontmatter field 'slug' is missing or empty.");
    }

    [Test]
    public async Task ValidateRequiredFields_WhenPublishedDateMissing_ThrowsContentValidationException()
    {
        var frontmatter = new Dictionary<string, object> { ["title"] = "Test Title", ["slug"] = "test-slug" };

        var exception = Assert.Throws<ContentValidationException>(() =>
            FrontmatterValidation.ValidateRequiredFields(frontmatter)
        );

        _ = await Assert
            .That(exception.Message)
            .IsEqualTo("Required frontmatter field 'publisheddate' is missing or empty.");
    }

    [Test]
    public async Task ValidateRequiredFields_WhenTitleEmpty_ThrowsContentValidationException()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "",
            ["slug"] = "test-slug",
            ["publisheddate"] = DateTimeOffset.UtcNow,
        };

        var exception = Assert.Throws<ContentValidationException>(() =>
            FrontmatterValidation.ValidateRequiredFields(frontmatter)
        );

        _ = await Assert.That(exception.Message).IsEqualTo("Required frontmatter field 'title' is missing or empty.");
    }

    [Test]
    public async Task ValidateRequiredFields_WhenSlugInvalid_ThrowsContentValidationException()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "ab",
            ["publisheddate"] = DateTimeOffset.UtcNow,
        };

        _ = Assert.Throws<ContentValidationException>(() => FrontmatterValidation.ValidateRequiredFields(frontmatter));
    }

    [Test]
    public async Task ValidateRequiredFields_WhenPublishedDateInvalidString_ThrowsContentValidationException()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "test-slug",
            ["publisheddate"] = "not-a-date",
        };

        _ = Assert.Throws<ContentValidationException>(() => FrontmatterValidation.ValidateRequiredFields(frontmatter));
    }

    [Test]
    public async Task ValidateRequiredFields_WhenPublishedDateValidString_DoesNotThrow()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "test-slug",
            ["publisheddate"] = "2026-01-25T10:00:00Z",
        };

        _ = await Assert.That(() => FrontmatterValidation.ValidateRequiredFields(frontmatter)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateRequiredFields_WhenPublishedDateUnspecifiedDateTime_ThrowsContentValidationException()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "test-slug",
            ["publisheddate"] = new DateTime(2026, 1, 25, 10, 0, 0, DateTimeKind.Unspecified),
        };

        var exception = Assert.Throws<ContentValidationException>(() =>
            FrontmatterValidation.ValidateRequiredFields(frontmatter)
        );

        _ = await Assert
            .That(exception.Message)
            .IsEqualTo(
                "The 'publisheddate' field must have a specified timezone. Use DateTimeOffset or specify DateTimeKind."
            );
    }

    [Test]
    public async Task ValidateRequiredFields_WhenPublishedDateUtcDateTime_DoesNotThrow()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "test-slug",
            ["publisheddate"] = DateTime.UtcNow,
        };

        _ = await Assert.That(() => FrontmatterValidation.ValidateRequiredFields(frontmatter)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateRequiredFields_WhenExpiredAtPresent_ValidatesExpiredAt()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "test-slug",
            ["publisheddate"] = DateTimeOffset.UtcNow,
            ["expiredat"] = "not-a-date",
        };

        _ = Assert.Throws<ContentValidationException>(() => FrontmatterValidation.ValidateRequiredFields(frontmatter));
    }

    [Test]
    public async Task ValidateRequiredFields_WhenExpiredAtValidString_DoesNotThrow()
    {
        var frontmatter = new Dictionary<string, object>
        {
            ["title"] = "Test Title",
            ["slug"] = "test-slug",
            ["publisheddate"] = DateTimeOffset.UtcNow,
            ["expiredat"] = "2026-12-31T23:59:59Z",
        };

        _ = await Assert.That(() => FrontmatterValidation.ValidateRequiredFields(frontmatter)).ThrowsNothing();
    }

    [Test]
    public async Task ValidateRequiredFields_WhenFrontmatterNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "frontmatter",
            () => FrontmatterValidation.ValidateRequiredFields(null!)
        );
}
