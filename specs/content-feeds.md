# RSS/Atom Feed Generation

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines the mechanism for generating RSS and Atom feeds from content in ForgingBlazor.

RSS and Atom feeds enable content syndication, allowing readers to subscribe to content updates via feed readers and enabling integration with external platforms.

---

## 2. Problem Statement

Content-driven sites typically provide RSS/Atom feeds for content subscription. ForgingBlazor currently has no built-in mechanism for feed generation from content stored in storage providers.

### 2.1 Requirements

| Requirement        | Description                                 |
| ------------------ | ------------------------------------------- |
| **RSS 2.0**        | Generate valid RSS 2.0 feeds                |
| **Atom 1.0**       | Generate valid Atom 1.0 feeds               |
| **Segment-scoped** | Feeds per segment (e.g., `/posts/feed.xml`) |
| **Configurable**   | Customizable feed metadata and item count   |
| **Culture-aware**  | Separate feeds per culture                  |

---

## 3. Goals

| Goal                     | Description                                       |
| ------------------------ | ------------------------------------------------- |
| **Standards Compliance** | Generate valid RSS 2.0 and Atom 1.0 feeds         |
| **Automatic Generation** | Feeds generated from content metadata             |
| **Caching**              | Feeds cached and invalidated with content changes |
| **Extensibility**        | Custom feed item mapping support                  |

---

## 4. Proposed Interface

```csharp
public interface IFeedGenerator
{
    ValueTask<FeedResult> GenerateRssAsync(
        string segmentPath,
        FeedOptions options,
        CancellationToken cancellationToken = default);

    ValueTask<FeedResult> GenerateAtomAsync(
        string segmentPath,
        FeedOptions options,
        CancellationToken cancellationToken = default);
}

public record FeedOptions
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? Author { get; init; }
    public int MaxItems { get; init; } = 20;
    public string? Culture { get; init; }
}

public record FeedResult
{
    public required string Content { get; init; }
    public required string ContentType { get; init; }
    public required DateTimeOffset LastModified { get; init; }
}
```

---

## 5. Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Feeds.Enabled = true;
    options.Feeds.DefaultMaxItems = 20;
    options.Feeds.IncludeSummary = true;
    options.Feeds.IncludeFullContent = false;
});
```

---

## 6. Routing Integration

```csharp
.WithSegment<PostList>("posts", segment => _ = segment
    .WithFeed(feed => feed
        .WithTitle("Blog Posts")
        .WithDescription("Latest blog posts")
        .WithMaxItems(25)
    )
    .WithPage<PostDetail>("{slug:alpha}", page => { })
)
```

Generated routes:

| URL                     | Format           |
| ----------------------- | ---------------- |
| `/posts/feed.xml`       | RSS 2.0          |
| `/posts/feed.atom`      | Atom 1.0         |
| `/de-DE/posts/feed.xml` | RSS 2.0 (German) |

---

## 7. Open Questions

- Should feeds be generated on-demand or pre-generated?
- How to handle content with `draft: true` in feeds?
- Should full content or summary be included by default?

---

## Revision History

| Version | Date       | Author                                        | Changes       |
| ------- | ---------- | --------------------------------------------- | ------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Initial draft |
