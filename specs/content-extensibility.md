# Content Extensibility Specification

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## Table of Contents

1. [Overview](#1-overview)
2. [Goals and Objectives](#2-goals-and-objectives)
3. [ContentDescriptor Base Properties](#3-contentdescriptor-base-properties)
4. [Derived Content Types](#4-derived-content-types)
5. [Property Categories](#5-property-categories)
6. [Interface Definitions](#6-interface-definitions)
7. [YAML Frontmatter Mapping](#7-yaml-frontmatter-mapping)
8. [Open Questions](#8-open-questions)

---

## 1. Overview

This specification defines the extensibility model for content descriptors in ForgingBlazor. It establishes which properties belong in the base `ContentDescriptor` class and which should be implemented in derived types for specific content scenarios.

### 1.1 Design Principles

| Principle                    | Description                                                              |
| ---------------------------- | ------------------------------------------------------------------------ |
| **Minimal Base**             | Base class contains only universally applicable properties               |
| **Explicit Derivation**      | Specialized content types derive from base with additional properties    |
| **YAML Compatibility**       | All properties map to YAML frontmatter in Markdown files                 |
| **Type Safety**              | Strongly-typed properties with nullable reference types where applicable |
| **TimeProvider Integration** | Date/time properties use `DateTimeOffset` for testability                |

---

## 2. Goals and Objectives

### 2.1 Primary Goals

| Goal                       | Description                                                         |
| -------------------------- | ------------------------------------------------------------------- |
| **Extensibility**          | Enable custom content types through inheritance                     |
| **Consistency**            | Provide standard properties for common content operations           |
| **SEO Support**            | Include properties for search engine optimization                   |
| **Publishing Workflow**    | Support draft/published states and scheduled publishing             |

### 2.2 Non-Goals

| Non-Goal                   | Rationale                                                           |
| -------------------------- | ------------------------------------------------------------------- |
| **Dynamic Properties**     | No dictionary-based arbitrary properties; use derived types instead |
| **Runtime Type Discovery** | Content types are compile-time defined                              |

---

## 3. ContentDescriptor Base Properties

### 3.1 Current Properties (Existing)

| Property      | Type      | Purpose                                      |
| ------------- | --------- | -------------------------------------------- |
| `Slug`        | `string?` | URL-friendly identifier for the content      |
| `Title`       | `string?` | Display title of the content                 |
| `LinkTitle`   | `string?` | Short title for navigation/links             |
| `Description` | `string?` | Brief description for SEO meta tags          |
| `Summary`     | `string?` | Longer summary for content previews          |
| `Author`      | `string?` | Content author name or identifier            |

### 3.2 Proposed Additional Properties

#### 3.2.1 Publishing & Lifecycle

| Property       | Type               | Purpose                                      | Required |
| -------------- | ------------------ | -------------------------------------------- | -------- |
| `PublishDate`  | `DateTimeOffset?`  | When the content was/will be published       | No       |
| `LastModified` | `DateTimeOffset?`  | Last modification timestamp                  | No       |
| `ExpiryDate`   | `DateTimeOffset?`  | When the content should be unpublished       | No       |
| `IsDraft`      | `bool`             | Whether content is in draft state            | Yes      |

#### 3.2.2 Localization

| Property  | Type      | Purpose                                           | Required |
| --------- | --------- | ------------------------------------------------- | -------- |
| `Culture` | `string?` | Content culture (e.g., `de-DE`, `en-US`)          | No       |

#### 3.2.3 Organization

| Property | Type   | Purpose                                           | Required |
| -------- | ------ | ------------------------------------------------- | -------- |
| `Weight` | `int`  | Sort order within segment (lower = higher)        | Yes      |

#### 3.2.4 SEO & Discovery

| Property       | Type            | Purpose                                      | Required |
| -------------- | --------------- | -------------------------------------------- | -------- |
| `Keywords`     | `List<string>?` | SEO keywords for meta tags                   | No       |
| `CanonicalUrl` | `string?`       | Canonical URL for duplicate content          | No       |
| `Aliases`      | `List<string>?` | Alternative URLs that redirect to this page  | No       |
| `NoIndex`      | `bool`          | Exclude from search engine indexing          | Yes      |
| `NoFollow`     | `bool`          | Prevent search engines from following links  | Yes      |

#### 3.2.5 Visual

| Property        | Type      | Purpose                                      | Required |
| --------------- | --------- | -------------------------------------------- | -------- |
| `FeaturedImage` | `string?` | Path to featured/hero image                  | No       |

### 3.3 Proposed ContentDescriptor Definition

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Base content descriptor containing properties common to all content types.
/// </summary>
public record ContentDescriptor
{
    // Identity
    public string? Slug { get; init; }
    public string? Title { get; init; }
    public string? LinkTitle { get; init; }

    // Content
    public string? Description { get; init; }
    public string? Summary { get; init; }
    public string? Author { get; init; }

    // Publishing & Lifecycle
    public DateTimeOffset? PublishDate { get; init; }
    public DateTimeOffset? LastModified { get; init; }
    public DateTimeOffset? ExpiryDate { get; init; }
    public bool IsDraft { get; init; }

    // Localization
    public string? Culture { get; init; }

    // Organization
    public int Weight { get; init; }

    // SEO & Discovery
    public IReadOnlyList<string>? Keywords { get; init; }
    public string? CanonicalUrl { get; init; }
    public IReadOnlyList<string>? Aliases { get; init; }
    public bool NoIndex { get; init; }
    public bool NoFollow { get; init; }

    // Visual
    public string? FeaturedImage { get; init; }
}
```

---

## 4. Derived Content Types

### 4.1 BlogPostDescriptor

For blog posts with additional metadata:

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Content descriptor for blog posts with additional blog-specific metadata.
/// </summary>
public record BlogPostDescriptor : ContentDescriptor
{
    /// <summary>
    /// Estimated reading time in minutes.
    /// </summary>
    public int? ReadingTimeMinutes { get; init; }

    /// <summary>
    /// Categories for the blog post.
    /// </summary>
    public IReadOnlyList<string>? Categories { get; init; }

    /// <summary>
    /// Tags for the blog post.
    /// </summary>
    public IReadOnlyList<string>? Tags { get; init; }

    /// <summary>
    /// Series name if part of a multi-part series.
    /// </summary>
    public string? Series { get; init; }

    /// <summary>
    /// Position within the series (1-based).
    /// </summary>
    public int? SeriesOrder { get; init; }

    /// <summary>
    /// Slugs of related posts for cross-linking.
    /// </summary>
    public IReadOnlyList<string>? RelatedPosts { get; init; }

    /// <summary>
    /// Whether comments are enabled for this post.
    /// </summary>
    public bool AllowComments { get; init; } = true;
}
```

### 4.2 ProjectDescriptor

For project/portfolio showcases:

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Content descriptor for project showcases.
/// </summary>
public record ProjectDescriptor : ContentDescriptor
{
    /// <summary>
    /// URL to the project repository (GitHub, GitLab, etc.).
    /// </summary>
    public string? RepositoryUrl { get; init; }

    /// <summary>
    /// URL to a live demo or website.
    /// </summary>
    public string? DemoUrl { get; init; }

    /// <summary>
    /// URL to documentation.
    /// </summary>
    public string? DocumentationUrl { get; init; }

    /// <summary>
    /// Technologies/frameworks used in the project.
    /// </summary>
    public IReadOnlyList<string>? Technologies { get; init; }

    /// <summary>
    /// Current project status (Active, Maintained, Archived, etc.).
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    /// License identifier (MIT, Apache-2.0, etc.).
    /// </summary>
    public string? License { get; init; }

    /// <summary>
    /// Project start date.
    /// </summary>
    public DateTimeOffset? StartDate { get; init; }

    /// <summary>
    /// Project end/completion date.
    /// </summary>
    public DateTimeOffset? EndDate { get; init; }
}
```

### 4.3 EventDescriptor

For events, conferences, meetups:

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Content descriptor for events.
/// </summary>
public record EventDescriptor : ContentDescriptor
{
    /// <summary>
    /// Event start date and time.
    /// </summary>
    public DateTimeOffset? StartDate { get; init; }

    /// <summary>
    /// Event end date and time.
    /// </summary>
    public DateTimeOffset? EndDate { get; init; }

    /// <summary>
    /// Physical location or venue name.
    /// </summary>
    public string? Location { get; init; }

    /// <summary>
    /// Full address of the venue.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// URL for online/virtual events.
    /// </summary>
    public string? OnlineUrl { get; init; }

    /// <summary>
    /// Whether the event is online, in-person, or hybrid.
    /// </summary>
    public string? EventType { get; init; }

    /// <summary>
    /// Registration or ticket URL.
    /// </summary>
    public string? RegistrationUrl { get; init; }

    /// <summary>
    /// Maximum number of attendees.
    /// </summary>
    public int? Capacity { get; init; }

    /// <summary>
    /// Event organizer name or organization.
    /// </summary>
    public string? Organizer { get; init; }
}
```

### 4.4 PageDescriptor

For static pages with minimal additional metadata:

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Content descriptor for static pages (About, Contact, etc.).
/// </summary>
public record PageDescriptor : ContentDescriptor
{
    /// <summary>
    /// Whether to show the page in navigation menus.
    /// </summary>
    public bool ShowInNavigation { get; init; } = true;

    /// <summary>
    /// Parent page slug for hierarchical navigation.
    /// </summary>
    public string? ParentSlug { get; init; }
}
```

---

## 5. Property Categories

### 5.1 Base vs. Derived Decision Matrix

| Property             | Base | Derived | Rationale                                         |
| -------------------- | ---- | ------- | ------------------------------------------------- |
| `Slug`               | ✓    |         | Universal content identifier                      |
| `Title`              | ✓    |         | All content has a title                           |
| `Description`        | ✓    |         | Universal SEO requirement                         |
| `PublishDate`        | ✓    |         | All content has a publish date                    |
| `IsDraft`            | ✓    |         | All content can be drafted                        |
| `Culture`            | ✓    |         | Localization applies to all content               |
| `Keywords`           | ✓    |         | SEO applies to all content                        |
| `Categories`         |      | ✓       | Not all content types use categorization          |
| `Tags`               |      | ✓       | Not all content types use tagging                 |
| `ReadingTime`        |      | ✓       | Specific to article-like content                  |
| `RepositoryUrl`      |      | ✓       | Specific to project content                       |
| `StartDate/EndDate`  |      | ✓       | Specific to time-bound content (events, projects) |
| `Location`           |      | ✓       | Specific to location-based content                |

### 5.2 Property Naming Conventions

| Convention           | Example                    | Rationale                              |
| -------------------- | -------------------------- | -------------------------------------- |
| **Dates**            | `PublishDate`, `StartDate` | Suffix with `Date` for `DateTimeOffset`|
| **URLs**             | `RepositoryUrl`, `DemoUrl` | Suffix with `Url` for links            |
| **Booleans**         | `IsDraft`, `NoIndex`       | Prefix with `Is`, `No`, `Allow`, etc.  |
| **Collections**      | `Tags`, `Categories`       | Plural form for lists                  |
| **Time Durations**   | `ReadingTimeMinutes`       | Include unit in name                   |

---

## 6. Interface Definitions

### 6.1 IContentDescriptor Interface

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Base interface for all content descriptors.
/// </summary>
public interface IContentDescriptor
{
    string? Slug { get; }
    string? Title { get; }
    string? Description { get; }
    DateTimeOffset? PublishDate { get; }
    bool IsDraft { get; }
}
```

### 6.2 IPublishable Interface

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Interface for content with full publishing lifecycle.
/// </summary>
public interface IPublishable
{
    DateTimeOffset? PublishDate { get; }
    DateTimeOffset? ExpiryDate { get; }
    bool IsDraft { get; }

    /// <summary>
    /// Determines if content is currently published based on the given time.
    /// </summary>
    bool IsPublished(DateTimeOffset currentTime);
}
```

### 6.3 ISeoMetadata Interface

```csharp
namespace NetEvolve.ForgingBlazor.Content;

/// <summary>
/// Interface for content with SEO metadata.
/// </summary>
public interface ISeoMetadata
{
    string? Title { get; }
    string? Description { get; }
    IReadOnlyList<string>? Keywords { get; }
    string? CanonicalUrl { get; }
    bool NoIndex { get; }
    bool NoFollow { get; }
}
```

---

## 7. YAML Frontmatter Mapping

### 7.1 Base ContentDescriptor Mapping

```yaml
---
slug: my-content-slug
title: My Content Title
linkTitle: My Content
description: A brief description for SEO
summary: A longer summary for previews
author: Martin Stühmer

publishDate: 2026-01-25T10:00:00+01:00
lastModified: 2026-01-25T14:30:00+01:00
expiryDate: 2027-01-25T00:00:00+01:00
draft: false

culture: de-DE
weight: 10

keywords:
  - blazor
  - routing
  - content

canonicalUrl: https://example.com/canonical-path
aliases:
  - /old-path
  - /alternative-path

noIndex: false
noFollow: false
featuredImage: /images/featured.webp
---
```

### 7.2 BlogPostDescriptor Mapping

```yaml
---
# Base properties...
title: My Blog Post

# Blog-specific properties
readingTime: 5
categories:
  - Technology
  - .NET
tags:
  - blazor
  - tutorial
series: Blazor Deep Dive
seriesOrder: 3
relatedPosts:
  - getting-started-with-blazor
  - blazor-components-explained
allowComments: true
---
```

### 7.3 Property Name Mapping

| C# Property         | YAML Key           | Notes                          |
| ------------------- | ------------------ | ------------------------------ |
| `PublishDate`       | `publishDate`      | camelCase                      |
| `LastModified`      | `lastModified`     | camelCase                      |
| `IsDraft`           | `draft`            | Simplified key                 |
| `FeaturedImage`     | `featuredImage`    | camelCase                      |
| `ReadingTimeMinutes`| `readingTime`      | Simplified key                 |
| `SeriesOrder`       | `seriesOrder`      | camelCase                      |
| `RepositoryUrl`     | `repositoryUrl`    | camelCase                      |

---

## 8. Open Questions

| ID   | Question                                                                                   | Status |
| ---- | ------------------------------------------------------------------------------------------ | ------ |
| Q1   | Should `Tags` and `Categories` be in base class since they're commonly used?               | Open   |
| Q2   | Should we provide a generic `CustomDescriptor<TData>` for user-defined properties?         | Open   |
| Q3   | How should unknown YAML frontmatter properties be handled (ignore, error, dictionary)?     | Open   |
| Q4   | Should `Author` be a string or a reference to an `AuthorDescriptor`?                       | Open   |
| Q5   | Should we support multiple authors per content item?                                       | Open   |
| Q6   | Should `ReadingTime` be auto-calculated or always manually specified?                      | Open   |
| Q7   | Should derived types be registered explicitly or discovered via reflection?                | Open   |
| Q8   | How to handle versioning of content descriptor schemas?                                    | Open   |

---

## Appendix A: Related Specifications

| Specification                                                      | Relationship                              |
| ------------------------------------------------------------------ | ----------------------------------------- |
| [Routing Specification](./routing.md)                              | Parent specification                      |
| [Content SEO](./content-seo.md)                                    | SEO property usage                        |
| [Content Aliases](./content-aliases.md)                            | Alias property implementation             |
| [Content Taxonomy](./content-taxonomy.md)                          | Tags/Categories implementation            |
| [Content Feeds](./content-feeds.md)                                | Feed generation uses descriptors          |

---

## Appendix B: Revision History

| Version | Date       | Author                                          | Changes         |
| ------- | ---------- | ----------------------------------------------- | --------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion)   | Initial draft   |
