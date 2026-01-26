namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing;

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Routing.Constraints;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="SlugRouteConstraint"/>.
/// </summary>
public sealed class SlugRouteConstraintTests
{
    [Test]
    public async Task Match_WithValidSlug_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "abc" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithValidSlugContainingHyphen_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test-slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithValidLongSlug_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "my-long-slug-name" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithMinimumLengthSlug_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "abc" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithMaximumLengthSlug_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var maxLengthSlug =
            new string('a', Defaults.SegmentLengthMinimum)
            + new string('b', Defaults.SegmentLengthMaximum - Defaults.SegmentLengthMinimum - 1)
            + 'c';
        var values = new RouteValueDictionary { { "slug", maxLengthSlug } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithNumbersInMiddle_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test123slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithMixedCaseLetters_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "MySlug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithMultipleHyphensNotConsecutive_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "my-test-slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithSlugStartingWithHyphen_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "-abc" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithSlugEndingWithHyphen_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "abc-" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithSlugStartingWithDigit_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "1abc" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithSlugEndingWithDigit_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "abc1" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithConsecutiveHyphens_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test--slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithUnderscore_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test_slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithDot_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test.slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithSpace_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithSpecialCharacter_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test@slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithTooShortSlug_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "ab" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithTooLongSlug_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var tooLongSlug = new string('a', Defaults.SegmentLengthMaximum + 1);
        var values = new RouteValueDictionary { { "slug", tooLongSlug } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithNullSlug_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", null } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithEmptySlug_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", string.Empty } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithWhitespaceSlug_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "   " } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithMissingRouteKey_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "other", "test-slug" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public void Match_WithNullValues_ThrowsArgumentNullException()
    {
        var constraint = new SlugRouteConstraint();

        _ = Assert.Throws<ArgumentNullException>(
            "values",
            () => constraint.Match(null, null, "slug", null!, RouteDirection.IncomingRequest)
        );
    }

    [Test]
    public void Match_WithNullRouteKey_ThrowsArgumentException()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test-slug" } };

        _ = Assert.Throws<ArgumentException>(() =>
            constraint.Match(null, null, null!, values, RouteDirection.IncomingRequest)
        );
    }

    [Test]
    public void Match_WithEmptyRouteKey_ThrowsArgumentException()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test-slug" } };

        _ = Assert.Throws<ArgumentException>(() =>
            constraint.Match(null, null, string.Empty, values, RouteDirection.IncomingRequest)
        );
    }

    [Test]
    public void Match_WithWhitespaceRouteKey_ThrowsArgumentException()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "test-slug" } };

        _ = Assert.Throws<ArgumentException>(() =>
            constraint.Match(null, null, "   ", values, RouteDirection.IncomingRequest)
        );
    }

    [Test]
    public async Task Match_WithReadOnlyMemoryValue_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var memory = "test-slug".AsMemory();
        var values = new RouteValueDictionary { { "slug", memory } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithReadOnlyMemoryInvalidValue_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var memory = "test_slug".AsMemory();
        var values = new RouteValueDictionary { { "slug", memory } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithNumericValue_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", 123 } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithSingleLetter_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "a" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithTwoLetters_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "ab" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithOnlyHyphens_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "---" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithOnlyDigits_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "123" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithUnicodeCharacters_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { { "slug", "tëst" } };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }
}
