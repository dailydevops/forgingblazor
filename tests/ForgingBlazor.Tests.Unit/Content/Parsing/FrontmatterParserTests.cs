namespace NetEvolve.ForgingBlazor.Tests.Unit.Content.Parsing;

using System;
using System.Collections.Generic;
using NetEvolve.ForgingBlazor.Content.Parsing;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class FrontmatterParserTests
{
    [Test]
    public async Task Parse_WithValidYaml_ExtractsFrontmatterAndBody()
    {
        var content = """
---
title: Test Title
slug: test-slug
publishedDate: 2026-01-25T10:00:00+00:00
---
# Test Content

This is the body.
""";

        var (frontmatter, body) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter).IsNotNull();
            _ = await Assert.That(frontmatter).Count().IsGreaterThan(0);
            _ = await Assert.That(body).Contains("# Test Content");
            _ = await Assert.That(body).Contains("This is the body.");
        }
    }

    [Test]
    public async Task Parse_WithMissingFrontmatter_ReturnsEmptyDictionaryAndFullContent()
    {
        var content = """
# Test Content

This is the body.
""";

        var (frontmatter, body) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter).IsEmpty();
            _ = await Assert.That(body).IsEqualTo(content);
        }
    }

    [Test]
    public async Task Parse_WithEmptyFrontmatter_ReturnsEmptyDictionaryAndBody()
    {
        var content = """
---
---
# Test Content
""";

        var (frontmatter, body) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter).IsEmpty();
            _ = await Assert.That(body).Contains("# Test Content");
        }
    }

    [Test]
    public async Task Parse_WithMissingClosingDelimiter_ReturnsEmptyDictionaryAndOriginalContent()
    {
        var content = """
---
title: Test Title

# Test Content
""";

        var (frontmatter, body) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter).IsNotEmpty();
            _ = await Assert.That(body).IsEmpty();
        }
    }

    [Test]
    public async Task Parse_WithComplexYaml_ExtractsAllFields()
    {
        var content = """
---
title: Test Title
slug: test-slug
publishedDate: 2026-01-25T10:00:00+00:00
draft: false
tags:
  - tag1
  - tag2
---
Body content
""";
        var (frontmatter, _) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter.Count).IsGreaterThanOrEqualTo(3);
            _ = await Assert.That(frontmatter.ContainsKey("title")).IsTrue();
            _ = await Assert.That(frontmatter.ContainsKey("slug")).IsTrue();
            _ = await Assert.That(frontmatter.ContainsKey("publishedDate")).IsTrue();
        }
    }

    [Test]
    public void Parse_WithMalformedYaml_ThrowsInvalidOperationException()
    {
        var content = """
---
title: Test Title
  invalid yaml
slug test-slug
---
Body
""";

        _ = Assert.Throws<InvalidOperationException>(() => FrontmatterParser.Parse(content));
    }

    [Test]
    public void Parse_WhenContentIsNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>("markdownContent", () => FrontmatterParser.Parse(null!));

    [Test]
    public async Task Parse_WithCamelCaseKeys_ParsesCorrectly()
    {
        var content = """
---
title: Test
publishedDate: 2026-01-25T10:00:00+00:00
---
Body
""";
        var (frontmatter, _) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter.ContainsKey("title")).IsTrue();
            _ = await Assert.That(frontmatter.ContainsKey("publishedDate")).IsTrue();
        }
    }

    [Test]
    public async Task Parse_WithWhitespaceAroundDelimiters_HandlesCorrectly()
    {
        var content = """
  ---
title: Test
  ---
Body
""";

        var (frontmatter, body) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter).Count().IsGreaterThan(0);
            _ = await Assert.That(body).Contains("Body");
        }
    }

    [Test]
    public async Task Parse_WithEmptyBody_ReturnsEmptyString()
    {
        var content = """
---
title: Test
slug: test
publishedDate: 2026-01-25T10:00:00+00:00
---
""";

        var (frontmatter, body) = FrontmatterParser.Parse(content);

        using (Assert.Multiple())
        {
            _ = await Assert.That(frontmatter).Count().IsGreaterThan(0);
            _ = await Assert.That(body).IsEmpty();
        }
    }
}
