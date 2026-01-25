# Taxonomy Management

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines taxonomy management for ForgingBlazor.

Taxonomies (tags, categories) enable content organization and navigation, allowing readers to browse content by topic.

---

## 2. Problem Statement

Content uses tags and categories for organization. ForgingBlazor's `ContentDescriptor` includes `Tags` and `Categories` properties but lacks infrastructure for taxonomy pages and navigation.

### 2.1 Requirements

| Requirement        | Description                                 |
| ------------------ | ------------------------------------------- |
| **Tag Pages**      | Auto-generated pages listing content by tag |
| **Category Pages** | Hierarchical category navigation            |
| **Tag Cloud**      | Visual representation of tag popularity     |
| **Cross-segment**  | Taxonomies can span multiple segments       |
| **Culture-aware**  | Taxonomy terms can be localized             |

---

## 3. Goals

| Goal             | Description                                              |
| ---------------- | -------------------------------------------------------- |
| **Organization** | Structured content browsing                              |
| **Discovery**    | Help readers find content by topic                       |
| **SEO**          | Taxonomy pages improve site structure for search engines |
| **Flexibility**  | Support custom taxonomy types beyond tags/categories     |

---

## 4. Proposed Interface

```csharp
public interface ITaxonomyProvider
{
    ValueTask<IReadOnlyList<TaxonomyTerm>> GetTermsAsync(
        string taxonomyType,
        TaxonomyOptions options,
        CancellationToken cancellationToken = default);

    ValueTask<TaxonomyTerm?> GetTermAsync(
        string taxonomyType,
        string termSlug,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<ContentReference>> GetContentByTermAsync(
        string taxonomyType,
        string termSlug,
        TaxonomyContentOptions options,
        CancellationToken cancellationToken = default);
}

public record TaxonomyTerm
{
    public required string Slug { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public int ContentCount { get; init; }
    public TaxonomyTerm? Parent { get; init; }  // For hierarchical taxonomies
}

public record TaxonomyOptions
{
    public string? Culture { get; init; }
    public bool IncludeEmpty { get; init; } = false;
    public TaxonomySortOrder SortOrder { get; init; } = TaxonomySortOrder.Alphabetical;
}

public enum TaxonomySortOrder
{
    Alphabetical,
    ContentCount,
    Custom
}

public record TaxonomyContentOptions
{
    public string? Culture { get; init; }
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 10;
    public ContentSortOrder SortOrder { get; init; } = ContentSortOrder.PublishDateDescending;
}
```

---

## 5. Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Taxonomies.Enabled = true;

    // Built-in taxonomies
    options.Taxonomies.EnableTags = true;
    options.Taxonomies.EnableCategories = true;

    // Custom taxonomy
    options.Taxonomies.Add("series", taxonomy =>
    {
        taxonomy.DisplayName = "Series";
        taxonomy.Hierarchical = false;
        taxonomy.RoutePrefix = "series";
    });
});
```

---

## 6. Routing Integration

```csharp
.WithTaxonomy<TagList>("tags", taxonomy => _ = taxonomy
    .WithTermPage<TagDetail>("{tag:alpha}")
)
.WithTaxonomy<CategoryList>("categories", taxonomy => _ = taxonomy
    .WithTermPage<CategoryDetail>("{category:alpha}")
    .Hierarchical()  // Supports nested categories
)
```

Generated routes:

| URL                              | Component        | Description                     |
| -------------------------------- | ---------------- | ------------------------------- |
| `/tags`                          | `TagList`        | All tags                        |
| `/tags/blazor`                   | `TagDetail`      | Content tagged "blazor"         |
| `/categories`                    | `CategoryList`   | All categories                  |
| `/categories/tutorials`          | `CategoryDetail` | Content in "tutorials" category |
| `/categories/tutorials/beginner` | `CategoryDetail` | Nested category                 |

---

## 7. Frontmatter Usage

```yaml
---
title: "Getting Started with Blazor"
tags:
  - blazor
  - dotnet
  - web-development
categories:
  - tutorials/beginner
---
```

---

## 8. Template Components

### 8.1 Tag List Page

```razor
@* TagList.razor *@
<h1>Tags</h1>

<div class="tag-cloud">
    @foreach (var tag in Tags)
    {
        <a href="/tags/@tag.Slug"
           class="tag"
           style="font-size: @GetFontSize(tag.ContentCount)">
            @tag.Name (@tag.ContentCount)
        </a>
    }
</div>

@code {
    [Inject]
    public ITaxonomyProvider TaxonomyProvider { get; set; } = default!;

    private IReadOnlyList<TaxonomyTerm> Tags { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        Tags = await TaxonomyProvider.GetTermsAsync("tags", new());
    }

    private string GetFontSize(int count)
    {
        var size = Math.Min(2.5, 0.8 + (count * 0.1));
        return $"{size}rem";
    }
}
```

### 8.2 Tag Detail Page

```razor
@* TagDetail.razor *@
<h1>Posts tagged "@Term?.Name"</h1>

@if (Term?.Description is not null)
{
    <p class="description">@Term.Description</p>
}

<ul class="content-list">
    @foreach (var content in Contents)
    {
        <li>
            <a href="@content.Path">@content.Title</a>
            <time>@content.PublishDate?.ToString("yyyy-MM-dd")</time>
        </li>
    }
</ul>

@code {
    [Parameter]
    public string Tag { get; set; } = string.Empty;

    [Inject]
    public ITaxonomyProvider TaxonomyProvider { get; set; } = default!;

    private TaxonomyTerm? Term { get; set; }
    private IReadOnlyList<ContentReference> Contents { get; set; } = [];

    protected override async Task OnParametersSetAsync()
    {
        Term = await TaxonomyProvider.GetTermAsync("tags", Tag);
        Contents = await TaxonomyProvider.GetContentByTermAsync("tags", Tag, new());
    }
}
```

---

## 9. Localization

Taxonomy terms can be localized:

```
content/
├── _taxonomies/
│   ├── tags.yml           # Tag definitions (default)
│   └── tags.de-DE.yml     # German translations
```

```yaml
# tags.yml
blazor:
  name: "Blazor"
  description: "Microsoft's web UI framework"

# tags.de-DE.yml
blazor:
  name: "Blazor"
  description: "Microsofts Web-UI-Framework"
```

---

## 10. Open Questions

- Should taxonomy URLs be prefixed with culture segment?
- How to handle taxonomy merging/aliasing (e.g., "C#" and "csharp")?
- Should we support tag/category weights for ordering?

---

## Revision History

| Version | Date       | Author                                        | Changes       |
| ------- | ---------- | --------------------------------------------- | ------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Initial draft |
