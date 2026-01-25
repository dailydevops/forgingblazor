# Content Aliases

> **Status:** Draft
> **Version:** 0.1.0
> **Created:** 2026-01-25
> **Last Modified:** 2026-01-25
> **Author:** [Martin Stühmer](https://github.com/samtrion)
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines content aliasing for ForgingBlazor.

Content aliases enable alternative URL paths to serve the same content, supporting URL migrations and canonical URL management.

---

## 2. Problem Statement

Content often needs to be accessible via multiple URLs:

- Legacy URL redirects after site restructuring
- Shortened URLs for sharing
- Alternative slugs (e.g., `/posts/2026/01/hello` and `/posts/hello-world`)

### 2.1 Requirements

| Requirement               | Description                                      |
| ------------------------- | ------------------------------------------------ |
| **Multiple Aliases**      | Single content accessible via multiple URL paths |
| **Canonical Declaration** | Define the primary/canonical URL for SEO         |
| **Redirect Support**      | HTTP 301 redirects from alias to canonical URL   |

---

## 3. Goals

| Goal                  | Description                                    |
| --------------------- | ---------------------------------------------- |
| **SEO Preservation**  | Maintain search rankings during URL migrations |
| **Flexibility**       | Support various aliasing patterns              |
| **Canonical Control** | Ensure proper canonical URL declaration        |

---

## 4. Proposed Interface

```csharp
public interface IContentAliasProvider
{
    ValueTask<ContentAlias?> ResolveAliasAsync(
        string aliasPath,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<string>> GetAliasesAsync(
        string contentPath,
        CancellationToken cancellationToken = default);

    ValueTask<string?> GetCanonicalPathAsync(
        string contentPath,
        CancellationToken cancellationToken = default);
}

public record ContentAlias
{
    public required string AliasPath { get; init; }
    public required string TargetPath { get; init; }
}
```

---

## 5. Frontmatter Schema

```yaml
---
title: "Hello World"
aliases:
  - /old-blog/hello-world
  - /posts/2026/01/25/hello
  - /hw
---
```

All aliases redirect (HTTP 301) to the canonical URL (the actual content path).

---

## 6. Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Aliases.Enabled = true;
    options.Aliases.ValidateOnStartup = true;
});
```

---

## 7. Canonical URL Handling

When aliases exist, the canonical URL is automatically set in the HTML head:

```html
<!-- For request to /old-blog/hello-world (alias) → redirects to canonical -->
<!-- At /posts/hello-world (canonical) -->
<link rel="canonical" href="https://example.com/posts/hello-world" />
```

### 7.1 Frontmatter Override

For content originally published elsewhere:

```yaml
---
title: "Hello World"
canonical: https://external-site.com/original-post
---
```

---

## 8. Alias Validation

Aliases are validated at startup:

| Validation              | Behavior                                         |
| ----------------------- | ------------------------------------------------ |
| **Target Exists**       | Warning if alias points to non-existent content  |
| **Circular References** | Error if A → B → A                               |
| **Duplicate Aliases**   | Error if same alias defined for multiple targets |

---

## 9. Use Cases

### 9.1 URL Migration

```yaml
# Old URL structure preserved after migration
aliases:
  - /2026/01/25/my-post.html
  - /archive/my-post
```

### 9.2 Vanity URLs

```yaml
# Short URLs for marketing/sharing
aliases:
  - /go/product-launch
  - /promo
```

---

## 10. Open Questions

- Should aliases support query string preservation?
- How to handle alias conflicts with configured routes?

---

## Revision History

| Version | Date       | Author                                        | Changes       |
| ------- | ---------- | --------------------------------------------- | ------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Initial draft |
