---
title: Dynamic Content Routing and Storage System
version: 1.0
date_created: 2026-01-25
last_updated: 2026-01-25
owner: ForgingBlazor Team
tags: [design, routing, storage, content-management, blazor, fluent-api]
state: approved
---

## Introduction

This specification defines the architecture and requirements for a dynamic content routing system with configurable storage providers. The system enables developers to define content structures via a FluentAPI at application startup, manage Markdown files with YAML frontmatter, and support multi-tier storage with automatic publishing workflows.

## 1. Purpose & Scope

### 1.1 Purpose

Provide a flexible, convention-based content management system for Blazor applications that:

- Allows developers to define routing structures via FluentAPI
- Supports Markdown content with YAML frontmatter
- Enables multi-culture content with fallback hierarchies
- Provides two-tier storage (local draft + publishing target)
- Automatically syncs content based on draft status

### 1.2 Scope

This specification covers:

- FluentAPI design for routing configuration
- Content descriptor model and frontmatter schema
- Storage provider abstractions and implementations
- Culture/localization handling
- Pagination system
- Publishing workflow
- Validation requirements

### 1.3 Out of Scope

- Tags/Categories as navigable segments
- Sitemap generation
- RSS/Atom feed generation
- Admin UI for content editing
- Database storage providers
- Archival or versioning of content
- User authentication/authorization
- Search functionality

### 1.4 Intended Audience

- Developers implementing the ForgingBlazor framework
- Developers using ForgingBlazor to build content-driven Blazor applications

## 2. Definitions

| Term                   | Definition                                                                         |
| ---------------------- | ---------------------------------------------------------------------------------- |
| **Content Descriptor** | Base class representing content metadata and body, derived from frontmatter        |
| **Segment**            | A paginated collection of content items (e.g., blog posts)                         |
| **Page**               | A single standalone content item (e.g., about page)                                |
| **Frontmatter**        | YAML metadata block at the beginning of Markdown files                             |
| **Slug**               | URL-friendly identifier for content, pattern: `[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]` |
| **Publishing Storage** | Remote storage target for published (non-draft) content                            |
| **Draft Storage**      | Local filesystem storage for content development                                   |
| **Canonical URL**      | The authoritative URL for content, used for SEO                                    |
| **LCID**               | Culture Identifier, e.g., `1033` for `en-US`                                       |
| **Full Culture**       | Culture in format `en-US`, `de-DE`, etc.                                           |
| **Two-Letter Code**    | ISO 639-1 language code (e.g., `en`, `de`)                                         |

## 3. Requirements, Constraints & Guidelines

### 3.1 FluentAPI Requirements

- **REQ-API-001**: The system MUST provide `AddRouting()` extension method on `IForgingBlazorApplicationBuilder`
- **REQ-API-002**: The system MUST provide `AddContentStorage()` extension method for content storage configuration
- **REQ-API-003**: The system MUST provide `AddAssetStorage()` extension method for asset storage configuration
- **REQ-API-004**: `ConfigureRoot()` MUST allow configuration of default culture, content type, component, and layout
- **REQ-API-005**: `MapSegment()` MUST allow mapping paginated content collections with configurable components and layouts
- **REQ-API-006**: `MapPage()` MUST allow mapping individual content pages with configurable components and layouts
- **REQ-API-007**: Nested segments MUST be supported (e.g., `blog/tutorials`)

### 3.2 Content Requirements

- **REQ-CNT-001**: All content MUST be stored as Markdown files with YAML frontmatter
- **REQ-CNT-002**: Standard frontmatter fields MUST include: `title` (required), `slug` (required), `publishedDate` (required)
- **REQ-CNT-003**: Standard optional frontmatter fields MUST include: `draft` (default: false), `expiredAt` (default: null)
- **REQ-CNT-004**: Content descriptors MUST inherit from `ContentDescriptor` base class
- **REQ-CNT-005**: Custom frontmatter fields MUST be supported via `WithMetadata()` configuration, and mapped to derived content descriptor properties
- **REQ-CNT-006**: All content MUST exist in the default culture
- **REQ-CNT-007**: Translated content MUST be supported via culture-specific Markdown files
- **REQ-CNT-008**: Missing translated content MUST fall back to default culture according to defined hierarchy

### 3.3 Routing Requirements

- **REQ-RTE-001**: Routes MUST be automatically resolved based on content structure and slugs
- **REQ-RTE-002**: Slug constraint MUST enforce pattern `[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]` (starts and ends with letter, 3-70 characters total)
- **REQ-RTE-003**: Culture MUST be configurable at root level only (segments and pages inherit)
- **REQ-RTE-004**: Canonical URL format MUST be configurable (with or without culture prefix)
- **REQ-RTE-005**: Non-canonical URLs MUST render normally with canonical link in HTML head (no redirects)

### 3.4 Culture Requirements

- **REQ-CUL-001**: Culture MUST be specifiable as Two-Letter code, LCID, or full format (e.g., `en-US`)
- **REQ-CUL-002**: Culture lookup MUST follow fallback hierarchy: `de-DE` → `de` → `en-US` (default) → `en`
- **REQ-CUL-003**: Default culture canonical format MUST be configurable: `/` or `/en-US` as canonical
- **REQ-CUL-004**: Alias URLs MUST be inherited by all child pages and segments
- **REQ-CUL-005**: Requests to non-canonical URLs MUST render content with canonical link in HTML head (no redirects)
- **REQ-CUL-006**: Missing default culture content MUST throw an exception at startup
- **REQ-CUL-007**: Requests for unsupported cultures MUST show not-found (404) response
- **REQ-CUL-008**: Culture MUST NOT be overridden at segment or page level

### 3.5 Pagination Requirements

- **REQ-PAG-001**: Pagination MUST be configurable via `WithPagination()` on root or segments
- **REQ-PAG-002**: Page size MUST be configurable, defaulting to 10 items per page, minimum 1
- **REQ-PAG-003**: URL format MUST be configurable: Numeric (`/posts/2`) or Prefixed (`/posts/page-2`)
- **REQ-PAG-004**: Page 1 MUST be canonical at segment root (e.g., `/posts`)
- **REQ-PAG-005**: Requests to `/posts/1` or `/posts/page-1` MUST render page 1 with canonical link to `/posts` (no redirects)
- **REQ-PAG-006**: Requests for out-of-range pages MUST return 404 response

### 3.6 Storage Requirements

- **REQ-STO-001**: FileSystem storage MUST always be available as draft storage
- **REQ-STO-002**: Publishing storage MUST be optional and configurable
- **REQ-STO-003**: Azure Blob Storage MUST be supported as initial publishing storage implementation
- **REQ-STO-004**: FileSystemWatcher MUST be supported for hot-reload during development
- **REQ-STO-005**: Asset storage MUST be separate from content storage
- **REQ-STO-006**: Storage providers MUST be configurable via FluentAPI with options pattern
- **REQ-STO-007**: Content MUST be looked up based on defined folder structure and naming conventions
- **REQ-STO-008**: Azure Blob Storage MUST implemented as a separate NuGet package, 'ForgingBlazor.Storage.AzureBlob'
- **REQ-STO-009**: Storage providers MUST implement `IContentStorageProvider` and `IAssetStorageProvider` interfaces
- **REQ-STO-010**: Content changes in draft storage MUST invalidate cache and reflect immediately if `WatchForChanges()` is enabled
- **REQ-STO-011**: The system MUST support multiple storage providers, but only one publishing target per type
- **REQ-STO-012**: User MUST confirm publishing actions before content is synced to publishing storage
- **REQ-STO-013**: User CAN `draft: true` content to be published for sharing with others, but it MUST NOT be rendered in production until `draft: false`

### 3.7 Publishing Requirements

- **REQ-PUB-001**: Content MUST automatically sync to publishing storage when `draft: false`
- **REQ-PUB-002**: Publishing sync MUST require user confirmation
- **REQ-PUB-003**: Content with `expiredAt` in the past MUST NOT be rendered (soft delete)
- **REQ-PUB-004**: Expired content MUST remain in storage but be excluded from routing
- **REQ-PUB-005**: Publishing service MUST implement `IPublishingService` interface
- **REQ-PUB-006**: Publishing MUST be supported for both content and assets

### 3.8 Validation Requirements

- **REQ-VAL-001**: Configuration MUST be validated at application startup (fail-fast)
- **REQ-VAL-002**: Content MUST be validated at runtime on first access
- **REQ-VAL-003**: Missing default culture content MUST throw an exception
- **REQ-VAL-004**: Invalid slugs MUST throw an exception
- **REQ-VAL-005**: Missing required frontmatter fields MUST throw an exception
- **REQ-VAL-006**: Segment mappings MUST validate existence of `_index.md` in default culture
- **REQ-VAL-007**: Page mappings MUST validate existence of content file in default culture

### 3.9 Component & Layout Requirements

- **REQ-CMP-001**: Root MUST define default component via `WithDefaultComponent<T>()`
- **REQ-CMP-002**: Root MUST define default layout via `WithDefaultLayout<T>()`
- **REQ-CMP-003**: Segments MUST support separate index and page components
- **REQ-CMP-004**: Segments MUST support separate index and page layouts
- **REQ-CMP-005**: Pages MUST support component and layout override
- **REQ-CMP-006**: All component/layout settings MUST inherit from root if not specified
- **REQ-CMP-007**: Components MUST implement `IComponent` and layouts MUST derive from `LayoutComponentBase`
- **REQ-CMP-008**: Missing component/layout configuration MUST throw an exception at startup
- **REQ-CMP-009**: Components are NOT ALLOWED to define `@page` directive; routing is handled by the system
- **REQ-CMP-010**: Components are NOT ALLOWED to define `@layout` directive; layout is handled by the system
- **REQ-CMP-011**: Components MUST receive `ResolvedContent<ContentDescriptor>` as a parameter for rendering

### 3.10 Constraints

- **CON-001**: Culture configuration MUST NOT be overridden at segment or page level
- **CON-002**: Route patterns MUST NOT be manually specified; they are derived from content structure
- **CON-003**: Source paths MUST NOT be manually specified; they are derived from segment/page names
- **CON-004**: Publishing storage provider can only be configured once per storage type
- **CON-005**: Default blazor routing constraints MUST be used/supported for slug validation
- **CON-006**: Routing constraints MUST be extendable in future versions
- **CON-007**: Content files MUST be UTF-8 encoded Markdown with valid YAML frontmatter
- **CON-008**: Slugs MUST conform to pattern `[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]` (starts and ends with letter, 3-70 characters total)

### 3.11 Guidelines

- **GUD-001**: SHOULD use meaningful segment names that reflect URL structure
- **GUD-002**: SHOULD organize content files in folders matching segment hierarchy
- **GUD-003**: SHOULD provide default culture content before adding translations
- **GUD-004**: SHOULD use descriptive slugs between 3-70 characters
- **GUD-005**: SHOULD keep frontmatter concise; use custom fields only when necessary
- **GUD-006**: SHOULD enable `WatchForChanges()` during development for faster iteration
- **GUD-007**: SHOULD implement custom content descriptors for complex metadata needs

### 3.12 Patterns

- **PAT-001**: FluentAPI builder pattern for all configuration
- **PAT-002**: Options pattern for storage provider configuration
- **PAT-003**: Strategy pattern for storage provider implementations
- **PAT-004**: Convention over configuration for content lookup
- **PAT-005**: Fail-fast validation at startup for configuration errors
- **PAT-006**: Dependency Injection for service abstractions
- **PAT-007**: Separation of concerns between content and asset storage

## 4. Interfaces & Data Contracts

### 4.1 FluentAPI Interfaces

```csharp
// Entry point extension method
public static class RoutingBuilderExtensions
{
    public static IForgingBlazorApplicationBuilder AddRouting(
        this IForgingBlazorApplicationBuilder builder,
        Action<IRoutingBuilder> configure);
}

// Root routing builder
public interface IRoutingBuilder
{
    IRoutingBuilder ConfigureRoot(Action<IRootConfiguration> configure);
    IRoutingBuilder MapSegment(string path, Action<ISegmentConfiguration>? configure = null);
    IRoutingBuilder MapPage(string path, Action<IPageConfiguration>? configure = null);
}

// Root configuration
public interface IRootConfiguration
{
    IRootConfiguration WithCulture(Action<ICultureConfiguration> configure);
    IRootConfiguration WithContentType<TDescriptor>() where TDescriptor : ContentDescriptor;
    IRootConfiguration WithDefaultComponent<TComponent>() where TComponent : IComponent;
    IRootConfiguration WithDefaultLayout<TLayout>() where TLayout : LayoutComponentBase;
    IRootConfiguration WithHomePage<TComponent>() where TComponent : IComponent;
}

// Culture configuration
public interface ICultureConfiguration
{
    ICultureConfiguration Default(string culture, CultureCanonical canonical = CultureCanonical.WithoutPrefix);
    ICultureConfiguration Supported(params string[] cultures);
}

// Segment configuration
public interface ISegmentConfiguration
{
    ISegmentConfiguration WithPagination(Action<IPaginationConfiguration> configure);
    ISegmentConfiguration WithContentType<TDescriptor>() where TDescriptor : ContentDescriptor;
    ISegmentConfiguration WithIndexComponent<TComponent>() where TComponent : IComponent;
    ISegmentConfiguration WithIndexLayout<TLayout>() where TLayout : LayoutComponentBase;
    ISegmentConfiguration WithPageComponent<TComponent>() where TComponent : IComponent;
    ISegmentConfiguration WithPageLayout<TLayout>() where TLayout : LayoutComponentBase;
    ISegmentConfiguration WithMetadata(Action<IMetadataConfiguration> configure);
}

// Page configuration
public interface IPageConfiguration
{
    IPageConfiguration WithContentType<TDescriptor>() where TDescriptor : ContentDescriptor;
    IPageConfiguration WithComponent<TComponent>() where TComponent : IComponent;
    IPageConfiguration WithLayout<TLayout>() where TLayout : LayoutComponentBase;
    IPageConfiguration WithMetadata(Action<IMetadataConfiguration> configure);
}

// Pagination configuration
public interface IPaginationConfiguration
{
    IPaginationConfiguration PageSize(int size = 10);
    IPaginationConfiguration UrlFormat(PaginationUrlFormat format, string? prefix = default);
}

// Metadata configuration
public interface IMetadataConfiguration
{
    IMetadataConfiguration ExtendWith<T>(string fieldName);
    IMetadataConfiguration ExtendWith<T>(string fieldName, T defaultValue);
}
```

### 4.2 Enumerations

```csharp
/// <summary>
/// Defines how the default culture is represented in URLs.
/// </summary>
public enum CultureCanonical
{
    /// <summary>
    /// Default culture uses "/" as canonical (e.g., "/" instead of "/en-US").
    /// </summary>
    WithoutPrefix,

    /// <summary>
    /// Default culture uses prefix as canonical (e.g., "/en-US").
    /// </summary>
    WithPrefix
}

/// <summary>
/// Defines the URL format for paginated content.
/// </summary>
public enum PaginationUrlFormat
{
    /// <summary>
    /// Numeric format: /posts/2, /posts/3
    /// </summary>
    Numeric,

    /// <summary>
    /// Prefixed format: /posts/page-2, /posts/page-3
    /// </summary>
    Prefixed
}
```

### 4.3 Content Descriptor

```csharp
/// <summary>
/// Base class for all content descriptors. Represents parsed frontmatter and content.
/// </summary>
public abstract class ContentDescriptor
{
    /// <summary>
    /// Gets or sets the content title. Required field.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the URL slug. Required field.
    /// </summary>
    /// <remarks>
    /// Slug must start and end with a letter, and can contain letters, numbers, and hyphens in between.
    /// Pattern: [A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z] (starts and ends with letter, 3-70 characters total)
    /// </remarks>
    public required string Slug { get; init; }

    /// <summary>
    /// Gets or sets the content published date. Required field.
    /// </summary>
    public required DateTimeOffset PublishedDate { get; init; }

    /// <summary>
    /// Gets or sets whether the content is a draft. Default: false.
    /// </summary>
    public bool Draft { get; init; } = false;

    /// <summary>
    /// Gets or sets the expiration date. Content is soft-deleted after this date.
    /// </summary>
    public DateTimeOffset? ExpiredAt { get; init; }

    /// <summary>
    /// Gets or sets the raw Markdown content (without frontmatter).
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Gets or sets additional frontmatter fields not mapped to properties.
    /// </summary>
    public IReadOnlyDictionary<string, object> ExtendedMetadata { get; init; }
        = new Dictionary<string, object>();
}
```

### 4.4 Storage Provider Interfaces

```csharp
// Content storage extension method
public static class ContentStorageExtensions
{
    public static IForgingBlazorApplicationBuilder AddContentStorage(
        this IForgingBlazorApplicationBuilder builder,
        Action<IContentStorageBuilder> configure);
}

// Asset storage extension method
public static class AssetStorageExtensions
{
    public static IForgingBlazorApplicationBuilder AddAssetStorage(
        this IForgingBlazorApplicationBuilder builder,
        Action<IAssetStorageBuilder> configure);
}

// Content storage builder
public interface IContentStorageBuilder
{
    IContentStorageBuilder UseFileSystem(Action<IFileSystemStorageOptions> configure);
    IContentStorageBuilder UseAzureBlobStorage(Action<IAzureBlobStorageOptions> configure); // As IContentStorageBuilder extension method available in 'ForgingBlazor.Storage.AzureBlob' package
}

// Asset storage builder
public interface IAssetStorageBuilder
{
    IAssetStorageBuilder UseFileSystem(Action<IFileSystemStorageOptions> configure);
    IAssetStorageBuilder UseAzureBlobStorage(Action<IAzureBlobStorageOptions> configure); // As IAssetStorageBuilder extension method available in 'ForgingBlazor.Storage.AzureBlob' package
}

// FileSystem options
public interface IFileSystemStorageOptions
{
    IFileSystemStorageOptions WithBasePath(string path); // Absolute or relative to application root
    IFileSystemStorageOptions WatchForChanges();
}

// Azure Blob Storage options
public interface IAzureBlobStorageOptions
{
    IAzureBlobStorageOptions WithConnectionString(string connectionString);
    IAzureBlobStorageOptions WithContainer(string containerName);
    IAzureBlobStorageOptions AsPublishingTarget();
}

// Storage provider abstraction
public interface IContentStorageProvider // and IAssetStorageProvider
{
    Task<ContentDescriptor?> GetContentAsync(string path, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentDescriptor>> GetContentsAsync(string segmentPath, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    // Additional methods for listing, saving, deleting content as needed
}

// Publishing service
public interface IPublishingService
{
    Task<bool> RequiresPublishingAsync(string path, CancellationToken cancellationToken = default);
    Task PublishAsync(string path, CancellationToken cancellationToken = default);
    Task UnpublishAsync(string path, CancellationToken cancellationToken = default);
}
```

### 4.5 Content Lookup Paths

| Content Type      | Lookup Order                                                                      |
| ----------------- | --------------------------------------------------------------------------------- |
| **Home Page**     | `{root}/_index.{culture}.md` → `{root}/_index.md`                                 |
| **Segment Index** | `{root}/{segment}/_index.{culture}.md` → `{root}/{segment}/_index.md`             |
| **Page (folder)** | `{root}/{segment}/{slug}/index.{culture}.md` → `{root}/{segment}/{slug}/index.md` |
| **Page (file)**   | `{root}/{segment}/{slug}.{culture}.md` → `{root}/{segment}/{slug}.md`             |

### 4.6 Culture Lookup Hierarchy

For a request with culture `de-DE`:

1. `{path}.de-DE.md`
2. `{path}.de.md`
3. `{path}.en-US.md` (default culture)
4. `{path}.en.md` (default culture fallback)
5. `{path}.md` (no culture = default)

## 5. Acceptance Criteria

### 5.1 FluentAPI Configuration

- **AC-001**: Given a new ForgingBlazor application, When `AddRouting()` is called with valid configuration, Then the routing system MUST be initialized without errors
- **AC-002**: Given a root configuration with culture settings, When segments are mapped, Then segments MUST inherit culture settings from root
- **AC-003**: Given a segment with pagination configured, When more items exist than page size, Then paginated URLs MUST be generated

### 5.2 Content Resolution

- **AC-004**: Given content exists at `content/posts/my-article.md`, When `/posts/my-article` is requested, Then the content MUST be resolved and rendered
- **AC-005**: Given content exists only in `de-DE` culture but not in default culture, When the application starts, Then an exception MUST be thrown
- **AC-006**: Given content with `draft: true`, When the content URL is requested, Then a 404 response MUST be returned, unless in local development mode with draft access enabled

### 5.3 Culture Handling

- **AC-007**: Given default culture is `en-US` with `CultureCanonical.WithoutPrefix`, When `/` is requested, Then the home page MUST be rendered with canonical link to `/`
- **AC-008**: Given default culture is `en-US` with `CultureCanonical.WithoutPrefix`, When `/en-US` is requested, Then the home page MUST be rendered with canonical link to `/`
- **AC-009**: Given culture `de-DE` is requested but content only exists in `de`, When the content is resolved, Then the `de` version MUST be returned

### 5.4 Pagination

- **AC-010**: Given a segment with 25 items and page size 10, When `/posts` is requested, Then page 1 (items 1-10) MUST be rendered
- **AC-011**: Given pagination format is `Numeric`, When `/posts/1` is requested, Then page 1 MUST be rendered with canonical link to `/posts`
- **AC-012**: Given pagination format is `Prefixed` with prefix `page-`, When `/posts/page-2` is requested, Then page 2 MUST be rendered with canonical link to `/posts/page-2`

### 5.5 Storage

- **AC-013**: Given FileSystem storage is configured with `WatchForChanges()`, When a Markdown file is modified, Then the content cache MUST be invalidated
- **AC-014**: Given Azure Blob Storage is configured as publishing target, When content changes from `draft: true` to `draft: false`, Then the content MUST be flagged for publishing
- **AC-015**: Given content has `expiredAt` in the past, When the content URL is requested, Then a 404 response MUST be returned

### 5.6 Validation

- **AC-016**: Given invalid slug format in frontmatter, When content is loaded, Then a validation exception MUST be thrown
- **AC-017**: Given missing required frontmatter field `title`, When content is loaded, Then a validation exception MUST be thrown
- **AC-018**: Given segment configured without existing `_index.md`, When application starts, Then a validation exception MUST be thrown

## 6. Test Automation Strategy

### 6.1 Test Levels

| Level       | Scope                                   | Framework                  |
| ----------- | --------------------------------------- | -------------------------- |
| Unit        | Individual classes, parsers, validators | TUnit                      |
| Integration | Storage providers, routing resolution   | TUnit                      |
| End-to-End  | Full request/response cycle             | TUnit + TestServer(Aspire) |

### 6.2 Test Categories

- **FluentAPI Tests**: Verify builder pattern and configuration validation
- **Frontmatter Parser Tests**: Verify YAML parsing and content descriptor mapping
- **Route Resolution Tests**: Verify URL to content mapping
- **Culture Fallback Tests**: Verify culture lookup hierarchy
- **Pagination Tests**: Verify page calculation and URL generation
- **Storage Provider Tests**: Verify file system and blob storage operations
- **Publishing Tests**: Verify draft/publish workflow
- **Validation Tests**: Verify startup and runtime validation

### 6.3 Test Data Management

- Use in-memory content for unit tests
- Use temporary file system directories for integration tests
- Use Azurite emulator for Azure Blob Storage integration tests
- Clean up test data after each test run

### 6.4 Coverage Requirements

- Minimum 80% code coverage for core libraries
- 100% coverage for validation logic
- 100% coverage for culture fallback logic

## 7. Rationale & Context

### 7.1 FluentAPI Design

The FluentAPI approach was chosen to:

- Provide compile-time type safety for component and layout configuration
- Enable IntelliSense support for developers
- Follow established .NET patterns (e.g., ASP.NET Core middleware)
- Allow progressive disclosure of complexity

### 7.2 Convention-Based Routing

Routes are automatically derived from content structure rather than manually configured to:

- Reduce configuration overhead
- Ensure consistency between file structure and URLs
- Minimize opportunities for configuration errors
- Follow the "convention over configuration" principle

### 7.3 Two-Tier Storage

The dual storage approach (local FileSystem + publishing target) enables:

- Local content development without affecting production
- Draft workflow with explicit publishing control
- Separation of concerns between authoring and delivery

### 7.4 Culture at Root Level Only

Culture configuration is restricted to root level to:

- Ensure consistent culture handling across all content
- Simplify the mental model for developers
- Prevent conflicting culture settings in nested segments
- Enable reliable canonical URL generation

### 7.5 Canonical Links vs Redirects

Canonical links are used instead of redirects to:

- Preserve user-facing URLs (no URL changes in browser)
- Reduce server load from redirect chains
- Provide clear SEO signals without affecting user experience
- Simplify routing logic

## 8. Dependencies & External Integrations

### 8.1 External Systems

- **EXT-001**: Azure Blob Storage - Cloud storage for published content and assets

### 8.2 Third-Party Services

- None required for core functionality

### 8.3 Infrastructure Dependencies

- **INF-001**: File System - Local storage for draft content and development
- **INF-002**: Azure Storage Account - Required for Azure Blob Storage publishing target

### 8.4 Data Dependencies

- **DAT-001**: Markdown files with YAML frontmatter - Content source format

### 8.5 Technology Platform Dependencies

- **PLT-001**: .NET 10 - Target framework
- **PLT-002**: ASP.NET Core Blazor - UI framework
- **PLT-003**: C# 13 - Programming language

### 8.6 Library Dependencies

| Library             | Purpose                     | Notes                      |
| ------------------- | --------------------------- | -------------------------- |
| YamlDotNet          | YAML frontmatter parsing    | Required                   |
| Markdig             | Markdown to HTML conversion | Required                   |
| Azure.Storage.Blobs | Azure Blob Storage client   | Optional, separate package |

### 8.7 Compliance Dependencies

- None identified

## 9. Examples & Edge Cases

### 9.1 Basic FluentAPI Configuration

```csharp
var builder = ForgingBlazorApplication.CreateDefaultBuilder(args)
    .AddRouting(routing => routing
        .ConfigureRoot(root => root
            .WithCulture(culture => culture
                .Default("en-US", canonical: CultureCanonical.WithoutPrefix)
                .Supported("de-DE", "de", "en-US", "en", "fr-FR", "fr"))
            .WithContentType<DefaultContentDescriptor>()
            .WithDefaultComponent<ContentPage>()
            .WithDefaultLayout<MainLayout>()
            .WithHomePage<HomePage>())

        .MapSegment("posts", segment => segment
            .WithPagination(pagination => pagination
                .PageSize(10)
                .UrlFormat(PaginationUrlFormat.Numeric))
            .WithContentType<BlogPostDescriptor>()
            .WithIndexComponent<BlogIndexPage>()
            .WithIndexLayout<BlogIndexLayout>()
            .WithPageComponent<BlogPostPage>()
            .WithPageLayout<BlogPostLayout>()
            .WithMetadata(meta => meta
                .ExtendWith<string>("author")
                .ExtendWith<string[]>("tags")))

        .MapSegment("blog/tutorials", segment => segment
            .WithPagination(pagination => pagination
                .PageSize(5)
                .UrlFormat(PaginationUrlFormat.Prefixed, "page-"))
            .WithContentType<TutorialDescriptor>()
            .WithIndexComponent<TutorialIndexPage>()
            .WithPageComponent<TutorialPage>())

        .MapPage("about", page => page
            .WithContentType<StaticPageDescriptor>()
            .WithComponent<AboutPage>()
            .WithLayout<MinimalLayout>())

        .MapPage("contact"))

    .AddContentStorage(storage => storage
        .UseFileSystem(fs => fs
            .WithBasePath("./content")
            .WatchForChanges())
        .UseAzureBlobStorage(blob => blob
            .WithConnectionString("UseDevelopmentStorage=true")
            .WithContainer("published-content")
            .AsPublishingTarget()))

    .AddAssetStorage(assets => assets
        .UseFileSystem(fs => fs
            .WithBasePath("./assets")
            .WatchForChanges())
        .UseAzureBlobStorage(blob => blob
            .WithConnectionString("UseDevelopmentStorage=true")
            .WithContainer("published-assets")
            .AsPublishingTarget()));
```

### 9.2 Content File Structure

```txt
content/
├── _index.md                           # Home page (default culture)
├── _index.de-DE.md                     # Home page (German)
├── about.md                            # About page
├── about.de-DE.md                      # About page (German)
├── contact/
│   └── index.md                        # Contact page (folder style)
├── posts/
│   ├── _index.md                       # Posts index page
│   ├── _index.de-DE.md                 # Posts index (German)
│   ├── getting-started.md              # Post (file style)
│   ├── getting-started.de-DE.md        # Post (German)
│   └── advanced-topics/
│       ├── index.md                    # Post (folder style)
│       └── index.de-DE.md              # Post (German)
└── blog/
    └── tutorials/
        ├── _index.md                   # Tutorials index
        └── first-tutorial.md           # Tutorial post
```

### 9.3 Frontmatter Example

```yaml
---
title: Getting Started with ForgingBlazor
slug: getting-started
publishedDate: 2026-01-25T10:00:00+01:00
draft: false
expiredAt: null
author: samtrion
tags:
  - blazor
  - tutorial
  - getting-started
---
## Getting Started

This is the content of the blog post...
```

### 9.4 Custom Content Descriptor

```csharp
public class BlogPostDescriptor : ContentDescriptor
{
    /// <summary>
    /// Gets or sets the post author.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Gets or sets the post tags.
    /// </summary>
    public string[] Tags { get; init; } = [];

    /// <summary>
    /// Gets or sets the estimated reading time in minutes.
    /// </summary>
    public int? ReadingTimeMinutes { get; init; }
}
```

### 9.5 Edge Cases

#### 9.5.1 Culture Fallback Chain

Request: `/de-DE/posts/my-article`

Lookup order:

1. `content/posts/my-article.de-DE.md` ❌ Not found
2. `content/posts/my-article.de.md` ❌ Not found
3. `content/posts/my-article.en-US.md` ❌ Not found
4. `content/posts/my-article.en.md` ❌ Not found
5. `content/posts/my-article.md` ✅ Found (renders with `de-DE` culture context)

#### 9.5.2 Pagination Edge Cases

| Scenario          | URL          | Behavior                           |
| ----------------- | ------------ | ---------------------------------- |
| Page 1 explicit   | `/posts/1`   | Render page 1, canonical: `/posts` |
| Page beyond range | `/posts/999` | Return 404                         |
| Negative page     | `/posts/-1`  | Return 404                         |
| Non-numeric page  | `/posts/abc` | Route not matched, try as slug     |

#### 9.5.3 Slug Validation Edge Cases

| Slug                               | Valid | Reason                         |
| ---------------------------------- | ----- | ------------------------------ |
| `my-article`                       | ✅    | Valid characters, valid length |
| `My-Article`                       | ✅    | Mixed case allowed             |
| `a123-post`                        | ✅    | Numbers allowed                |
| `my_article`                       | ❌    | Underscore not allowed         |
| `ab`                               | ❌    | Too short (min 3)              |
| `a-very-long-slug-...` (71+ chars) | ❌    | Too long (max 70)              |
| `one--double`                      | ❌    | Consecutive hyphens denied     |
| `trailing-`                        | ❌    | Trailing hyphen denied         |
| `-leading`                         | ❌    | Leading hyphen denied          |
| `trailing-123`                     | ❌    | Trailing number denied         |
| `123-leading`                      | ❌    | Leading number denied          |

## 10. Validation Criteria

### 10.1 Startup Validation

- [ ] All configured segments have corresponding content directories
- [ ] All configured pages have content files in default culture
- [ ] Culture configuration specifies at least one supported culture
- [ ] Default culture is included in supported cultures
- [ ] No duplicate segment or page paths
- [ ] Pagination page size is greater than zero
- [ ] Base paths for storage providers exist and are accessible

### 10.2 Runtime Validation

- [ ] Frontmatter contains all required fields (title, slug, publishedDate)
- [ ] Slug matches constraint pattern `[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]`
- [ ] PublishedDate is valid DateTimeOffset
- [ ] ExpiredAt, if present, is valid DateTimeOffset
- [ ] Content file is valid UTF-8 encoded Markdown
- [ ] YAML frontmatter is well-formed

### 10.3 Publishing Validation

- [ ] Content has `draft: false` before publishing
- [ ] Publishing storage provider is configured
- [ ] User has confirmed publishing action
- [ ] Content exists in default culture

## 11. Related Specifications / Further Reading

### 11.1 Internal References

- [Folder Structure and Naming Conventions](../decisions/2025-07-10-folder-structure-and-naming-conventions.md)
- [.NET 10 and C# 13 Adoption](../decisions/2025-07-11-dotnet-10-csharp-13-adoption.md)
- [DateTimeOffset and TimeProvider Usage](../decisions/2026-01-21-datetimeoffset-and-timeprovider-usage.md)

### 11.2 External References

- [ASP.NET Core Blazor Routing](https://learn.microsoft.com/aspnet/core/blazor/fundamentals/routing)
- [YamlDotNet Documentation](https://github.com/aaubry/YamlDotNet)
- [Markdig Documentation](https://github.com/xoofx/markdig)
- [Azure Blob Storage Documentation](https://learn.microsoft.com/azure/storage/blobs/)
- [Hugo Content Organization](https://gohugo.io/content-management/organization/) (inspiration for content structure)
