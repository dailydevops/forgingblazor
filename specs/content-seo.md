# SEO Metadata Injection

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines how ForgingBlazor injects SEO metadata from content frontmatter into the HTML `<head>` element.

---

## 2. Problem Statement

Content frontmatter contains metadata that should appear in the HTML `<head>`:

- `title` → `<title>` and `<meta property="og:title">`
- `description` → `<meta name="description">` and `<meta property="og:description">`
- `featuredImage` → `<meta property="og:image">`

### 2.1 Current Behavior

Manual binding via Blazor `<PageTitle>` and `<HeadContent>` components is required.

### 2.2 Limitations

| Limitation         | Impact                                      |
| ------------------ | ------------------------------------------- |
| **Manual Binding** | Each template must bind metadata explicitly |
| **Boilerplate**    | Repetitive code across templates            |
| **Inconsistency**  | Easy to miss metadata fields                |

---

## 3. Goals

| Goal                    | Description                                |
| ----------------------- | ------------------------------------------ |
| **Automatic Injection** | Metadata injected without explicit binding |
| **SEO Standards**       | Support Open Graph, Twitter Cards, JSON-LD |
| **Override Capability** | Templates can override automatic values    |
| **Blazor Integration**  | Use `<HeadOutlet>` infrastructure          |

---

## 4. SEO Metadata Types

### 4.1 Basic HTML Meta

```html
<title>Page Title - Site Name</title>
<meta name="description" content="Page description for search engines" />
<meta name="author" content="Author Name" />
<link rel="canonical" href="https://example.com/posts/hello-world" />
```

### 4.2 Open Graph (Facebook, LinkedIn)

```html
<meta property="og:type" content="article" />
<meta property="og:title" content="Page Title" />
<meta property="og:description" content="Page description" />
<meta property="og:image" content="https://example.com/images/hero.png" />
<meta property="og:url" content="https://example.com/posts/hello-world" />
<meta property="og:site_name" content="Site Name" />
<meta property="article:published_time" content="2026-01-25T09:00:00Z" />
<meta property="article:author" content="Author Name" />
<meta property="article:tag" content="tag1" />
<meta property="article:tag" content="tag2" />
```

### 4.3 Twitter Cards

```html
<meta name="twitter:card" content="summary_large_image" />
<meta name="twitter:title" content="Page Title" />
<meta name="twitter:description" content="Page description" />
<meta name="twitter:image" content="https://example.com/images/hero.png" />
<meta name="twitter:creator" content="@authorhandle" />
<meta name="twitter:site" content="@sitehandle" />
```

### 4.4 JSON-LD Structured Data

```html
<script type="application/ld+json">
  {
    "@context": "https://schema.org",
    "@type": "Article",
    "headline": "Page Title",
    "description": "Page description",
    "image": "https://example.com/images/hero.png",
    "author": {
      "@type": "Person",
      "name": "Author Name"
    },
    "datePublished": "2026-01-25T09:00:00Z",
    "dateModified": "2026-01-25T12:00:00Z"
  }
</script>
```

---

## 5. Proposed Options

### 5.1 Automatic Injection

`ForgingRouteView` injects metadata automatically from frontmatter.

```razor
@* No explicit binding needed in templates *@
<article>
    <h1>@Content?.Descriptor.Title</h1>
    @((MarkupString)(Content?.RenderedHtml ?? string.Empty))
</article>
```

`ForgingRouteView` internally:

```csharp
protected override void OnParametersSet()
{
    if (Content?.Descriptor is { } descriptor)
    {
        // Inject via HeadOutlet
        _headContent = builder =>
        {
            builder.AddContent(0, new PageTitle { Text = descriptor.Title });
            builder.AddMetaTag("description", descriptor.Description);
            builder.AddMetaTag("og:title", descriptor.Title);
            // ... etc
        };
    }
}
```

#### Advantages

| Advantage            | Description                      |
| -------------------- | -------------------------------- |
| **Zero Boilerplate** | No code needed in templates      |
| **Consistent**       | All content gets proper metadata |

#### Disadvantages

| Disadvantage     | Description                                 |
| ---------------- | ------------------------------------------- |
| **Less Control** | Cannot customize without override mechanism |
| **Magic**        | Behavior not visible in template code       |

### 5.2 Manual Binding

Developer binds explicitly via `<PageTitle>` and `<HeadContent>`.

```razor
<PageTitle>@Content?.Descriptor.Title - My Site</PageTitle>
<HeadContent>
    <meta name="description" content="@Content?.Descriptor.Description">
    <meta property="og:title" content="@Content?.Descriptor.Title">
    @* ... more meta tags *@
</HeadContent>

<article>
    <h1>@Content?.Descriptor.Title</h1>
    @((MarkupString)(Content?.RenderedHtml ?? string.Empty))
</article>
```

#### Advantages

| Advantage    | Description                      |
| ------------ | -------------------------------- |
| **Explicit** | All behavior visible in template |
| **Flexible** | Full control over metadata       |

#### Disadvantages

| Disadvantage    | Description                      |
| --------------- | -------------------------------- |
| **Boilerplate** | Repetitive code across templates |
| **Error-prone** | Easy to miss fields              |

### 5.3 Dedicated Component

`<ForgingHeadContent>` component for content-based metadata.

```razor
<ForgingHeadContent
    Content="Content"
    SiteName="My Site"
    TwitterHandle="@sitehandle"
    IncludeOpenGraph="true"
    IncludeTwitterCards="true"
    IncludeJsonLd="true" />

<article>
    <h1>@Content?.Descriptor.Title</h1>
    @((MarkupString)(Content?.RenderedHtml ?? string.Empty))
</article>
```

#### Advantages

| Advantage        | Description                      |
| ---------------- | -------------------------------- |
| **Opt-in**       | Templates choose what to include |
| **Configurable** | Parameters control behavior      |
| **Visible**      | Component usage is explicit      |

#### Disadvantages

| Disadvantage      | Description                              |
| ----------------- | ---------------------------------------- |
| **Must Remember** | Still need to add component to templates |

### 5.4 Hybrid Approach

Automatic injection with override capability.

```csharp
services.AddForgingBlazor(options =>
{
    options.Seo.AutoInject = true;  // Default: true
    options.Seo.SiteName = "My Site";
    options.Seo.TwitterHandle = "@sitehandle";
    options.Seo.IncludeOpenGraph = true;
    options.Seo.IncludeTwitterCards = true;
    options.Seo.IncludeJsonLd = true;
    options.Seo.TitleFormat = "{title} - {siteName}";  // Customizable
});
```

Template can override:

```razor
@* Override automatic title *@
<PageTitle>Custom Title Override</PageTitle>

<article>
    <h1>@Content?.Descriptor.Title</h1>
</article>
```

**Resolution Order:**

1. Explicit `<PageTitle>` / `<HeadContent>` in template (highest priority)
2. Automatic injection from frontmatter
3. Default values from configuration

---

## 6. Frontmatter Extensions

```yaml
---
title: "Page Title"
description: "Page description"
featuredImage: "/images/hero.png"

# SEO-specific (optional)
seo:
  title: "Override Title for SEO" # Override title
  description: "Override description for meta"
  noIndex: false # robots noindex
  canonical: "/posts/original-post" # canonical URL override
  openGraph:
    type: "article"
    image: "/images/og-specific.png"
  twitter:
    card: "summary_large_image"
    creator: "@authorhandle"
  jsonLd:
    type: "Article"
    # Additional JSON-LD properties
---
```

---

## 7. Open Questions

| #   | Question                                                  | Context                                             |
| --- | --------------------------------------------------------- | --------------------------------------------------- |
| 1   | Should injection be automatic or opt-in?                  | Developer experience vs. explicit control           |
| 2   | Should Blazor's `<HeadOutlet>` and `<PageTitle>` be used? | Leverage existing infrastructure                    |
| 3   | Which Open Graph properties to support?                   | og:type, og:title, og:description, og:image, og:url |
| 4   | Should Twitter Cards be supported?                        | Additional meta tags                                |
| 5   | Should canonical URLs be auto-generated?                  | `<link rel="canonical">`                            |
| 6   | Should JSON-LD structured data be supported?              | SEO best practices                                  |
| 7   | How to handle image URLs (relative vs. absolute)?         | CDN integration                                     |

---

## 8. Related Specifications

- [Routing Specification](./routing.md) - Parent specification
- [Routing Specification §9](./routing.md#9-content-descriptor) - Content Descriptor
- [Routing Specification §5.4](./routing.md#54-asset-provider) - Asset Provider (for image URLs)

---

## 9. References

- [Open Graph Protocol](https://ogp.me/)
- [Twitter Cards](https://developer.twitter.com/en/docs/twitter-for-websites/cards/overview/abouts-cards)
- [Schema.org Article](https://schema.org/Article)
- [Blazor HeadOutlet](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/control-head-content)

---

## Revision History

| Version | Date       | Author  | Changes       |
| ------- | ---------- | ------- | ------------- |
| 0.1.0   | 2026-01-25 | Initial | Initial draft |
