# Sitemap Generation

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines the mechanism for generating XML sitemaps from content in ForgingBlazor.

Sitemaps help search engines discover and index content efficiently, improving SEO and ensuring all content is crawlable.

---

## 2. Problem Statement

Search engines rely on sitemaps to discover content. ForgingBlazor currently has no built-in mechanism for sitemap generation from configured routes and content.

### 2.1 Requirements

| Requirement            | Description                                          |
| ---------------------- | ---------------------------------------------------- |
| **Sitemap Protocol**   | Generate valid XML sitemaps per sitemap.org protocol |
| **Sitemap Index**      | Support sitemap index for large sites (>50,000 URLs) |
| **robots.txt**         | Integration with robots.txt generation               |
| **Priority/Frequency** | Configurable priority and change frequency           |
| **Multi-culture**      | Include alternate language URLs (hreflang)           |

---

## 3. Goals

| Goal                       | Description                                             |
| -------------------------- | ------------------------------------------------------- |
| **Standards Compliance**   | Generate valid sitemaps per sitemap.org protocol        |
| **Automatic Generation**   | Sitemaps generated from routing configuration           |
| **SEO Optimization**       | Include lastmod, priority, changefreq metadata          |
| **Multi-language Support** | Generate hreflang annotations for multi-culture content |

---

## 4. Proposed Interface

```csharp
public interface ISitemapGenerator
{
    ValueTask<SitemapResult> GenerateAsync(
        SitemapOptions options,
        CancellationToken cancellationToken = default);

    ValueTask<string> GenerateRobotsTxtAsync(
        RobotsTxtOptions options,
        CancellationToken cancellationToken = default);
}

public record SitemapOptions
{
    public string BaseUrl { get; init; } = string.Empty;
    public bool IncludeDrafts { get; init; } = false;
    public bool IncludeAlternateLanguages { get; init; } = true;
}

public record SitemapResult
{
    public required string Content { get; init; }
    public required DateTimeOffset LastModified { get; init; }
    public bool RequiresIndex { get; init; }
    public IReadOnlyList<string>? SitemapFiles { get; init; }
}
```

---

## 5. Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Sitemap.Enabled = true;
    options.Sitemap.BaseUrl = "https://example.com";
    options.Sitemap.IncludeAlternateLanguages = true;
    options.Sitemap.DefaultChangeFrequency = ChangeFrequency.Weekly;
    options.Sitemap.DefaultPriority = 0.5;
});
```

---

## 6. Routing Integration

```csharp
.WithSegment<PostList>("posts", segment => _ = segment
    .WithSitemapOptions(sitemap => sitemap
        .WithChangeFrequency(ChangeFrequency.Daily)
        .WithPriority(0.8)
    )
    .WithPage<PostDetail>("{slug:alpha}", page => _ = page
        .WithSitemapOptions(sitemap => sitemap
            .WithChangeFrequency(ChangeFrequency.Monthly)
            .WithPriority(0.6)
        )
    )
)
```

---

## 7. Generated Output

### 7.1 Sitemap XML

```xml
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9"
        xmlns:xhtml="http://www.w3.org/1999/xhtml">
  <url>
    <loc>https://example.com/posts/hello-world</loc>
    <lastmod>2026-01-25T12:00:00Z</lastmod>
    <changefreq>monthly</changefreq>
    <priority>0.6</priority>
    <xhtml:link rel="alternate" hreflang="en-US" href="https://example.com/posts/hello-world"/>
    <xhtml:link rel="alternate" hreflang="de-DE" href="https://example.com/de-DE/posts/hello-world"/>
  </url>
</urlset>
```

### 7.2 robots.txt

```
User-agent: *
Allow: /

Sitemap: https://example.com/sitemap.xml
```

---

## 8. Open Questions

- Should sitemap be generated on-demand or pre-generated at startup?
- How to handle content with `expiryDate` in the past?
- Should we support Google News sitemaps for time-sensitive content?

---

## Revision History

| Version | Date       | Author                                        | Changes       |
| ------- | ---------- | --------------------------------------------- | ------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Initial draft |
