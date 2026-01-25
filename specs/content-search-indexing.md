# Content Search and Indexing

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines the mechanism for indexing and searching content in ForgingBlazor.

Full-text search enables users to find content across the site, improving content discoverability and user experience.

---

## 2. Problem Statement

Content-driven sites need search functionality. ForgingBlazor currently has no built-in mechanism for content indexing or search.

### 2.1 Requirements

| Requirement           | Description                                     |
| --------------------- | ----------------------------------------------- |
| **Full-text Search**  | Search across content body and metadata         |
| **Relevance Ranking** | Results ranked by relevance                     |
| **Faceted Search**    | Filter by tags, categories, dates               |
| **Highlighting**      | Highlight matching terms in results             |
| **Culture-aware**     | Search within specific culture or cross-culture |

---

## 3. Goals

| Goal                 | Description                                                   |
| -------------------- | ------------------------------------------------------------- |
| **Fast Search**      | Sub-second search response times                              |
| **Relevant Results** | TF-IDF or similar ranking algorithm                           |
| **Offline-capable**  | Client-side search option for static sites                    |
| **Pluggable**        | Support for external search providers (Algolia, Azure Search) |

---

## 4. Proposed Interface

```csharp
public interface IContentSearchProvider
{
    ValueTask IndexContentAsync(
        ContentDescriptor content,
        string renderedText,
        CancellationToken cancellationToken = default);

    ValueTask<SearchResults> SearchAsync(
        SearchQuery query,
        CancellationToken cancellationToken = default);

    ValueTask RemoveFromIndexAsync(
        string contentPath,
        CancellationToken cancellationToken = default);
}

public record SearchQuery
{
    public required string Terms { get; init; }
    public string? Culture { get; init; }
    public IReadOnlyList<string>? Tags { get; init; }
    public IReadOnlyList<string>? Categories { get; init; }
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 10;
}

public record SearchResults
{
    public required IReadOnlyList<SearchResult> Items { get; init; }
    public required int TotalCount { get; init; }
}

public record SearchResult
{
    public required string Path { get; init; }
    public required string Title { get; init; }
    public required string Excerpt { get; init; }
    public required double Score { get; init; }
    public IReadOnlyList<string>? HighlightedFragments { get; init; }
}
```

---

## 5. Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Search.Enabled = true;
    options.Search.Provider = SearchProvider.InMemory; // or Algolia, AzureSearch
    options.Search.IndexOnStartup = true;
    options.Search.IncludeDrafts = false;
});

// External provider example
services.AddForgingBlazorSearch(search => search
    .UseAlgolia(config =>
    {
        config.ApplicationId = "...";
        config.ApiKey = "...";
        config.IndexName = "content";
    })
);
```

---

## 6. Search Providers

### 6.1 In-Memory Provider

Built-in provider using Lucene.NET or similar for small to medium sites.

| Aspect          | Description                 |
| --------------- | --------------------------- |
| **Package**     | `ForgingBlazor` (built-in)  |
| **Use Case**    | Small sites (<10,000 pages) |
| **Persistence** | Index rebuilt on startup    |

### 6.2 External Providers

| Provider                   | Package                              | Use Case                              |
| -------------------------- | ------------------------------------ | ------------------------------------- |
| **Algolia**                | `ForgingBlazor.Search.Algolia`       | High-traffic sites, advanced features |
| **Azure Cognitive Search** | `ForgingBlazor.Search.Azure`         | Azure-hosted applications             |
| **Elasticsearch**          | `ForgingBlazor.Search.Elasticsearch` | Self-hosted, large-scale search       |

---

## 7. Client-Side Search

For static site generation scenarios, generate a search index as JSON:

```csharp
services.AddForgingBlazor(options =>
{
    options.Search.GenerateClientIndex = true;
    options.Search.ClientIndexPath = "/search-index.json";
});
```

Client-side search using libraries like Lunr.js or Fuse.js.

---

## 8. Open Questions

- Should indexing happen synchronously or asynchronously?
- How to handle partial matches and typo tolerance?
- What metadata fields should be indexed by default?

---

## Revision History

| Version | Date       | Author                                        | Changes       |
| ------- | ---------- | --------------------------------------------- | ------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Initial draft |
