namespace NetEvolve.ForgingBlazor.Tests.Unit.Routing.Constraints;

using System;
using global::NetEvolve.ForgingBlazor.Routing.Constraints;
using Microsoft.AspNetCore.Routing;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class SlugRouteConstraintTests
{
    [Test]
    public async Task Match_WithValidSlugString_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "valid-slug" };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithInvalidSlugTooShort_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "ab" };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithInvalidSlugEndsWithHyphen_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "invalid-" };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithInvalidSlugStartsWithHyphen_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "-invalid" };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithReadOnlyMemoryChar_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "valid-slug".AsMemory() };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithNumericValue_ConvertsAndValidates()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = 123 };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithNullValue_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = null };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WithMissingKey_ReturnsFalse()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary();

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task Match_WhenRouteKeyEmpty_ThrowsArgumentException()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "valid-slug" };

        _ = await Assert
            .That(() => constraint.Match(null, null, "", values, RouteDirection.IncomingRequest))
            .ThrowsExactly<ArgumentException>()
            .WithParameterName("routeKey");
    }

    [Test]
    public async Task Match_WhenValuesNull_ThrowsArgumentNullException()
    {
        var constraint = new SlugRouteConstraint();

        _ = await Assert
            .That(() => constraint.Match(null, null, "slug", null!, RouteDirection.IncomingRequest))
            .ThrowsExactly<ArgumentNullException>()
            .WithParameterName("values");
    }

    [Test]
    public async Task Match_WithValidSlugContainingNumbers_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "post-123-test" };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task Match_WithSlugExactlyThreeChars_ReturnsTrue()
    {
        var constraint = new SlugRouteConstraint();
        var values = new RouteValueDictionary { ["slug"] = "abc" };

        var result = constraint.Match(null, null, "slug", values, RouteDirection.IncomingRequest);

        _ = await Assert.That(result).IsTrue();
    }
}
