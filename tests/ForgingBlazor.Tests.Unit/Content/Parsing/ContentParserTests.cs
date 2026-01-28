namespace NetEvolve.ForgingBlazor.Tests.Unit.Content.Parsing;

using System;
using System.Diagnostics.CodeAnalysis;
using NetEvolve.ForgingBlazor;
using NetEvolve.ForgingBlazor.Content;
using NetEvolve.ForgingBlazor.Content.Parsing;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class ContentParserTests
{
    [Test]
    public async Task Parse_WithValidMarkdownAndFrontmatter_CreatesContentDescriptor()
    {
        var content = """
            ---
            title: Test Article
            slug: test-article
            publishedDate: 2026-01-25T10:00:00+00:00
            ---
            # Article Content

            This is the article body.
            """;

        var descriptor = ContentParser.Parse<TestContentDescriptorForParsing>(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(descriptor).IsNotNull();
            _ = await Assert.That(descriptor.Title).IsEqualTo("Test Article");
            _ = await Assert.That(descriptor.Slug).IsEqualTo("test-article");
            _ = await Assert.That(descriptor.Body).Contains("# Article Content");
            _ = await Assert.That(descriptor.BodyHtml).Contains("<h1");
        }
    }

    [Test]
    public async Task Parse_WithCustomDescriptor_PopulatesCustomFields()
    {
        var content = """
---
title: Blog Post
slug: blog-post
publishedDate: 2026-01-25T10:00:00+00:00
author: John Doe
tags:
  - csharp
  - blazor
---
Content here
""";

        var descriptor = ContentParser.Parse<TestBlogPostDescriptor>(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(descriptor).IsNotNull();
            _ = await Assert.That(descriptor.Author).IsEqualTo("John Doe");
            _ = await Assert.That(descriptor.Tags!.Length).IsEqualTo(2);
        }
    }

    [Test]
    public async Task Parse_WithDraftFalse_SetsDraftToFalse()
    {
        var content = """
---
title: Test
slug: test
publishedDate: 2026-01-25T10:00:00+00:00
draft: false
---
Body
""";

        var descriptor = ContentParser.Parse<TestContentDescriptorForParsing>(content);

        _ = await Assert.That(descriptor.Draft).IsFalse();
    }

    [Test]
    public async Task Parse_WithDraftTrue_SetsDraftToTrue()
    {
        var content = """
---
title: Test
slug: test
publishedDate: 2026-01-25T10:00:00+00:00
draft: true
---
Body
""";

        var descriptor = ContentParser.Parse<TestContentDescriptorForParsing>(content);

        _ = await Assert.That(descriptor.Draft).IsTrue();
    }

    [Test]
    public async Task Parse_WithExpiredAt_SetsExpiredAtProperty()
    {
        var content = """
---
title: Test
slug: test
publishedDate: 2026-01-25T10:00:00+00:00
expiredAt: 2027-01-01T00:00:00+00:00
---
Body
""";

        var descriptor = ContentParser.Parse<TestContentDescriptorForParsing>(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(descriptor.ExpiredAt).IsNotNull();
            _ = await Assert.That(descriptor.ExpiredAt!.Value.Year).IsEqualTo(2027);
        }
    }

    [Test]
    public void Parse_WithMissingTitle_ThrowsContentValidationException()
    {
        var content = """
---
slug: test
publishedDate: 2026-01-25T10:00:00+00:00
---
Body
""";

        _ = Assert.Throws<ContentValidationException>(() =>
            ContentParser.Parse<TestContentDescriptorForParsing>(content)
        );
    }

    [Test]
    public void Parse_WithMissingSlug_ThrowsContentValidationException()
    {
        var content = """
---
title: Test
publishedDate: 2026-01-25T10:00:00+00:00
---
Body
""";

        _ = Assert.Throws<ContentValidationException>(() =>
            ContentParser.Parse<TestContentDescriptorForParsing>(content)
        );
    }

    [Test]
    public void Parse_WithMissingPublishedDate_ThrowsContentValidationException()
    {
        var content = """
---
title: Test
slug: test
---
Body
""";

        _ = Assert.Throws<ContentValidationException>(() =>
            ContentParser.Parse<TestContentDescriptorForParsing>(content)
        );
    }

    [Test]
    public void Parse_WhenContentIsNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "markdownContent",
            () => ContentParser.Parse<TestContentDescriptorForParsing>(null!)
        );

    [Test]
    public async Task Parse_WithComplexMarkdown_RendersHtmlCorrectly()
    {
        var content = """
---
title: Test
slug: test
publishedDate: 2026-01-25T10:00:00+00:00
---
# Heading

Paragraph with **bold** and *italic*.

```csharp
var x = 42;
```

| Header |
|--------|
| Cell   |
""";

        var descriptor = ContentParser.Parse<TestContentDescriptorForParsing>(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(descriptor.BodyHtml).Contains("<h1");
            _ = await Assert.That(descriptor.BodyHtml).Contains("<strong");
            _ = await Assert.That(descriptor.BodyHtml).Contains("<em");
            _ = await Assert.That(descriptor.BodyHtml).Contains("<pre");
            _ = await Assert.That(descriptor.BodyHtml).Contains("<table");
        }
    }
}

file sealed class TestBlogPostDescriptor : ContentDescriptor
{
    [SetsRequiredMembers]
    public TestBlogPostDescriptor()
    {
        Title = string.Empty;
        Slug = string.Empty;
    }

    public string? Author { get; set; }
    public string[]? Tags { get; set; }
}

file sealed class TestContentDescriptorForParsing : ContentDescriptor
{
    [SetsRequiredMembers]
    public TestContentDescriptorForParsing()
    {
        Title = string.Empty;
        Slug = string.Empty;
    }
}
