namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

/// <summary>
/// Tests for <see cref="Check.IsValidSlug"/> and <see cref="Check.ValidateSlug"/> methods.
/// </summary>
public sealed class CheckSlugTests
{
    [Test]
    [Arguments("abc")]
    [Arguments("test")]
    [Arguments("hello-world")]
    [Arguments("my-slug")]
    [Arguments("slug-with-multiple-parts")]
    public async Task IsValidSlug_ValidSlug_ReturnsTrue(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidSlug_MinimumLength_ReturnsTrue()
    {
        var slug = "abc"; // 3 characters

        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task IsValidSlug_MaximumLength_ReturnsTrue()
    {
        // 70 characters: exactly at boundary
        var slug = "maximum-length-slug-that-is-exactly-seventy-characters-long-boundary";

        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("  ")]
    public async Task IsValidSlug_NullOrWhiteSpace_ReturnsFalse(string? slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("ab")] // Too short (2 chars)
    [Arguments("a")] // Too short (1 char)
    public async Task IsValidSlug_BelowMinimumLength_ReturnsFalse(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task IsValidSlug_AboveMaximumLength_ReturnsFalse()
    {
        // 71 characters: one character over the limit (must end with letter)
        var slug = "a" + new string('x', 69) + "b"; // axxx...xxxb (71 chars)

        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("-abc")] // Starts with hyphen
    [Arguments("abc-")] // Ends with hyphen
    [Arguments("123")] // Starts with digit
    [Arguments("test-123")] // Ends with digit
    [Arguments("a1-")] // Ends with hyphen
    [Arguments("-1a")] // Starts with hyphen
    [Arguments("123abc")] // Starts with digit
    [Arguments("abc123")] // Ends with digit
    public async Task IsValidSlug_InvalidStartOrEnd_ReturnsFalse(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("test--slug")] // Consecutive hyphens
    [Arguments("a--b")] // Consecutive hyphens at minimum length
    [Arguments("hello---world")] // Triple hyphens
    public async Task IsValidSlug_ConsecutiveHyphens_ReturnsFalse(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("test slug")] // Space
    [Arguments("test_slug")] // Underscore
    [Arguments("test.slug")] // Period
    [Arguments("test/slug")] // Slash
    [Arguments("test\\slug")] // Backslash
    [Arguments("tëst")] // Umlaut
    [Arguments("tést")] // Accent
    [Arguments("te🚀st")] // Emoji
    [Arguments("test!")] // Exclamation
    [Arguments("test@slug")] // At symbol
    public async Task IsValidSlug_InvalidCharacters_ReturnsFalse(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    [Arguments("abc")]
    [Arguments("test")]
    [Arguments("hello-world")]
    public async Task ValidateSlug_ValidSlug_ReturnsSlug(string slug)
    {
        var result = Check.ValidateSlug(slug);

        _ = await Assert.That(result).IsEqualTo(slug);
    }

    [Test]
    public async Task ValidateSlug_MinimumLength_ReturnsSlug()
    {
        var slug = "abc";

        var result = Check.ValidateSlug(slug);

        _ = await Assert.That(result).IsEqualTo(slug);
    }

    [Test]
    public async Task ValidateSlug_MaximumLength_ReturnsSlug()
    {
        var slug = "maximum-length-slug-that-is-exactly-seventy-characters-long-boundary";

        var result = Check.ValidateSlug(slug);

        _ = await Assert.That(result).IsEqualTo(slug);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("ab")] // Too short
    [Arguments("-abc")] // Invalid start
    [Arguments("test--slug")] // Consecutive hyphens
    [Arguments("test slug")] // Space
    public async Task ValidateSlug_InvalidSlug_ThrowsArgumentException(string? slug)
    {
        await Assert
            .That(() => Check.ValidateSlug(slug))
            .Throws<ArgumentException>()
            .And.HasMessageContaining("not valid");
    }

    [Test]
    public async Task ValidateSlug_InvalidSlugWithParameterName_ThrowsWithParameterName()
    {
        var slug = "ab";
        var parameterName = "testParameter";

        var exception = await Assert.That(() => Check.ValidateSlug(slug, parameterName)).Throws<ArgumentException>();

        _ = await Assert.That(exception.ParamName!).IsEqualTo(parameterName);
    }

    [Test]
    public async Task ValidateSlug_BelowMinimumLength_ThrowsWithMessage()
    {
        var slug = "ab";

        var exception = await Assert.That(() => Check.ValidateSlug(slug)).Throws<ArgumentException>();

        _ = await Assert.That(exception.Message!).Contains("between 3 and 70 characters");
    }

    [Test]
    public async Task ValidateSlug_AboveMaximumLength_ThrowsWithMessage()
    {
        var slug = new string('a', 71);

        var exception = await Assert.That(() => Check.ValidateSlug(slug)).Throws<ArgumentException>();

        _ = await Assert.That(exception.Message!).Contains("between 3 and 70 characters");
    }

    [Test]
    public async Task ValidateSlug_ConsecutiveHyphens_ThrowsWithMessage()
    {
        var slug = "test--slug";

        var exception = await Assert.That(() => Check.ValidateSlug(slug)).Throws<ArgumentException>();

        _ = await Assert.That(exception.Message!).Contains("single hyphens");
    }

    [Test]
    [Arguments("MiXeD-CaSe")] // Mixed case is valid (constraint is ASCII letters)
    [Arguments("UPPERCASE")]
    [Arguments("lowercase")]
    [Arguments("CamelCase")]
    public async Task IsValidSlug_MixedCase_ReturnsTrue(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("num123end")]
    [Arguments("test-123-end")]
    [Arguments("a1b2c3d")]
    public async Task IsValidSlug_WithNumbers_ReturnsTrue(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    [Arguments("single-hyphen-test")]
    [Arguments("a-b-c-d-e-f")]
    [Arguments("many-single-hyphens-are-valid")]
    public async Task IsValidSlug_MultipleSingleHyphens_ReturnsTrue(string slug)
    {
        var result = Check.IsValidSlug(slug);

        _ = await Assert.That(result).IsTrue();
    }
}
