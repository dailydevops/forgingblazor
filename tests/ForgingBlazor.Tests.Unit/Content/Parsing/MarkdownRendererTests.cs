namespace NetEvolve.ForgingBlazor.Tests.Unit.Content.Parsing;

using System;
using NetEvolve.ForgingBlazor.Content.Parsing;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class MarkdownRendererTests
{
    [Test]
    public async Task Render_WithBasicMarkdown_RendersToHtml()
    {
        var markdown = "# Heading\n\nParagraph text.";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<h1");
            _ = await Assert.That(html).Contains("Heading");
            _ = await Assert.That(html).Contains("<p>");
            _ = await Assert.That(html).Contains("Paragraph text");
        }
    }

    [Test]
    public async Task Render_WithCodeBlock_RendersCodeBlock()
    {
        var markdown = """
```csharp
var x = 42;
```
""";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<pre");
            _ = await Assert.That(html).Contains("<code");
            _ = await Assert.That(html).Contains("var x = 42");
        }
    }

    [Test]
    public async Task Render_WithTable_RendersTable()
    {
        var markdown = """
| Header 1 | Header 2 |
|----------|----------|
| Cell 1   | Cell 2   |
""";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<table");
            _ = await Assert.That(html).Contains("<thead");
            _ = await Assert.That(html).Contains("<tbody");
            _ = await Assert.That(html).Contains("Header 1");
            _ = await Assert.That(html).Contains("Cell 1");
        }
    }

    [Test]
    public async Task Render_WithTaskList_RendersTaskList()
    {
        var markdown = """
- [x] Completed task
- [ ] Incomplete task
""";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<ul");
            _ = await Assert.That(html).Contains("<li");
            _ = await Assert.That(html).Contains("Completed task");
            _ = await Assert.That(html).Contains("Incomplete task");
        }
    }

    [Test]
    public async Task Render_WithEmoji_RendersEmoji()
    {
        var markdown = "Hello :smile:";

        var html = MarkdownRenderer.Render(markdown);

        _ = await Assert.That(html).IsNotNull();
    }

    [Test]
    public async Task Render_WithAutoLinks_RendersLinks()
    {
        var markdown = "Visit https://example.com for more info.";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<a");
            _ = await Assert.That(html).Contains("https://example.com");
        }
    }

    [Test]
    public async Task Render_WithNestedLists_RendersCorrectly()
    {
        var markdown = """
- Item 1
  - Nested Item 1
  - Nested Item 2
- Item 2
""";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<ul");
            _ = await Assert.That(html).Contains("<li");
            _ = await Assert.That(html).Contains("Item 1");
            _ = await Assert.That(html).Contains("Nested Item 1");
        }
    }

    [Test]
    public async Task Render_WithEmptyString_ReturnsEmptyString()
    {
        var html = MarkdownRenderer.Render("");

        _ = await Assert.That(html).IsEmpty();
    }

    [Test]
    public void Render_WhenMarkdownIsNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>("markdown", () => MarkdownRenderer.Render(null!));

    [Test]
    public async Task Render_WithMultipleParagraphs_RendersAllParagraphs()
    {
        var markdown = """
First paragraph.

Second paragraph.

Third paragraph.
""";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("First paragraph");
            _ = await Assert.That(html).Contains("Second paragraph");
            _ = await Assert.That(html).Contains("Third paragraph");
        }
    }

    [Test]
    public async Task Render_WithInlineCode_RendersCode()
    {
        var markdown = "Use `var` keyword in C#.";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<code");
            _ = await Assert.That(html).Contains("var");
        }
    }

    [Test]
    public async Task Render_WithBoldAndItalic_RendersFormatting()
    {
        var markdown = "This is **bold** and *italic* text.";

        var html = MarkdownRenderer.Render(markdown);

        using (Assert.Multiple())
        {
            _ = await Assert.That(html).Contains("<strong");
            _ = await Assert.That(html).Contains("bold");
            _ = await Assert.That(html).Contains("<em");
            _ = await Assert.That(html).Contains("italic");
        }
    }
}
