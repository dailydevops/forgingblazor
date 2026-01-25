# Content Relationships

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines content relationship management for ForgingBlazor.

Content relationships enable features like related posts, series/collections, and content linking, improving content discoverability and reader engagement.

---

## 2. Problem Statement

Content often has relationships to other content (related posts, series, prerequisites). ForgingBlazor currently has no mechanism for defining or querying content relationships.

### 2.1 Requirements

| Requirement             | Description                                        |
| ----------------------- | -------------------------------------------------- |
| **Related Content**     | Define and query related posts/pages               |
| **Series/Collections**  | Group content into ordered series                  |
| **Bidirectional Links** | Automatic backlink generation                      |
| **Similarity Matching** | Auto-suggest related content based on tags/content |

---

## 3. Goals

| Goal                | Description                                    |
| ------------------- | ---------------------------------------------- |
| **Discoverability** | Help readers find related content              |
| **Author Control**  | Manual relationship definition via frontmatter |
| **Automation**      | Auto-detect relationships where possible       |
| **Performance**     | Efficient relationship queries                 |

---

## 4. Proposed Interface

```csharp
public interface IContentRelationshipProvider
{
    ValueTask<IReadOnlyList<ContentReference>> GetRelatedContentAsync(
        string contentPath,
        RelationshipOptions options,
        CancellationToken cancellationToken = default);

    ValueTask<ContentSeries?> GetSeriesAsync(
        string seriesId,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<ContentReference>> GetBacklinksAsync(
        string contentPath,
        CancellationToken cancellationToken = default);
}

public record RelationshipOptions
{
    public int MaxResults { get; init; } = 5;
    public bool IncludeAutoDetected { get; init; } = true;
    public bool IncludeManual { get; init; } = true;
    public string? Culture { get; init; }
}

public record ContentReference
{
    public required string Path { get; init; }
    public required string Title { get; init; }
    public string? Summary { get; init; }
    public DateTimeOffset? PublishDate { get; init; }
    public RelationshipType Type { get; init; }
}

public enum RelationshipType
{
    Manual,
    TagBased,
    CategoryBased,
    ContentSimilarity
}

public record ContentSeries
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required IReadOnlyList<ContentReference> Items { get; init; }
}
```

---

## 5. Frontmatter Schema

### 5.1 Manual Related Content

```yaml
---
title: "Advanced Patterns"
related:
  - /posts/getting-started
  - /posts/intermediate-guide
---
```

### 5.2 Series Membership

```yaml
---
title: "Part 2: Configuration"
series:
  id: "blazor-tutorial"
  order: 2
---
```

### 5.3 Series Definition

Create a series index file:

```yaml
# content/series/blazor-tutorial/_index.md
---
title: "Blazor Tutorial Series"
description: "Complete guide to building Blazor applications"
seriesId: "blazor-tutorial"
---
```

---

## 6. Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Relationships.Enabled = true;
    options.Relationships.AutoDetectByTags = true;
    options.Relationships.AutoDetectByCategory = true;
    options.Relationships.MinTagOverlap = 2;
    options.Relationships.GenerateBacklinks = true;
});
```

---

## 7. Template Usage

```razor
@* PostDetail.razor *@
<article>
    <h1>@Content?.Descriptor.Title</h1>
    @((MarkupString)(Content?.RenderedHtml ?? string.Empty))
</article>

@if (Series is not null)
{
    <nav class="series-nav">
        <h3>@Series.Title</h3>
        <ol>
            @foreach (var item in Series.Items)
            {
                <li class="@(item.Path == CurrentPath ? "current" : "")">
                    <a href="@item.Path">@item.Title</a>
                </li>
            }
        </ol>
    </nav>
}

@if (RelatedPosts.Count > 0)
{
    <aside class="related-posts">
        <h3>Related Posts</h3>
        <ul>
            @foreach (var post in RelatedPosts)
            {
                <li><a href="@post.Path">@post.Title</a></li>
            }
        </ul>
    </aside>
}

@code {
    [CascadingParameter]
    public ResolvedContent<ContentDescriptor>? Content { get; set; }

    [Inject]
    public IContentRelationshipProvider RelationshipProvider { get; set; } = default!;

    private ContentSeries? Series { get; set; }
    private IReadOnlyList<ContentReference> RelatedPosts { get; set; } = [];
    private string CurrentPath => Content?.StoragePath ?? string.Empty;

    protected override async Task OnParametersSetAsync()
    {
        if (Content is not null)
        {
            Series = await RelationshipProvider.GetSeriesAsync(Content.Descriptor.Series?.Id);
            RelatedPosts = await RelationshipProvider.GetRelatedContentAsync(CurrentPath, new());
        }
    }
}
```

---

## 8. Open Questions

- How to handle relationships across cultures?
- Should backlinks be computed at build-time or runtime?
- How to rank related content (recency, popularity, similarity)?

---

## Revision History

| Version | Date       | Author                                        | Changes       |
| ------- | ---------- | --------------------------------------------- | ------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Initial draft |
