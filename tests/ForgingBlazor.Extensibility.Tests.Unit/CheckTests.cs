namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit;

public class CheckTests
{
    [Test]
    [MethodDataSource(nameof(IsValidPathSegment_Theory_Expected_Data))]
    public async Task IsValidPathSegment_Theory_Expected(string? pathSegment, bool expected)
    {
        // Act
        var result = Check.IsValidPathSegment(pathSegment);

        // Assert
        _ = await Assert.That(result).EqualTo(expected);
    }

    [Test]
    [MethodDataSource(nameof(IsValidAdminSegment_Theory_Expected_Data))]
    public async Task IsValidAdminSegment_Theory_Expected(string? pathSegment, bool expected)
    {
        // Act
        var result = Check.IsValidAdminSegment(pathSegment);

        // Assert
        _ = await Assert.That(result).EqualTo(expected);
    }

    [Test]
    [MethodDataSource(nameof(IsValidSlug_Theory_Expected_Data))]
    public async Task IsValidSlug_Theory_Expected(string? slug, bool expected)
    {
        // Act
        var result = Check.IsValidSlug(slug);

        // Assert
        _ = await Assert.That(result).EqualTo(expected);
    }

    [Test]
    public async Task ValidateSlug_Invalid_ThrowsArgumentException() =>
        _ = await Assert.That(() => Check.ValidateSlug("invalid_slug", "slug")).Throws<ArgumentException>();

    public static IEnumerable<(string?, bool)> IsValidPathSegment_Theory_Expected_Data =>
        [
            (null, false),
            ("", false),
            ("\t", false),
            ("ab", false),
            ("a_b", true),
            ("a-b", true),
            ("abc", true),
            ("a_bc-def", true),
            ("a".PadRight(70, 'a'), true),
            ("a".PadRight(71, 'a'), false),
            ("_abc", false),
            ("abc_", false),
            ("-abc", false),
            ("abc-", false),
            ("ab$c", false),
            ("ab c", false),
        ];

    public static IEnumerable<(string?, bool)> IsValidAdminSegment_Theory_Expected_Data =>
        [
            (null, false),
            ("", false),
            ("\t", false),
            ("ab", false),
            ("a_b", true),
            ("a-b", true),
            ("abc", true),
            ("a_bc-def", true),
            ("a".PadRight(70, 'a'), true),
            ("a".PadRight(71, 'a'), false),
            ("_abc", true),
            ("abc_", true),
            ("-abc", true),
            ("abc-", true),
            ("ab$c", false),
            ("ab c", false),
        ];

    public static IEnumerable<(string?, bool)> IsValidSlug_Theory_Expected_Data =>
        [
            (null, false),
            ("", false),
            ("ab", false),
            ("abc", true),
            ("my-article", true),
            ("My-Article", true),
            ("a123-post", true),
            ("my_article", false),
            ("one--double", false),
            ("trailing-", false),
            ("-leading", false),
            ("trailing-123", false),
            ("123-leading", false),
            ("a".PadRight(70, 'a'), true),
            ("a".PadRight(71, 'a'), false),
        ];
}
