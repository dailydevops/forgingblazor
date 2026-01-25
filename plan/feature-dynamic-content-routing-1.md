---
goal: Implement Dynamic Content Routing and Storage System for ForgingBlazor
version: 1.0
date_created: 2026-01-25
last_updated: 2026-01-25T00:00:00Z
owner: ForgingBlazor Team
status: In progress
tags: [feature, routing, storage, content-management, blazor, fluent-api]
---

# Introduction

![Status: In progress](https://img.shields.io/badge/status-In%20progress-blue)

This implementation plan defines the complete development roadmap for the Dynamic Content Routing and Storage System based on [spec-design-dynamic-content-routing.md](../spec/spec-design-dynamic-content-routing.md). The system enables developers to define content structures via FluentAPI, manage Markdown files with YAML frontmatter, and support multi-tier storage with automatic publishing workflows.

---

## AI Agent Instructions

Mandatory guidance for executing this plan. Keep it simple, precise, and consistent with project decisions.

### Principles

- **MUST** work in English and follow all accepted ADRs in `decisions/`.
- **MUST** run phases sequentially and complete one task at a time.
- **MAY** run tasks in parallel only when there are no dependencies.
- **MUST** fail-fast: stop and report blockers before proceeding.

### Task Workflow

- Read the task, confirm prerequisites, and list affected files.
- Implement per `.github/instructions/*.instructions.md` and plan requirements.
- Add XML docs to all public APIs.
- Build: `dotnet restore` then `dotnet build ForgingBlazor.slnx --no-restore`.
- For tests: `dotnet test --solution ForgingBlazor.slnx --no-build --no-restore --ignore-exit-code 8`.
- Update the task table; mark ✅ only after successful build/tests.
- Commit using Conventional Commits (see [decisions/2025-07-10-conventional-commits.md](../decisions/2025-07-10-conventional-commits.md)). Example:

```txt
feat(routing): add IRoutingBuilder and root config

Implements TASK-005, TASK-006
Refs: TASK-005, TASK-006
```

### Quality Gates

- No new warnings; tests pass; XML docs present; constraints respected.

### Phase Completion

- Add a completion note, verify tests, and update `last_updated` in frontmatter.

### Progress Reporting

- After each phase, add a brief report (completed tasks, created/modified files, tests added, notes).

### Per-Phase Testing

- End every phase with unit/integration tests for new elements.

---

## 1. Requirements & Constraints

### 1.1 Functional Requirements

- **REQ-API-001**: Provide `AddRouting()` extension method on `IForgingBlazorApplicationBuilder`
- **REQ-API-002**: Provide `AddContentStorage()` extension method for content storage configuration
- **REQ-API-003**: Provide `AddAssetStorage()` extension method for asset storage configuration
- **REQ-API-004**: `ConfigureRoot()` MUST allow configuration of default culture, content type, component, and layout
- **REQ-API-005**: `MapSegment()` MUST allow mapping paginated content collections
- **REQ-API-006**: `MapPage()` MUST allow mapping individual content pages
- **REQ-API-007**: Nested segments MUST be supported (e.g., `blog/tutorials`)
- **REQ-CNT-001**: All content MUST be stored as Markdown files with YAML frontmatter
- **REQ-CNT-002**: Standard frontmatter fields: `title` (required), `slug` (required), `publishedDate` (required)
- **REQ-CNT-003**: Optional frontmatter fields: `draft` (default: false), `expiredAt` (default: null)
- **REQ-CNT-004**: Content descriptors MUST inherit from `ContentDescriptor` base class
- **REQ-CNT-005**: Custom frontmatter fields via `WithMetadata()` configuration
- **REQ-CNT-006**: All content MUST exist in the default culture
- **REQ-CNT-007**: Translated content MUST be supported via culture-specific Markdown files
- **REQ-CNT-008**: Missing translated content MUST fall back to default culture
- **REQ-RTE-001**: Routes MUST be automatically resolved based on content structure
- **REQ-RTE-002**: Slug constraint: `[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]` (3-70 characters)
- **REQ-RTE-003**: Culture configurable at root level only
- **REQ-RTE-004**: Canonical URL format configurable (with or without culture prefix)
- **REQ-RTE-005**: Non-canonical URLs render with canonical link in HTML head
- **REQ-CUL-001**: Culture specifiable as Two-Letter code, LCID, or full format
- **REQ-CUL-002**: Culture fallback: `de-DE` → `de` → `en-US` (default) → `en`
- **REQ-CUL-003**: Default culture canonical format configurable
- **REQ-CUL-004**: Alias URLs inherited by all child pages and segments
- **REQ-CUL-005**: Non-canonical URLs render content with canonical link
- **REQ-CUL-006**: Missing default culture content throws exception at startup
- **REQ-CUL-007**: Unsupported cultures return 404 response
- **REQ-CUL-008**: Culture NOT overridable at segment or page level
- **REQ-PAG-001**: Pagination configurable via `WithPagination()` on root or segments
- **REQ-PAG-002**: Page size configurable (default: 10, minimum: 1)
- **REQ-PAG-003**: URL format: Numeric (`/posts/2`) or Prefixed (`/posts/page-2`)
- **REQ-PAG-004**: Page 1 canonical at segment root
- **REQ-PAG-005**: `/posts/1` renders page 1 with canonical link to `/posts`
- **REQ-PAG-006**: Out-of-range pages return 404
- **REQ-STO-001**: FileSystem storage always available as draft storage
- **REQ-STO-002**: Publishing storage optional and configurable
- **REQ-STO-003**: Azure Blob Storage as initial publishing storage implementation
- **REQ-STO-004**: FileSystemWatcher for hot-reload during development
- **REQ-STO-005**: Asset storage separate from content storage
- **REQ-STO-006**: Storage providers configurable via FluentAPI with options pattern
- **REQ-STO-007**: Content lookup based on defined folder structure and naming conventions
- **REQ-STO-008**: Azure Blob Storage as separate NuGet package `ForgingBlazor.Storage.AzureBlob`
- **REQ-STO-009**: Storage providers implement `IContentStorageProvider` and `IAssetStorageProvider`
- **REQ-STO-010**: Content changes in draft storage invalidate cache if `WatchForChanges()` enabled
- **REQ-STO-011**: Support multiple storage providers, one publishing target per type
- **REQ-STO-012**: User confirmation required before publishing
- **REQ-STO-013**: Draft content publishable for sharing, not rendered in production
- **REQ-PUB-001**: Content auto-syncs to publishing storage when `draft: false`
- **REQ-PUB-002**: Publishing sync requires user confirmation
- **REQ-PUB-003**: Content with `expiredAt` in past NOT rendered
- **REQ-PUB-004**: Expired content remains in storage but excluded from routing
- **REQ-PUB-005**: Publishing service implements `IPublishingService` interface
- **REQ-PUB-006**: Publishing supported for content and assets
- **REQ-VAL-001**: Configuration validated at startup (fail-fast)
- **REQ-VAL-002**: Content validated at runtime on first access
- **REQ-VAL-003**: Missing default culture content throws exception
- **REQ-VAL-004**: Invalid slugs throw exception
- **REQ-VAL-005**: Missing required frontmatter fields throw exception
- **REQ-VAL-006**: Segment mappings validate existence of `_index.md`
- **REQ-VAL-007**: Page mappings validate existence of content file
- **REQ-CMP-001**: Root defines default component via `WithDefaultComponent<T>()`
- **REQ-CMP-002**: Root defines default layout via `WithDefaultLayout<T>()`
- **REQ-CMP-003**: Segments support separate index and page components
- **REQ-CMP-004**: Segments support separate index and page layouts
- **REQ-CMP-005**: Pages support component and layout override
- **REQ-CMP-006**: Component/layout settings inherit from root if not specified
- **REQ-CMP-007**: Components implement `IComponent`, layouts derive from `LayoutComponentBase`
- **REQ-CMP-008**: Missing component/layout throws exception at startup
- **REQ-CMP-009**: Components NOT ALLOWED to define `@page` directive
- **REQ-CMP-010**: Components NOT ALLOWED to define `@layout` directive
- **REQ-CMP-011**: Components receive `ResolvedContent<ContentDescriptor>` as parameter

### 1.2 Security Requirements

- **SEC-001**: Content paths MUST be sanitized to prevent path traversal attacks
- **SEC-002**: Storage connection strings MUST be read from secure configuration sources
- **SEC-003**: Publishing actions MUST require explicit user confirmation

### 1.3 Performance Requirements

- **PRF-001**: Content MUST be cached after first access
- **PRF-002**: Cache invalidation MUST be triggered by FileSystemWatcher events
- **PRF-003**: Startup validation MUST be parallelizable where possible

### 1.4 Constraints

- **CON-001**: Culture configuration NOT overridable at segment or page level
- **CON-002**: Route patterns NOT manually specified; derived from content structure
- **CON-003**: Source paths NOT manually specified; derived from segment/page names
- **CON-004**: Only one publishing storage provider per storage type
- **CON-005**: Default Blazor routing constraints used/supported for slug validation
- **CON-006**: Routing constraints extendable in future versions
- **CON-007**: Content files MUST be UTF-8 encoded Markdown with valid YAML frontmatter
- **CON-008**: Slugs MUST conform to pattern `[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]`
- **CON-DEC-001**: MUST use `DateTimeOffset` instead of `DateTime` (Decision: 2026-01-21)
- **CON-DEC-002**: MUST use `TimeProvider` for obtaining current time (Decision: 2026-01-21)
- **CON-DEC-003**: MUST NOT use `#region`/`#endregion` (Decision: 2026-01-21)
- **CON-DEC-004**: MUST write all code in English (Decision: 2025-07-11)
- **CON-DEC-005**: MUST target .NET 10 with C# 13 (Decision: 2025-07-11)
- **CON-DEC-006**: MUST use centralized package version management (Decision: 2025-07-10)

### 1.5 Guidelines

- **GUD-001**: Use meaningful segment names reflecting URL structure
- **GUD-002**: Organize content files in folders matching segment hierarchy
- **GUD-003**: Provide default culture content before adding translations
- **GUD-004**: Use descriptive slugs between 3-70 characters
- **GUD-005**: Keep frontmatter concise; use custom fields only when necessary
- **GUD-006**: Enable `WatchForChanges()` during development
- **GUD-007**: Implement custom content descriptors for complex metadata needs

### 1.6 Patterns

- **PAT-001**: FluentAPI builder pattern for all configuration
- **PAT-002**: Options pattern for storage provider configuration
- **PAT-003**: Strategy pattern for storage provider implementations
- **PAT-004**: Convention over configuration for content lookup
- **PAT-005**: Fail-fast validation at startup for configuration errors
- **PAT-006**: Dependency Injection for service abstractions
- **PAT-007**: Separation of concerns between content and asset storage

## 2. Implementation Steps

### Phase 1: Core Interfaces and Abstractions

- GOAL-001: Define all public interfaces and abstractions in `ForgingBlazor.Extensibility` project

| Task     | Description                                                                                                                                                                                                                                                                                                                                                                      | Completed | Date       |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---------- |
| TASK-001 | Create `ContentDescriptor` base class in `src/ForgingBlazor.Extensibility/Content/ContentDescriptor.cs` with properties: `Title` (required string), `Slug` (required string), `PublishedDate` (required DateTimeOffset), `Draft` (bool, default false), `ExpiredAt` (DateTimeOffset?, default null), `Body` (string for Markdown content), `BodyHtml` (string for rendered HTML) | ✅        | 2026-01-25 |
| TASK-002 | Create `ResolvedContent<TDescriptor>` class in `src/ForgingBlazor.Extensibility/Content/ResolvedContent.cs` wrapping content descriptor with culture information, canonical URL, and resolved route data                                                                                                                                                                         | ✅        | 2026-01-25 |
| TASK-003 | Create `CultureCanonical` enum in `src/ForgingBlazor.Extensibility/Routing/CultureCanonical.cs` with values: `WithoutPrefix`, `WithPrefix`                                                                                                                                                                                                                                       | ✅        | 2026-01-25 |
| TASK-004 | Create `PaginationUrlFormat` enum in `src/ForgingBlazor.Extensibility/Pagination/PaginationUrlFormat.cs` with values: `Numeric`, `Prefixed`                                                                                                                                                                                                                                      | ✅        | 2026-01-25 |
| TASK-005 | Create `IRoutingBuilder` interface in `src/ForgingBlazor.Extensibility/Routing/IRoutingBuilder.cs` with methods: `ConfigureRoot()`, `MapSegment()`, `MapPage()`                                                                                                                                                                                                                  | ✅        | 2026-01-25 |
| TASK-006 | Create `IRootConfiguration` interface in `src/ForgingBlazor.Extensibility/Routing/IRootConfiguration.cs` with methods: `WithCulture()`, `WithDefaultContentType<T>()`, `WithDefaultComponent<T>()`, `WithDefaultLayout<T>()`, `WithHomePage<T>()`                                                                                                                                | ✅        | 2026-01-25 |
| TASK-007 | Create `ICultureConfiguration` interface in `src/ForgingBlazor.Extensibility/Routing/ICultureConfiguration.cs` with methods: `Default()`, `Supported()`                                                                                                                                                                                                                          | ✅        | 2026-01-25 |
| TASK-008 | Create `ISegmentConfiguration` interface in `src/ForgingBlazor.Extensibility/Routing/ISegmentConfiguration.cs` with methods: `WithPagination()`, `WithContentType<T>()`, `WithIndexComponent<T>()`, `WithIndexLayout<T>()`, `WithPageComponent<T>()`, `WithPageLayout<T>()`, `WithMetadata()`, `MapSegment()`                                                                    | ✅        | 2026-01-25 |
| TASK-009 | Create `IPageConfiguration` interface in `src/ForgingBlazor.Extensibility/Routing/IPageConfiguration.cs` with methods: `WithContentType<T>()`, `WithComponent<T>()`, `WithLayout<T>()`, `WithMetadata()`                                                                                                                                                                         | ✅        | 2026-01-25 |
| TASK-010 | Create `IPaginationConfiguration` interface in `src/ForgingBlazor.Extensibility/Pagination/IPaginationConfiguration.cs` with methods: `PageSize()`, `UrlFormat()`                                                                                                                                                                                                                | ✅        | 2026-01-25 |
| TASK-011 | Create `IMetadataConfiguration` interface in `src/ForgingBlazor.Extensibility/Content/IMetadataConfiguration.cs` with methods: `ExtendWith<T>()` (with and without default value)                                                                                                                                                                                                | ✅        | 2026-01-25 |
| TASK-012 | Create `IContentStorageProvider` interface in `src/ForgingBlazor.Extensibility/Storage/IContentStorageProvider.cs` with methods: `GetContentAsync()`, `GetContentsAsync()`, `ExistsAsync()`, `SaveContentAsync()`, `DeleteContentAsync()`                                                                                                                                        | ✅        | 2026-01-25 |
| TASK-013 | Create `IAssetStorageProvider` interface in `src/ForgingBlazor.Extensibility/Storage/IAssetStorageProvider.cs` with methods: `GetAssetAsync()`, `GetAssetsAsync()`, `ExistsAsync()`, `SaveAssetAsync()`, `DeleteAssetAsync()`                                                                                                                                                    | ✅        | 2026-01-25 |
| TASK-014 | Create `IContentStorageBuilder` interface in `src/ForgingBlazor.Extensibility/Storage/IContentStorageBuilder.cs` with method: `UseFileSystem()`                                                                                                                                                                                                                                  | ✅        | 2026-01-25 |
| TASK-015 | Create `IAssetStorageBuilder` interface in `src/ForgingBlazor.Extensibility/Storage/IAssetStorageBuilder.cs` with method: `UseFileSystem()`                                                                                                                                                                                                                                      | ✅        | 2026-01-25 |
| TASK-016 | Create `IFileSystemStorageOptions` interface in `src/ForgingBlazor.Extensibility/Storage/IFileSystemStorageOptions.cs` with methods: `WithBasePath()`, `WatchForChanges()`                                                                                                                                                                                                       | ✅        | 2026-01-25 |
| TASK-017 | Create `IPublishingService` interface in `src/ForgingBlazor.Extensibility/Storage/IPublishingService.cs` with methods: `RequiresPublishingAsync()`, `PublishAsync()`, `UnpublishAsync()`                                                                                                                                                                                         | ✅        | 2026-01-25 |

### Phase 2: Routing Configuration Builders

- GOAL-002: Implement FluentAPI builders for routing configuration in `ForgingBlazor` project

| Task     | Description                                                                                                                                                                                                                        | Completed | Date |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-018 | Create `RoutingBuilderExtensions` static class in `src/ForgingBlazor/Routing/RoutingBuilderExtensions.cs` with `AddRouting()` extension method on `IForgingBlazorApplicationBuilder` accepting `Action<IRoutingBuilder>` parameter | ✅        | 2026-01-25 |
| TASK-019 | Create `RoutingBuilder` internal class in `src/ForgingBlazor/Routing/RoutingBuilder.cs` implementing `IRoutingBuilder` with internal state for root configuration, segments, and pages                                             | ✅        | 2026-01-25 |
| TASK-020 | Create `RootConfiguration` internal class in `src/ForgingBlazor/Routing/Configurations/RootConfiguration.cs` implementing `IRootConfiguration` storing culture settings, default component/layout types, and home page component   | ✅        | 2026-01-25 |
| TASK-021 | Create `CultureConfiguration` internal class in `src/ForgingBlazor/Routing/Configurations/CultureConfiguration.cs` implementing `ICultureConfiguration` with support for Two-Letter code, LCID, and full culture format parsing    | ✅        | 2026-01-25 |
| TASK-022 | Create `SegmentConfiguration` internal class in `src/ForgingBlazor/Routing/Configurations/SegmentConfiguration.cs` implementing `ISegmentConfiguration` with support for nested segments via recursive `MapSegment()` calls        | ✅        | 2026-01-25 |
| TASK-023 | Create `PageConfiguration` internal class in `src/ForgingBlazor/Routing/Configurations/PageConfiguration.cs` implementing `IPageConfiguration` with component/layout overrides and metadata configuration                          | ✅        | 2026-01-25 |
| TASK-024 | Create `PaginationConfiguration` internal class in `src/ForgingBlazor/Routing/Configurations/PaginationConfiguration.cs` implementing `IPaginationConfiguration` with page size (default: 10, min: 1) and URL format settings      | ✅        | 2026-01-25 |
| TASK-025 | Create `MetadataConfiguration` internal class in `src/ForgingBlazor/Routing/Configurations/MetadataConfiguration.cs` implementing `IMetadataConfiguration` storing custom field definitions with types and default values          | ✅        | 2026-01-25 |
| TASK-026 | Create `SlugRouteConstraint` class in `src/ForgingBlazor/Routing/Constraints/SlugRouteConstraint.cs` implementing `IRouteConstraint` with regex pattern `^[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]$`                                     | ✅        | 2026-01-25 |
| TASK-027 | Update `Check` class in `src/ForgingBlazor.Extensibility/Check.cs` adding `IsValidSlug()` method with pattern validation returning bool and `ValidateSlug()` method throwing `ArgumentException` on invalid input                  | ✅        | 2026-01-25 |

#### Phase 2 Report (2026-01-25)

- Completed: TASK-018 through TASK-027 delivering routing builder extensions, configuration builders, slug constraint, and validation helpers.
- Files: Implementations added under `src/ForgingBlazor/Routing/**`, slug helpers in `src/ForgingBlazor.Extensibility/Check.cs`, unit coverage in `tests/ForgingBlazor.Tests.Unit/Routing/` and `tests/ForgingBlazor.Extensibility.Tests.Unit/`.
- Tests: `dotnet build ForgingBlazor.slnx --no-restore`, `dotnet test --solution ForgingBlazor.slnx --no-build --no-restore --ignore-exit-code 8`.
- Notes: Routing state snapshots ensure immutable configuration for DI registration and enforce slug correctness across FluentAPI usage.

### Phase 3: Content Parsing and Processing

- GOAL-003: Implement Markdown and YAML frontmatter parsing with content descriptor mapping

| Task     | Description                                                                                                                                                                                                                          | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- | ---- |
| TASK-028 | Add `YamlDotNet` package reference to `Directory.Packages.props` with latest stable version                                                                                                                                          |           |      |
| TASK-029 | Add `Markdig` package reference to `Directory.Packages.props` with latest stable version                                                                                                                                             |           |      |
| TASK-030 | Add package references to `ForgingBlazor.csproj` for `YamlDotNet` and `Markdig` without version attributes                                                                                                                           |           |      |
| TASK-031 | Create `FrontmatterParser` internal class in `src/ForgingBlazor/Content/Parsing/FrontmatterParser.cs` using YamlDotNet to extract YAML between `---` delimiters and return Dictionary<string, object>                                |           |      |
| TASK-032 | Create `MarkdownRenderer` internal class in `src/ForgingBlazor/Content/Parsing/MarkdownRenderer.cs` using Markdig with pipeline configured for: advanced extensions, syntax highlighting, tables, task lists                         |           |      |
| TASK-033 | Create `ContentParser` internal class in `src/ForgingBlazor/Content/Parsing/ContentParser.cs` orchestrating frontmatter extraction, YAML parsing, Markdown rendering, and content descriptor instantiation                           |           |      |
| TASK-034 | Create `ContentDescriptorFactory` internal class in `src/ForgingBlazor/Content/ContentDescriptorFactory.cs` using reflection to instantiate content descriptor types and map frontmatter properties including custom metadata fields |           |      |
| TASK-035 | Create `ContentValidationException` class in `src/ForgingBlazor.Extensibility/Content/ContentValidationException.cs` deriving from `Exception` with properties for field name, expected type, and actual value                       |           |      |
| TASK-036 | Create `FrontmatterValidation` internal class in `src/ForgingBlazor/Content/Validation/FrontmatterValidation.cs` validating required fields (title, slug, publishedDate), slug format, and DateTimeOffset parsing                    |           |      |

### Phase 4: Storage Provider Implementations

- GOAL-004: Implement FileSystem storage provider with watch capability

| Task     | Description                                                                                                                                                                                                                            | Completed | Date |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-037 | Create `ContentStorageExtensions` static class in `src/ForgingBlazor/Storage/ContentStorageExtensions.cs` with `AddContentStorage()` extension method on `IForgingBlazorApplicationBuilder` accepting `Action<IContentStorageBuilder>` |           |      |
| TASK-038 | Create `AssetStorageExtensions` static class in `src/ForgingBlazor/Storage/AssetStorageExtensions.cs` with `AddAssetStorage()` extension method on `IForgingBlazorApplicationBuilder` accepting `Action<IAssetStorageBuilder>`         |           |      |
| TASK-039 | Create `ContentStorageBuilder` internal class in `src/ForgingBlazor/Storage/ContentStorageBuilder.cs` implementing `IContentStorageBuilder` with fluent configuration state                                                            |           |      |
| TASK-040 | Create `AssetStorageBuilder` internal class in `src/ForgingBlazor/Storage/AssetStorageBuilder.cs` implementing `IAssetStorageBuilder` with fluent configuration state                                                                  |           |      |
| TASK-041 | Create `FileSystemStorageOptions` class in `src/ForgingBlazor/Storage/FileSystem/FileSystemStorageOptions.cs` implementing `IFileSystemStorageOptions` with `BasePath` and `EnableWatch` properties                                    |           |      |
| TASK-042 | Create `FileSystemContentStorageProvider` internal class in `src/ForgingBlazor/Storage/FileSystem/FileSystemContentStorageProvider.cs` implementing `IContentStorageProvider` with async file operations using `System.IO`             |           |      |
| TASK-043 | Create `FileSystemAssetStorageProvider` internal class in `src/ForgingBlazor/Storage/FileSystem/FileSystemAssetStorageProvider.cs` implementing `IAssetStorageProvider` with async file operations                                     |           |      |
| TASK-044 | Create `FileSystemWatcherService` internal class in `src/ForgingBlazor/Storage/FileSystem/FileSystemWatcherService.cs` implementing `IHostedService` wrapping `FileSystemWatcher` for .md file changes with debouncing                 |           |      |
| TASK-045 | Create `ContentCacheService` internal class in `src/ForgingBlazor/Content/ContentCacheService.cs` implementing `IMemoryCache` wrapper with culture-aware cache keys and invalidation support                                           |           |      |
| TASK-046 | Create `ContentCacheInvalidationHandler` internal class in `src/ForgingBlazor/Content/ContentCacheInvalidationHandler.cs` subscribing to FileSystemWatcher events and invalidating affected cache entries                              |           |      |

### Phase 5: Culture Resolution and Fallback

- GOAL-005: Implement culture resolution with fallback hierarchy

| Task     | Description                                                                                                                                                                                            | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- | ---- |
| TASK-047 | Create `CultureResolver` internal class in `src/ForgingBlazor/Routing/Culture/CultureResolver.cs` with methods to parse culture from Two-Letter code, LCID, or full format (e.g., "en", 1033, "en-US") |           |      |
| TASK-048 | Create `CultureFallbackChain` internal class in `src/ForgingBlazor/Routing/Culture/CultureFallbackChain.cs` implementing fallback: `de-DE` → `de` → `en-US` (default) → `en` → no suffix               |           |      |
| TASK-049 | Create `CultureContentLocator` internal class in `src/ForgingBlazor/Content/CultureContentLocator.cs` using `CultureFallbackChain` to locate content files following lookup order defined in spec      |           |      |
| TASK-050 | Create `ContentLookupPath` record in `src/ForgingBlazor/Content/ContentLookupPath.cs` representing a content file path with culture suffix variations                                                  |           |      |
| TASK-051 | Create `CultureValidation` internal class in `src/ForgingBlazor/Routing/Culture/CultureValidation.cs` validating supported cultures at startup and throwing exception for unsupported cultures         |           |      |

### Phase 6: Route Resolution and Matching

- GOAL-006: Implement dynamic route resolution and content matching

| Task     | Description                                                                                                                                                                                               | Completed | Date |
| -------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-052 | Create `RouteDefinition` record in `src/ForgingBlazor/Routing/RouteDefinition.cs` containing: path pattern, component type, layout type, content type, pagination settings, and parent reference          |           |      |
| TASK-053 | Create `RouteRegistry` internal class in `src/ForgingBlazor/Routing/RouteRegistry.cs` as singleton storing all registered routes from FluentAPI configuration with lookup by path                         |           |      |
| TASK-054 | Create `RouteResolver` internal class in `src/ForgingBlazor/Routing/RouteResolver.cs` matching incoming request paths to `RouteDefinition` entries considering culture prefix and pagination patterns     |           |      |
| TASK-055 | Create `ContentRouteHandler` internal class in `src/ForgingBlazor/Routing/ContentRouteHandler.cs` implementing Blazor `IComponent` as dynamic router resolving content and rendering configured component |           |      |
| TASK-056 | Create `CanonicalUrlGenerator` internal class in `src/ForgingBlazor/Routing/CanonicalUrlGenerator.cs` generating canonical URL based on `CultureCanonical` setting and route definition                   |           |      |
| TASK-057 | Create `CanonicalLinkComponent` Razor component in `src/ForgingBlazor/Components/CanonicalLinkComponent.razor` rendering `<link rel="canonical" href="..." />` in HTML head section                       |           |      |

### Phase 7: Pagination System

- GOAL-007: Implement pagination with configurable URL formats

| Task     | Description                                                                                                                                                                                                                                 | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-058 | Create `PaginationSettings` record in `src/ForgingBlazor.Extensibility/Pagination/PaginationSettings.cs` with: `PageSize` (int), `UrlFormat` (enum), `Prefix` (string?, for Prefixed format)                                                |           |      |
| TASK-059 | Create `PaginatedResult<T>` record in `src/ForgingBlazor.Extensibility/Pagination/PaginatedResult.cs` with: `Items` (IReadOnlyList<T>), `CurrentPage` (int), `TotalPages` (int), `TotalItems` (int), `HasPrevious` (bool), `HasNext` (bool) |           |      |
| TASK-060 | Create `PaginationService` internal class in `src/ForgingBlazor/Pagination/PaginationService.cs` calculating page indices, validating page numbers (returning 404 for out-of-range), and generating page URLs                               |           |      |
| TASK-061 | Create `PaginationUrlParser` internal class in `src/ForgingBlazor/Pagination/PaginationUrlParser.cs` extracting page number from URL path based on configured format (Numeric: `/2`, Prefixed: `/page-2`)                                   |           |      |
| TASK-062 | Create `PaginationRouteConstraint` class in `src/ForgingBlazor/Routing/Constraints/PaginationRouteConstraint.cs` implementing `IRouteConstraint` matching pagination patterns                                                               |           |      |

### Phase 8: Publishing Workflow

- GOAL-008: Implement publishing service with confirmation workflow

| Task     | Description                                                                                                                                                                                                  | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- | ---- |
| TASK-063 | Create `PublishingService` internal class in `src/ForgingBlazor/Storage/PublishingService.cs` implementing `IPublishingService` with draft detection and publishing target sync                              |           |      |
| TASK-064 | Create `PublishingConfirmationService` internal class in `src/ForgingBlazor/Storage/PublishingConfirmationService.cs` managing user confirmation state for publishing operations                             |           |      |
| TASK-065 | Create `ContentExpirationService` internal class in `src/ForgingBlazor/Content/ContentExpirationService.cs` using `TimeProvider` to check `expiredAt` field against current time and exclude expired content |           |      |
| TASK-066 | Create `DraftContentFilter` internal class in `src/ForgingBlazor/Content/DraftContentFilter.cs` filtering content based on `draft` property and environment (development vs production)                      |           |      |

### Phase 9: Startup Validation

- GOAL-009: Implement fail-fast validation at application startup

| Task     | Description                                                                                                                                                                                                                                                            | Completed | Date |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-067 | Create `RoutingConfigurationValidation` class in `src/ForgingBlazor/Routing/Validation/RoutingConfigurationValidation.cs` implementing `IValidateOptions<RoutingConfiguration>` with validation for: at least one culture configured, default component/layout defined |           |      |
| TASK-068 | Create `ContentStructureValidation` internal class in `src/ForgingBlazor/Content/Validation/ContentStructureValidation.cs` validating: segment `_index.md` files exist, page content files exist in default culture                                                    |           |      |
| TASK-069 | Create `StorageConfigurationValidation` class in `src/ForgingBlazor/Storage/Validation/StorageConfigurationValidation.cs` implementing `IValidateOptions<StorageConfiguration>` validating: at least one storage provider configured, base paths accessible            |           |      |
| TASK-070 | Create `StartupValidationHostedService` internal class in `src/ForgingBlazor/Validation/StartupValidationHostedService.cs` implementing `IHostedService` running all validations during `StartAsync` and throwing aggregated exceptions                                |           |      |

### Phase 10: Service Registration

- GOAL-010: Register all services with dependency injection container

| Task     | Description                                                                                                                                                                                                                                        | Completed | Date |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-071 | Update `ServiceCollectionExtensions` in `src/ForgingBlazor/ServiceCollectionExtensions.cs` adding internal method `AddRoutingServices()` registering: `RouteRegistry`, `RouteResolver`, `CanonicalUrlGenerator`, `PaginationService` as singletons |           |      |
| TASK-072 | Create `AddContentServices()` internal method in `ServiceCollectionExtensions` registering: `ContentParser`, `ContentDescriptorFactory`, `ContentCacheService`, `CultureContentLocator`                                                            |           |      |
| TASK-073 | Create `AddStorageServices()` internal method in `ServiceCollectionExtensions` registering configured storage providers via factory pattern based on builder configuration                                                                         |           |      |
| TASK-074 | Create `AddValidationServices()` internal method in `ServiceCollectionExtensions` registering: `IValidateOptions<>` implementations, `StartupValidationHostedService`                                                                              |           |      |
| TASK-075 | Create `AddCultureServices()` internal method in `ServiceCollectionExtensions` registering: `CultureResolver`, `CultureFallbackChain`, `CultureValidation`                                                                                         |           |      |

### Phase 11: Router Drop-in Components

- GOAL-011: Implement drop-in `ForgingRouter` and `ForgingRouteView` compatible with default Blazor Router/RouteView and integrated with Dynamic Content Routing.

| Task     | Description                                                                                                                                                                                                                                                   | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-076 | Add component shells `src/ForgingBlazor/Components/ForgingRouteView.razor` and `src/ForgingBlazor/Components/ForgingRouteView.razor.cs` exposing public parameters/events equivalent to `Microsoft.AspNetCore.Components.RouteView`                           |           |      |
| TASK-077 | Implement `ForgingRouteView` rendering semantics: bind `RouteData`, honor layout selection consistent with `RouteView`, support NotFound content, and pass `ResolvedContent<ContentDescriptor>` when content routes are resolved (REQ-CMP-011)                |           |      |
| TASK-078 | Add component shells `src/ForgingBlazor/Components/ForgingRouter.razor` and `src/ForgingBlazor/Components/ForgingRouter.razor.cs` exposing `AppAssembly`, `AdditionalAssemblies`, `Found`, `NotFound`, `Navigating`, and `OnNavigateAsync` parity with Router |           |      |
| TASK-079 | Implement `ForgingRouter` pipeline: resolve Dynamic Content routes first using `RouteResolver`; when matched, render via `ForgingRouteView`; otherwise, fall back to default component routing with standard `Router`-compatible behavior (REQ-RTE-001..005)  |           |      |
| TASK-080 | Enforce slug constraint `[A-Za-z][A-Za-z0-9-]{1,68}[A-Za-z]` and validate cultures based on root configuration during resolution; ensure non-canonical URLs render with canonical link (no redirects) (REQ-RTE-002, REQ-CUL-001..005, REQ-PAG-005)            |           |      |
| TASK-081 | Unit tests `tests/ForgingBlazor.Tests.Unit/Components/ForgingRouteViewTests.cs`: layout selection, NotFound rendering, passing `ResolvedContent<ContentDescriptor>` parameters                                                                                |           |      |
| TASK-082 | Unit tests `tests/ForgingBlazor.Tests.Unit/Components/ForgingRouterTests.cs`: content-first resolution, fallback to component routes, slug/culture constraint enforcement, canonical link behavior                                                            |           |      |
| TASK-083 | Integration tests `tests/Xample.AppHost.Tests.Integration/Routing/ForgingRouterSmokeTests.cs`: verify `ForgingRouter` is used in sample app, resolves content routes, and falls back correctly to component routes                                            |           |      |

### Phase 12: Azure Blob Storage Package

- GOAL-012: Create separate NuGet package for Azure Blob Storage provider

| Task     | Description                                                                                                                                                                                                                      | Completed | Date |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-084 | Create new project `src/ForgingBlazor.Storage.AzureBlob/ForgingBlazor.Storage.AzureBlob.csproj` with `OutputType` Library, referencing `ForgingBlazor.Extensibility`                                                             |           |      |
| TASK-085 | Add `Azure.Storage.Blobs` package reference to `Directory.Packages.props` with latest stable version                                                                                                                             |           |      |
| TASK-086 | Add package reference to `ForgingBlazor.Storage.AzureBlob.csproj` for `Azure.Storage.Blobs` without version attribute                                                                                                            |           |      |
| TASK-087 | Create `IAzureBlobStorageOptions` interface in `src/ForgingBlazor.Storage.AzureBlob/IAzureBlobStorageOptions.cs` with methods: `WithConnectionString()`, `WithContainerName()`, `AsPublishingTarget()`                           |           |      |
| TASK-088 | Create `AzureBlobStorageOptions` class in `src/ForgingBlazor.Storage.AzureBlob/AzureBlobStorageOptions.cs` implementing `IAzureBlobStorageOptions` with `ConnectionString`, `ContainerName`, and `IsPublishingTarget` properties |           |      |
| TASK-089 | Create `AzureBlobContentStorageProvider` class in `src/ForgingBlazor.Storage.AzureBlob/AzureBlobContentStorageProvider.cs` implementing `IContentStorageProvider` using `BlobContainerClient` for async blob operations          |           |      |
| TASK-090 | Create `AzureBlobAssetStorageProvider` class in `src/ForgingBlazor.Storage.AzureBlob/AzureBlobAssetStorageProvider.cs` implementing `IAssetStorageProvider` using `BlobContainerClient`                                          |           |      |
| TASK-091 | Create `ContentStorageBuilderExtensions` static class in `src/ForgingBlazor.Storage.AzureBlob/ContentStorageBuilderExtensions.cs` with `UseAzureBlobStorage()` extension method on `IContentStorageBuilder`                      |           |      |
| TASK-092 | Create `AssetStorageBuilderExtensions` static class in `src/ForgingBlazor.Storage.AzureBlob/AssetStorageBuilderExtensions.cs` with `UseAzureBlobStorage()` extension method on `IAssetStorageBuilder`                            |           |      |

### Phase 13: Unit Tests - Core Interfaces

- GOAL-013: Implement unit tests for core interfaces and abstractions

| Task     | Description                                                                                                                                                                    | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- | ---- |
| TASK-093 | Create test file `tests/ForgingBlazor.Extensibility.Tests.Unit/Content/ContentDescriptorTests.cs` testing: property initialization, required field validation, default values  |           |      |
| TASK-094 | Create test file `tests/ForgingBlazor.Extensibility.Tests.Unit/Content/ResolvedContentTests.cs` testing: wrapping descriptor, culture info, canonical URL                      |           |      |
| TASK-095 | Create test file `tests/ForgingBlazor.Extensibility.Tests.Unit/CheckSlugTests.cs` testing `IsValidSlug()` and `ValidateSlug()` methods with edge cases from spec section 9.5.3 |           |      |

### Phase 14: Unit Tests - Routing

- GOAL-014: Implement unit tests for routing configuration and resolution

| Task     | Description                                                                                                                                                                                                          | Completed | Date |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-096 | Create test file `tests/ForgingBlazor.Tests.Unit/Routing/RoutingBuilderTests.cs` testing: FluentAPI chain methods, configuration accumulation, nested segment support                                                |           |      |
| TASK-097 | Create test file `tests/ForgingBlazor.Tests.Unit/Routing/SlugRouteConstraintTests.cs` testing: valid slug patterns, invalid patterns, edge cases (length limits, consecutive hyphens, leading/trailing restrictions) |           |      |
| TASK-098 | Create test file `tests/ForgingBlazor.Tests.Unit/Routing/RouteResolverTests.cs` testing: path matching, culture prefix handling, pagination pattern matching                                                         |           |      |
| TASK-099 | Create test file `tests/ForgingBlazor.Tests.Unit/Routing/CanonicalUrlGeneratorTests.cs` testing: WithPrefix vs WithoutPrefix generation, culture variations                                                          |           |      |

### Phase 15: Unit Tests - Content Parsing

- GOAL-015: Implement unit tests for content parsing with 100% validation coverage

| Task     | Description                                                                                                                                                               | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-100 | Create test file `tests/ForgingBlazor.Tests.Unit/Content/FrontmatterParserTests.cs` testing: valid YAML extraction, missing delimiters, empty frontmatter, malformed YAML |           |      |
| TASK-101 | Create test file `tests/ForgingBlazor.Tests.Unit/Content/MarkdownRendererTests.cs` testing: basic Markdown, code blocks, tables, task lists, edge cases                   |           |      |
| TASK-102 | Create test file `tests/ForgingBlazor.Tests.Unit/Content/ContentParserTests.cs` testing: full parsing pipeline, descriptor instantiation, custom metadata mapping         |           |      |
| TASK-103 | Create test file `tests/ForgingBlazor.Tests.Unit/Content/FrontmatterValidationTests.cs` testing: required field validation (100% coverage), slug validation, date parsing |           |      |

### Phase 16: Unit Tests - Culture

- GOAL-016: Implement unit tests for culture resolution with 100% fallback coverage

| Task     | Description                                                                                                                                                                                             | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-096 | Create test file `tests/ForgingBlazor.Tests.Unit/Routing/Culture/CultureResolverTests.cs` testing: Two-Letter code parsing, LCID parsing, full format parsing, invalid inputs                           |           |      |
| TASK-097 | Create test file `tests/ForgingBlazor.Tests.Unit/Routing/Culture/CultureFallbackChainTests.cs` testing: complete fallback hierarchy (de-DE → de → en-US → en → none), all documented fallback scenarios |           |      |
| TASK-098 | Create test file `tests/ForgingBlazor.Tests.Unit/Content/CultureContentLocatorTests.cs` testing: content file lookup order from spec section 4.5, missing file handling                                 |           |      |

### Phase 17: Unit Tests - Pagination

- GOAL-017: Implement unit tests for pagination system

| Task     | Description                                                                                                                                                                 | Completed | Date |
| -------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-107 | Create test file `tests/ForgingBlazor.Tests.Unit/Pagination/PaginationServiceTests.cs` testing: page calculation, out-of-range handling, edge cases from spec section 9.5.2 |           |      |
| TASK-108 | Create test file `tests/ForgingBlazor.Tests.Unit/Pagination/PaginationUrlParserTests.cs` testing: Numeric format parsing, Prefixed format parsing, invalid inputs           |           |      |

### Phase 18: Unit Tests - Storage

- GOAL-018: Implement unit tests for storage providers

| Task     | Description                                                                                                                                                     | Completed | Date |
| -------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-109 | Create test file `tests/ForgingBlazor.Tests.Unit/Storage/FileSystemContentStorageProviderTests.cs` using temporary directories for file operations testing      |           |      |
| TASK-110 | Create test file `tests/ForgingBlazor.Tests.Unit/Storage/FileSystemWatcherServiceTests.cs` testing: file change detection, debouncing behavior, filter patterns |           |      |
| TASK-111 | Create test file `tests/ForgingBlazor.Tests.Unit/Storage/ContentCacheServiceTests.cs` testing: cache hit/miss, culture-aware keys, invalidation                 |           |      |

### Phase 19: Unit Tests - Validation

- GOAL-019: Implement unit tests for startup validation with 100% coverage

| Task     | Description                                                                                                                                                                   | Completed | Date |
| -------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-112 | Create test file `tests/ForgingBlazor.Tests.Unit/Routing/Validation/RoutingConfigurationValidationTests.cs` testing: all validation rules, error messages                     |           |      |
| TASK-113 | Create test file `tests/ForgingBlazor.Tests.Unit/Content/Validation/ContentStructureValidationTests.cs` testing: missing \_index.md detection, missing page content detection |           |      |
| TASK-114 | Create test file `tests/ForgingBlazor.Tests.Unit/Storage/Validation/StorageConfigurationValidationTests.cs` testing: provider configuration validation, path accessibility    |           |      |

### Phase 20: Integration Tests

- GOAL-020: Implement integration tests for end-to-end scenarios

| Task     | Description                                                                                                                                                                      | Completed | Date |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-115 | Create test project `tests/ForgingBlazor.Storage.AzureBlob.Tests.Unit/ForgingBlazor.Storage.AzureBlob.Tests.Unit.csproj` referencing `ForgingBlazor.Storage.AzureBlob`           |           |      |
| TASK-116 | Create test file `tests/ForgingBlazor.Storage.AzureBlob.Tests.Unit/AzureBlobContentStorageProviderTests.cs` using Azurite emulator for blob operations                           |           |      |
| TASK-117 | Create test file `tests/ForgingBlazor.Tests.Integration/Routing/ContentRoutingIntegrationTests.cs` testing: full request/response cycle, content resolution, component rendering |           |      |
| TASK-118 | Create test file `tests/ForgingBlazor.Tests.Integration/Storage/PublishingWorkflowIntegrationTests.cs` testing: draft → publish workflow, expiration handling                    |           |      |
| TASK-119 | Create test fixture `tests/ForgingBlazor.Tests.Integration/Fixtures/TestContentFixture.cs` providing sample Markdown content files for integration tests                         |           |      |

### Phase 21: Documentation and Examples

- GOAL-021: Create documentation and usage examples

| Task     | Description                                                                                                                                                       | Completed | Date |
| -------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-120 | Create README file `src/ForgingBlazor.Storage.AzureBlob/README.md` following template from `templates/readme-project.md` with installation and usage instructions |           |      |
| TASK-121 | Update `src/ForgingBlazor/README.md` adding section for Dynamic Content Routing with FluentAPI examples                                                           |           |      |

### Phase 22: Xample Demonstration Application

- GOAL-022: Build comprehensive demonstration application in Xample project showcasing all features with edge cases and pitfalls

| Task     | Description                                                                                                                                                                                                                                       | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-130 | Update `src/Xample/Program.cs` with complete FluentAPI configuration demonstrating: `AddRouting()` with `ConfigureRoot()`, multiple cultures (en-US, de-DE, fr-FR), `CultureCanonical.WithoutPrefix`, default component/layout, home page mapping |           |      |
| TASK-131 | Add storage configuration to `src/Xample/Program.cs` with `AddContentStorage()` using FileSystem provider with `WatchForChanges()` enabled for development hot-reload                                                                             |           |      |
| TASK-132 | Add asset storage configuration to `src/Xample/Program.cs` with `AddAssetStorage()` using FileSystem provider with separate base path for images/media                                                                                            |           |      |
| TASK-133 | Create `BlogPostDescriptor` class in `src/Xample/Content/BlogPostDescriptor.cs` extending `ContentDescriptor` with custom properties: `Author`, `Tags`, `ReadingTimeMinutes`, `FeaturedImage`                                                     |           |      |
| TASK-134 | Create `TutorialDescriptor` class in `src/Xample/Content/TutorialDescriptor.cs` extending `ContentDescriptor` with: `Difficulty` (enum), `Prerequisites` (string[]), `EstimatedDuration`                                                          |           |      |
| TASK-135 | Configure segment mapping for `/posts` in `src/Xample/Program.cs` with: `BlogPostDescriptor` content type, pagination (PageSize: 5, Prefixed format with "page-"), custom index/page components                                                   |           |      |
| TASK-136 | Configure nested segment mapping for `/posts/tutorials` in `src/Xample/Program.cs` with: `TutorialDescriptor` content type, pagination (PageSize: 3, Numeric format), separate components                                                         |           |      |
| TASK-137 | Configure segment mapping for `/blog` in `src/Xample/Program.cs` demonstrating alternative segment with different pagination settings (PageSize: 10, Numeric format)                                                                              |           |      |
| TASK-138 | Configure page mapping for `/about` in `src/Xample/Program.cs` as standalone page with custom component and layout override                                                                                                                       |           |      |
| TASK-139 | Configure page mapping for `/contact` in `src/Xample/Program.cs` as folder-style page (`contact/index.md`) with form layout                                                                                                                       |           |      |
| TASK-140 | Configure page mapping for `/legal/privacy` in `src/Xample/Program.cs` demonstrating nested standalone page without pagination                                                                                                                    |           |      |
| TASK-141 | Create `PostIndexComponent.razor` in `src/Xample/Components/Content/PostIndexComponent.razor` displaying paginated post list with `PaginatedResult<BlogPostDescriptor>` parameter                                                                 |           |      |
| TASK-142 | Create `PostDetailComponent.razor` in `src/Xample/Components/Content/PostDetailComponent.razor` displaying single post with `ResolvedContent<BlogPostDescriptor>` parameter, including canonical link rendering                                   |           |      |
| TASK-143 | Create `TutorialIndexComponent.razor` in `src/Xample/Components/Content/TutorialIndexComponent.razor` displaying tutorial list with difficulty badges and prerequisites                                                                           |           |      |
| TASK-144 | Create `TutorialDetailComponent.razor` in `src/Xample/Components/Content/TutorialDetailComponent.razor` displaying tutorial with estimated duration and difficulty indicator                                                                      |           |      |
| TASK-145 | Create `PageComponent.razor` in `src/Xample/Components/Content/PageComponent.razor` as generic page component for standalone pages with `ResolvedContent<ContentDescriptor>`                                                                      |           |      |
| TASK-146 | Create `ContentLayout.razor` in `src/Xample/Components/Layout/ContentLayout.razor` as default layout for content pages with sidebar navigation and breadcrumbs                                                                                    |           |      |

### Phase 23: Xample Content Files - Standard Cases

- GOAL-023: Create standard content files demonstrating proper usage patterns

| Task     | Description                                                                                                                                                                              | Completed | Date |
| -------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-147 | Create `src/Xample/content/_index.md` as home page with full frontmatter (title, slug, publishedDate) in default culture (en-US)                                                         |           |      |
| TASK-148 | Create `src/Xample/content/_index.de-DE.md` as German translation of home page with identical slug                                                                                       |           |      |
| TASK-149 | Create `src/Xample/content/_index.fr-FR.md` as French translation of home page with identical slug                                                                                       |           |      |
| TASK-150 | Create `src/Xample/content/about.md` as standalone about page with author bio and contact information                                                                                    |           |      |
| TASK-151 | Create `src/Xample/content/about.de-DE.md` as German translation of about page                                                                                                           |           |      |
| TASK-152 | Create `src/Xample/content/contact/index.md` as folder-style contact page demonstrating alternative file organization                                                                    |           |      |
| TASK-153 | Create `src/Xample/content/legal/privacy.md` as nested standalone page for privacy policy                                                                                                |           |      |
| TASK-154 | Create `src/Xample/content/posts/_index.md` as posts segment index page with segment description                                                                                         |           |      |
| TASK-155 | Create `src/Xample/content/posts/_index.de-DE.md` as German posts segment index                                                                                                          |           |      |
| TASK-156 | Create `src/Xample/content/posts/getting-started.md` (existing folder) with complete `BlogPostDescriptor` frontmatter including custom fields (author, tags, readingTimeMinutes)         |           |      |
| TASK-157 | Create `src/Xample/content/posts/getting-started.de-DE.md` as German translation with localized content                                                                                  |           |      |
| TASK-158 | Create `src/Xample/content/posts/multi-language-support.md` (existing folder) demonstrating multi-culture content workflow                                                               |           |      |
| TASK-159 | Create 10 additional posts in `src/Xample/content/posts/` to demonstrate pagination across multiple pages (post-three through post-twelve)                                               |           |      |
| TASK-160 | Create `src/Xample/content/posts/tutorials/_index.md` as nested segment index for tutorials                                                                                              |           |      |
| TASK-161 | Create `src/Xample/content/posts/tutorials/beginner-guide.md` with `TutorialDescriptor` frontmatter (difficulty: Beginner, prerequisites: [], estimatedDuration: 30)                     |           |      |
| TASK-162 | Create `src/Xample/content/posts/tutorials/advanced-patterns.md` with `TutorialDescriptor` frontmatter (difficulty: Advanced, prerequisites: ["beginner-guide"], estimatedDuration: 120) |           |      |
| TASK-163 | Create `src/Xample/content/blog/_index.md` as alternative blog segment demonstrating multiple segments with different configurations                                                     |           |      |

### Phase 24: Xample Content Files - Edge Cases and Pitfalls

- GOAL-024: Create content files demonstrating edge cases, boundary conditions, and potential pitfalls

| Task     | Description                                                                                                                                                                            | Completed | Date |
| -------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | ---- |
| TASK-164 | Create `src/Xample/content/posts/minimum-slug.md` with shortest valid slug (3 characters: "abc") testing REQ-RTE-002 minimum boundary                                                  |           |      |
| TASK-165 | Create `src/Xample/content/posts/maximum-length-slug-that-is-exactly-seventy-characters-long-boundary.md` with maximum valid slug (70 characters) testing REQ-RTE-002 maximum boundary |           |      |
| TASK-166 | Create `src/Xample/content/posts/mixed-case-slug-test.md` with `slug: MiXeD-CaSe-SlUg-TeSt` demonstrating case handling in slugs                                                       |           |      |
| TASK-167 | Create `src/Xample/content/posts/numbers-in-middle-123-test.md` with `slug: numbers-in-middle-123-test` demonstrating numbers in slug middle (valid)                                   |           |      |
| TASK-168 | Create `src/Xample/content/posts/special-post-draft.md` with `draft: true` demonstrating draft content filtering (should not appear in production)                                     |           |      |
| TASK-169 | Create `src/Xample/content/posts/expired-content-example.md` with `expiredAt: 2025-01-01T00:00:00+00:00` demonstrating soft-deleted expired content                                    |           |      |
| TASK-170 | Create `src/Xample/content/posts/future-publish-date.md` with `publishedDate` set to future date (2027-01-01) demonstrating scheduled content handling                                 |           |      |
| TASK-171 | Create `src/Xample/content/posts/timezone-edge-case.md` with `publishedDate: 2026-01-25T23:59:59-12:00` demonstrating extreme timezone handling                                        |           |      |
| TASK-172 | Create `src/Xample/content/posts/unicode-content-test.md` with content containing: emoji (🚀), Chinese characters (你好), Arabic text (مرحبا), special symbols (™©®)                   |           |      |
| TASK-173 | Create `src/Xample/content/posts/markdown-edge-cases.md` with complex Markdown: nested code blocks, tables with special characters, HTML entities, task lists, footnotes               |           |      |
| TASK-174 | Create `src/Xample/content/posts/yaml-special-characters.md` with frontmatter containing: colons in strings, quotes, multiline values, special YAML characters that need escaping      |           |      |
| TASK-175 | Create `src/Xample/content/posts/empty-optional-fields.md` with explicitly null optional fields (`expiredAt: null`, `draft: false`) and empty tags array                               |           |      |
| TASK-176 | Create `src/Xample/content/posts/whitespace-handling.md` with content containing: leading/trailing whitespace, multiple blank lines, tabs vs spaces                                    |           |      |
| TASK-177 | Create `src/Xample/content/fallback-only-default.md` as page existing only in default culture to test culture fallback (no translations)                                               |           |      |
| TASK-178 | Create `src/Xample/content/posts/partial-translation.md` with only `en-US` and `de-DE` translations, no `fr-FR` - testing partial translation fallback                                 |           |      |
| TASK-179 | Create test content demonstrating culture fallback chain: file exists as `.de.md` (two-letter) but not `.de-DE.md` (full culture)                                                      |           |      |

### Phase 25: Xample AppHost Integration Tests

- GOAL-025: Create comprehensive integration tests in Xample.AppHost.Tests.Integration verifying all routing, content, and edge case scenarios

| Task     | Description                                                                                                                                                                                                                          | Completed | Date |
| -------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------- | ---- |
| TASK-180 | Create `tests/Xample.AppHost.Tests.Integration/Routing/HomePageRoutingTests.cs` testing: `/` returns home page, `/en-US` returns home with canonical to `/`, `/de-DE` returns German home page                                       |           |      |
| TASK-181 | Create `tests/Xample.AppHost.Tests.Integration/Routing/CultureRoutingTests.cs` testing: all supported cultures return content, unsupported culture `/es-ES` returns 404, culture fallback from `de-DE` → `de` → `en-US`              |           |      |
| TASK-182 | Create `tests/Xample.AppHost.Tests.Integration/Routing/SegmentRoutingTests.cs` testing: `/posts` returns paginated index, `/posts/getting-started` returns post detail, nested `/posts/tutorials/beginner-guide` works               |           |      |
| TASK-183 | Create `tests/Xample.AppHost.Tests.Integration/Routing/PageRoutingTests.cs` testing: `/about` returns standalone page, `/contact` resolves folder-style page, `/legal/privacy` resolves nested page                                  |           |      |
| TASK-184 | Create `tests/Xample.AppHost.Tests.Integration/Routing/CanonicalUrlTests.cs` testing: canonical link present in HTML head, non-canonical URLs (e.g., `/en-US`) include canonical link to `/`, no HTTP redirects occur                |           |      |
| TASK-185 | Create `tests/Xample.AppHost.Tests.Integration/Pagination/PaginationRoutingTests.cs` testing: `/posts` shows page 1, `/posts/page-2` shows page 2, `/posts/page-1` canonical to `/posts`, out-of-range `/posts/page-999` returns 404 |           |      |
| TASK-186 | Create `tests/Xample.AppHost.Tests.Integration/Pagination/NumericPaginationTests.cs` testing: `/posts/tutorials` with Numeric format, `/posts/tutorials/2` works, `/posts/tutorials/1` canonical to `/posts/tutorials`               |           |      |
| TASK-187 | Create `tests/Xample.AppHost.Tests.Integration/Pagination/PaginationBoundaryTests.cs` testing: exactly PageSize items shows single page, PageSize+1 items creates page 2, empty segment shows no results message                     |           |      |
| TASK-188 | Create `tests/Xample.AppHost.Tests.Integration/Content/ContentResolutionTests.cs` testing: frontmatter correctly parsed, custom descriptor fields populated, Markdown body rendered to HTML                                          |           |      |
| TASK-189 | Create `tests/Xample.AppHost.Tests.Integration/Content/DraftContentTests.cs` testing: `draft: true` content returns 404 in production mode, draft content accessible in development mode if configured                               |           |      |
| TASK-190 | Create `tests/Xample.AppHost.Tests.Integration/Content/ExpiredContentTests.cs` testing: content with past `expiredAt` returns 404, content with future `expiredAt` renders normally, null `expiredAt` never expires                  |           |      |
| TASK-191 | Create `tests/Xample.AppHost.Tests.Integration/Content/FutureDateContentTests.cs` testing: content with future `publishedDate` behavior (may be hidden or shown based on configuration)                                              |           |      |
| TASK-192 | Create `tests/Xample.AppHost.Tests.Integration/Slug/SlugValidationTests.cs` testing: valid slugs resolve correctly, requests for invalid slug patterns return 404, slug case sensitivity handling                                    |           |      |
| TASK-193 | Create `tests/Xample.AppHost.Tests.Integration/Slug/SlugBoundaryTests.cs` testing: minimum slug length (3 chars) works, maximum slug length (70 chars) works, 2-char slug returns 404, 71-char slug returns 404                      |           |      |
| TASK-194 | Create `tests/Xample.AppHost.Tests.Integration/Culture/CultureFallbackTests.cs` testing: full fallback chain `de-DE` → `de` → `en-US` → `en`, partial translation fallback, two-letter culture file resolution                       |           |      |
| TASK-195 | Create `tests/Xample.AppHost.Tests.Integration/Culture/CultureNegotiationTests.cs` testing: Accept-Language header handling, explicit culture in URL overrides header, default culture with/without prefix                           |           |      |
| TASK-196 | Create `tests/Xample.AppHost.Tests.Integration/EdgeCases/UnicodeContentTests.cs` testing: emoji rendering, CJK character support, RTL text handling, special symbols in content                                                      |           |      |
| TASK-197 | Create `tests/Xample.AppHost.Tests.Integration/EdgeCases/MarkdownRenderingTests.cs` testing: code blocks syntax highlighted, tables rendered correctly, task lists functional, nested lists proper indentation                       |           |      |
| TASK-198 | Create `tests/Xample.AppHost.Tests.Integration/EdgeCases/YamlParsingTests.cs` testing: special characters in frontmatter, multiline values, complex nested structures, array fields                                                  |           |      |
| TASK-199 | Create `tests/Xample.AppHost.Tests.Integration/EdgeCases/WhitespaceHandlingTests.cs` testing: content with various whitespace preserved correctly, frontmatter whitespace trimmed appropriately                                      |           |      |
| TASK-200 | Create `tests/Xample.AppHost.Tests.Integration/EdgeCases/TimezoneTests.cs` testing: DateTimeOffset parsing across timezones, expiration calculation with different timezones, UTC normalization                                      |           |      |
| TASK-201 | Create `tests/Xample.AppHost.Tests.Integration/Security/PathTraversalTests.cs` testing: `../` in URL rejected, encoded path traversal (`%2e%2e%2f`) rejected, absolute paths rejected                                                |           |      |
| TASK-202 | Create `tests/Xample.AppHost.Tests.Integration/Performance/CachingTests.cs` testing: second request served from cache, cache invalidation on file change (if watcher enabled), cache key includes culture                            |           |      |
| TASK-203 | Create `tests/Xample.AppHost.Tests.Integration/Startup/ValidationTests.cs` testing: startup fails for missing `_index.md` in segment, startup fails for missing default culture content, startup fails for invalid configuration     |           |      |
| TASK-204 | Create test fixture `tests/Xample.AppHost.Tests.Integration/Fixtures/XampleAppFixture.cs` using Aspire testing to spin up Xample application with test configuration                                                                 |           |      |
| TASK-205 | Create test data helper `tests/Xample.AppHost.Tests.Integration/Helpers/ContentTestHelpers.cs` with methods to create/modify/delete test content files during integration tests                                                      |           |      |
| TASK-206 | Create test assertions helper `tests/Xample.AppHost.Tests.Integration/Helpers/HtmlAssertions.cs` with custom TUnit assertions for: canonical link presence, meta tags, content structure                                             |           |      |

## 3. Alternatives

- **ALT-001**: Use source generators for content descriptor mapping instead of runtime reflection. Rejected due to added complexity and build-time dependency on content structure.
- **ALT-002**: Use SQLite as local draft storage instead of FileSystem. Rejected because FileSystem provides simpler content authoring workflow with standard editors.
- **ALT-003**: Use URL redirects instead of canonical links for non-canonical URLs. Rejected per spec requirement to preserve user-facing URLs and reduce server load.
- **ALT-004**: Allow culture override at segment/page level. Rejected per spec constraint CON-001 to ensure consistent culture handling.
- **ALT-005**: Use database storage for content instead of Markdown files. Out of scope per spec section 1.3.

## 4. Dependencies

### 4.1 External Dependencies

- **DEP-001**: YamlDotNet (NuGet) - YAML frontmatter parsing, required for content processing
- **DEP-002**: Markdig (NuGet) - Markdown to HTML conversion, required for content rendering
- **DEP-003**: Azure.Storage.Blobs (NuGet) - Azure Blob Storage client, required for `ForgingBlazor.Storage.AzureBlob` package

### 4.2 Internal Dependencies

- **DEP-004**: `ForgingBlazor.Extensibility` - All public interfaces MUST be defined here before implementation
- **DEP-005**: Existing `IForgingBlazorApplicationBuilder` interface - Extension methods attach to this interface
- **DEP-006**: Existing `Check` class - Slug validation methods extend this utility class
- **DEP-007**: Existing `Defaults` class - Default values for pagination and storage paths

### 4.3 Infrastructure Dependencies

- **DEP-008**: File System - Local storage for draft content and development
- **DEP-009**: Azure Storage Account - Required for Azure Blob Storage publishing target (optional)
- **DEP-010**: Azurite emulator - Required for Azure Blob Storage integration tests

## 5. Files

### 5.1 New Files in ForgingBlazor.Extensibility

- **FILE-001**: `src/ForgingBlazor.Extensibility/Content/ContentDescriptor.cs` - Base class for all content descriptors
- **FILE-002**: `src/ForgingBlazor.Extensibility/Content/ResolvedContent.cs` - Wrapper for resolved content with metadata
- **FILE-003**: `src/ForgingBlazor.Extensibility/Content/IMetadataConfiguration.cs` - Custom metadata configuration interface
- **FILE-004**: `src/ForgingBlazor.Extensibility/Content/ContentValidationException.cs` - Content validation exception
- **FILE-005**: `src/ForgingBlazor.Extensibility/Routing/CultureCanonical.cs` - Culture canonical URL format enum
- **FILE-006**: `src/ForgingBlazor.Extensibility/Routing/IRoutingBuilder.cs` - Main routing builder interface
- **FILE-007**: `src/ForgingBlazor.Extensibility/Routing/IRootConfiguration.cs` - Root configuration interface
- **FILE-008**: `src/ForgingBlazor.Extensibility/Routing/ICultureConfiguration.cs` - Culture configuration interface
- **FILE-009**: `src/ForgingBlazor.Extensibility/Routing/ISegmentConfiguration.cs` - Segment configuration interface
- **FILE-010**: `src/ForgingBlazor.Extensibility/Routing/IPageConfiguration.cs` - Page configuration interface
- **FILE-011**: `src/ForgingBlazor.Extensibility/Pagination/PaginationUrlFormat.cs` - Pagination URL format enum
- **FILE-012**: `src/ForgingBlazor.Extensibility/Pagination/IPaginationConfiguration.cs` - Pagination configuration interface
- **FILE-013**: `src/ForgingBlazor.Extensibility/Pagination/PaginationSettings.cs` - Pagination settings record
- **FILE-014**: `src/ForgingBlazor.Extensibility/Pagination/PaginatedResult.cs` - Paginated result record
- **FILE-015**: `src/ForgingBlazor.Extensibility/Storage/IContentStorageProvider.cs` - Content storage provider interface
- **FILE-016**: `src/ForgingBlazor.Extensibility/Storage/IAssetStorageProvider.cs` - Asset storage provider interface
- **FILE-017**: `src/ForgingBlazor.Extensibility/Storage/IContentStorageBuilder.cs` - Content storage builder interface
- **FILE-018**: `src/ForgingBlazor.Extensibility/Storage/IAssetStorageBuilder.cs` - Asset storage builder interface
- **FILE-019**: `src/ForgingBlazor.Extensibility/Storage/IFileSystemStorageOptions.cs` - FileSystem storage options interface
- **FILE-020**: `src/ForgingBlazor.Extensibility/Storage/IPublishingService.cs` - Publishing service interface

### 5.2 New Files in ForgingBlazor

- **FILE-021**: `src/ForgingBlazor/Routing/RoutingBuilderExtensions.cs` - `AddRouting()` extension method
- **FILE-022**: `src/ForgingBlazor/Routing/RoutingBuilder.cs` - Internal routing builder implementation
- **FILE-023**: `src/ForgingBlazor/Routing/Configurations/RootConfiguration.cs` - Root configuration implementation
- **FILE-024**: `src/ForgingBlazor/Routing/Configurations/CultureConfiguration.cs` - Culture configuration implementation
- **FILE-025**: `src/ForgingBlazor/Routing/Configurations/SegmentConfiguration.cs` - Segment configuration implementation
- **FILE-026**: `src/ForgingBlazor/Routing/Configurations/PageConfiguration.cs` - Page configuration implementation
- **FILE-027**: `src/ForgingBlazor/Routing/Configurations/PaginationConfiguration.cs` - Pagination configuration implementation
- **FILE-028**: `src/ForgingBlazor/Routing/Configurations/MetadataConfiguration.cs` - Metadata configuration implementation
- **FILE-029**: `src/ForgingBlazor/Routing/Constraints/SlugRouteConstraint.cs` - Slug route constraint
- **FILE-030**: `src/ForgingBlazor/Routing/Constraints/PaginationRouteConstraint.cs` - Pagination route constraint
- **FILE-031**: `src/ForgingBlazor/Routing/Culture/CultureResolver.cs` - Culture parsing and resolution
- **FILE-032**: `src/ForgingBlazor/Routing/Culture/CultureFallbackChain.cs` - Culture fallback hierarchy
- **FILE-033**: `src/ForgingBlazor/Routing/Culture/CultureValidation.cs` - Culture validation at startup
- **FILE-034**: `src/ForgingBlazor/Routing/RouteDefinition.cs` - Route definition record
- **FILE-035**: `src/ForgingBlazor/Routing/RouteRegistry.cs` - Route registry singleton
- **FILE-036**: `src/ForgingBlazor/Routing/RouteResolver.cs` - Route resolver implementation
- **FILE-037**: `src/ForgingBlazor/Routing/ContentRouteHandler.cs` - Dynamic content router component
- **FILE-038**: `src/ForgingBlazor/Routing/CanonicalUrlGenerator.cs` - Canonical URL generation
- **FILE-039**: `src/ForgingBlazor/Routing/Validation/RoutingConfigurationValidation.cs` - Routing config validation
- **FILE-040**: `src/ForgingBlazor/Content/Parsing/FrontmatterParser.cs` - YAML frontmatter parser
- **FILE-041**: `src/ForgingBlazor/Content/Parsing/MarkdownRenderer.cs` - Markdown to HTML renderer
- **FILE-042**: `src/ForgingBlazor/Content/Parsing/ContentParser.cs` - Content parsing orchestrator
- **FILE-043**: `src/ForgingBlazor/Content/ContentDescriptorFactory.cs` - Content descriptor instantiation
- **FILE-044**: `src/ForgingBlazor/Content/ContentCacheService.cs` - Content caching service
- **FILE-045**: `src/ForgingBlazor/Content/ContentCacheInvalidationHandler.cs` - Cache invalidation handler
- **FILE-046**: `src/ForgingBlazor/Content/CultureContentLocator.cs` - Culture-aware content locator
- **FILE-047**: `src/ForgingBlazor/Content/ContentLookupPath.cs` - Content lookup path record
- **FILE-048**: `src/ForgingBlazor/Content/ContentExpirationService.cs` - Content expiration checking
- **FILE-049**: `src/ForgingBlazor/Content/DraftContentFilter.cs` - Draft content filtering
- **FILE-050**: `src/ForgingBlazor/Content/Validation/FrontmatterValidation.cs` - Frontmatter validation
- **FILE-051**: `src/ForgingBlazor/Content/Validation/ContentStructureValidation.cs` - Content structure validation
- **FILE-052**: `src/ForgingBlazor/Storage/ContentStorageExtensions.cs` - `AddContentStorage()` extension
- **FILE-053**: `src/ForgingBlazor/Storage/AssetStorageExtensions.cs` - `AddAssetStorage()` extension
- **FILE-054**: `src/ForgingBlazor/Storage/ContentStorageBuilder.cs` - Content storage builder
- **FILE-055**: `src/ForgingBlazor/Storage/AssetStorageBuilder.cs` - Asset storage builder
- **FILE-056**: `src/ForgingBlazor/Storage/FileSystem/FileSystemStorageOptions.cs` - FileSystem options
- **FILE-057**: `src/ForgingBlazor/Storage/FileSystem/FileSystemContentStorageProvider.cs` - FileSystem content provider
- **FILE-058**: `src/ForgingBlazor/Storage/FileSystem/FileSystemAssetStorageProvider.cs` - FileSystem asset provider
- **FILE-059**: `src/ForgingBlazor/Storage/FileSystem/FileSystemWatcherService.cs` - FileSystem watcher service
- **FILE-060**: `src/ForgingBlazor/Storage/PublishingService.cs` - Publishing service implementation
- **FILE-061**: `src/ForgingBlazor/Storage/PublishingConfirmationService.cs` - Publishing confirmation
- **FILE-062**: `src/ForgingBlazor/Storage/Validation/StorageConfigurationValidation.cs` - Storage config validation
- **FILE-063**: `src/ForgingBlazor/Pagination/PaginationService.cs` - Pagination service
- **FILE-064**: `src/ForgingBlazor/Pagination/PaginationUrlParser.cs` - Pagination URL parser
- **FILE-065**: `src/ForgingBlazor/Validation/StartupValidationHostedService.cs` - Startup validation hosted service
- **FILE-066**: `src/ForgingBlazor/Components/CanonicalLinkComponent.razor` - Canonical link Blazor component
- **FILE-067**: `src/ForgingBlazor/Components/ForgingRouter.razor` - Drop-in router component shell
- **FILE-068**: `src/ForgingBlazor/Components/ForgingRouter.razor.cs` - Drop-in router logic integrating Dynamic Content Routing
- **FILE-069**: `src/ForgingBlazor/Components/ForgingRouteView.razor` - Drop-in route view component shell
- **FILE-070**: `src/ForgingBlazor/Components/ForgingRouteView.razor.cs` - Route view rendering with layout and NotFound semantics

### 5.3 New Files in ForgingBlazor.Storage.AzureBlob (New Project)

- **FILE-067**: `src/ForgingBlazor.Storage.AzureBlob/ForgingBlazor.Storage.AzureBlob.csproj` - Project file
- **FILE-068**: `src/ForgingBlazor.Storage.AzureBlob/IAzureBlobStorageOptions.cs` - Azure Blob options interface
- **FILE-069**: `src/ForgingBlazor.Storage.AzureBlob/AzureBlobStorageOptions.cs` - Azure Blob options implementation
- **FILE-070**: `src/ForgingBlazor.Storage.AzureBlob/AzureBlobContentStorageProvider.cs` - Azure Blob content provider
- **FILE-071**: `src/ForgingBlazor.Storage.AzureBlob/AzureBlobAssetStorageProvider.cs` - Azure Blob asset provider
- **FILE-072**: `src/ForgingBlazor.Storage.AzureBlob/ContentStorageBuilderExtensions.cs` - Content storage extensions
- **FILE-073**: `src/ForgingBlazor.Storage.AzureBlob/AssetStorageBuilderExtensions.cs` - Asset storage extensions
- **FILE-074**: `src/ForgingBlazor.Storage.AzureBlob/README.md` - Package documentation

### 5.4 Modified Files

- **FILE-075**: `Directory.Packages.props` - Add YamlDotNet, Markdig, Azure.Storage.Blobs package versions
- **FILE-076**: `src/ForgingBlazor/ForgingBlazor.csproj` - Add YamlDotNet, Markdig package references
- **FILE-077**: `src/ForgingBlazor/ServiceCollectionExtensions.cs` - Add routing, content, storage service registration
- **FILE-078**: `src/ForgingBlazor.Extensibility/Check.cs` - Add `IsValidSlug()` and `ValidateSlug()` methods
- **FILE-079**: `ForgingBlazor.slnx` - Add `ForgingBlazor.Storage.AzureBlob` project and test projects

### 5.5 New Test Files

- **FILE-080**: `tests/ForgingBlazor.Extensibility.Tests.Unit/Content/ContentDescriptorTests.cs`
- **FILE-081**: `tests/ForgingBlazor.Extensibility.Tests.Unit/Content/ResolvedContentTests.cs`
- **FILE-082**: `tests/ForgingBlazor.Extensibility.Tests.Unit/CheckSlugTests.cs`
- **FILE-083**: `tests/ForgingBlazor.Tests.Unit/Routing/RoutingBuilderTests.cs`
- **FILE-084**: `tests/ForgingBlazor.Tests.Unit/Routing/SlugRouteConstraintTests.cs`
- **FILE-085**: `tests/ForgingBlazor.Tests.Unit/Routing/RouteResolverTests.cs`
- **FILE-086**: `tests/ForgingBlazor.Tests.Unit/Routing/CanonicalUrlGeneratorTests.cs`
- **FILE-087**: `tests/ForgingBlazor.Tests.Unit/Routing/Culture/CultureResolverTests.cs`
- **FILE-088**: `tests/ForgingBlazor.Tests.Unit/Routing/Culture/CultureFallbackChainTests.cs`
- **FILE-089**: `tests/ForgingBlazor.Tests.Unit/Routing/Validation/RoutingConfigurationValidationTests.cs`
- **FILE-090**: `tests/ForgingBlazor.Tests.Unit/Content/FrontmatterParserTests.cs`
- **FILE-091**: `tests/ForgingBlazor.Tests.Unit/Content/MarkdownRendererTests.cs`
- **FILE-092**: `tests/ForgingBlazor.Tests.Unit/Content/ContentParserTests.cs`
- **FILE-093**: `tests/ForgingBlazor.Tests.Unit/Content/FrontmatterValidationTests.cs`
- **FILE-094**: `tests/ForgingBlazor.Tests.Unit/Content/CultureContentLocatorTests.cs`
- **FILE-095**: `tests/ForgingBlazor.Tests.Unit/Content/Validation/ContentStructureValidationTests.cs`
- **FILE-096**: `tests/ForgingBlazor.Tests.Unit/Pagination/PaginationServiceTests.cs`
- **FILE-097**: `tests/ForgingBlazor.Tests.Unit/Pagination/PaginationUrlParserTests.cs`
- **FILE-098**: `tests/ForgingBlazor.Tests.Unit/Storage/FileSystemContentStorageProviderTests.cs`
- **FILE-099**: `tests/ForgingBlazor.Tests.Unit/Storage/FileSystemWatcherServiceTests.cs`
- **FILE-100**: `tests/ForgingBlazor.Tests.Unit/Storage/ContentCacheServiceTests.cs`
- **FILE-101**: `tests/ForgingBlazor.Tests.Unit/Storage/Validation/StorageConfigurationValidationTests.cs`
- **FILE-102**: `tests/ForgingBlazor.Storage.AzureBlob.Tests.Unit/ForgingBlazor.Storage.AzureBlob.Tests.Unit.csproj`
- **FILE-103**: `tests/ForgingBlazor.Storage.AzureBlob.Tests.Unit/AzureBlobContentStorageProviderTests.cs`
- **FILE-104**: `tests/ForgingBlazor.Tests.Integration/Routing/ContentRoutingIntegrationTests.cs`
- **FILE-105**: `tests/ForgingBlazor.Tests.Integration/Storage/PublishingWorkflowIntegrationTests.cs`
- **FILE-106**: `tests/ForgingBlazor.Tests.Integration/Fixtures/TestContentFixture.cs`
- **FILE-107**: `tests/ForgingBlazor.Tests.Unit/Components/ForgingRouteViewTests.cs` - Unit tests for ForgingRouteView
- **FILE-108**: `tests/ForgingBlazor.Tests.Unit/Components/ForgingRouterTests.cs` - Unit tests for ForgingRouter

### 5.6 New Files in Xample Demonstration Project

- **FILE-107**: `src/Xample/Content/BlogPostDescriptor.cs` - Custom content descriptor for blog posts
- **FILE-108**: `src/Xample/Content/TutorialDescriptor.cs` - Custom content descriptor for tutorials
- **FILE-109**: `src/Xample/Components/Content/PostIndexComponent.razor` - Paginated post list component
- **FILE-110**: `src/Xample/Components/Content/PostDetailComponent.razor` - Single post detail component
- **FILE-111**: `src/Xample/Components/Content/TutorialIndexComponent.razor` - Tutorial list component
- **FILE-112**: `src/Xample/Components/Content/TutorialDetailComponent.razor` - Tutorial detail component
- **FILE-113**: `src/Xample/Components/Content/PageComponent.razor` - Generic page component
- **FILE-114**: `src/Xample/Components/Layout/ContentLayout.razor` - Content layout with navigation

### 5.7 New Content Files in Xample (Standard)

- **FILE-115**: `src/Xample/content/_index.md` - Home page (en-US)
- **FILE-116**: `src/Xample/content/_index.de-DE.md` - Home page (de-DE)
- **FILE-117**: `src/Xample/content/_index.fr-FR.md` - Home page (fr-FR)
- **FILE-118**: `src/Xample/content/about.md` - About page (en-US)
- **FILE-119**: `src/Xample/content/about.de-DE.md` - About page (de-DE)
- **FILE-120**: `src/Xample/content/contact/index.md` - Contact page (folder-style)
- **FILE-121**: `src/Xample/content/legal/privacy.md` - Privacy policy page
- **FILE-122**: `src/Xample/content/posts/_index.md` - Posts segment index
- **FILE-123**: `src/Xample/content/posts/_index.de-DE.md` - Posts segment index (de-DE)
- **FILE-124**: `src/Xample/content/posts/getting-started/index.md` - Getting started post
- **FILE-125**: `src/Xample/content/posts/getting-started/index.de-DE.md` - Getting started (de-DE)
- **FILE-126**: `src/Xample/content/posts/multi-language-support/index.md` - Multi-language post
- **FILE-127**: `src/Xample/content/posts/post-three.md` through `post-twelve.md` - Additional posts for pagination (10 files)
- **FILE-128**: `src/Xample/content/posts/tutorials/_index.md` - Tutorials segment index
- **FILE-129**: `src/Xample/content/posts/tutorials/beginner-guide.md` - Beginner tutorial
- **FILE-130**: `src/Xample/content/posts/tutorials/advanced-patterns.md` - Advanced tutorial
- **FILE-131**: `src/Xample/content/blog/_index.md` - Alternative blog segment index

### 5.8 New Content Files in Xample (Edge Cases)

- **FILE-132**: `src/Xample/content/posts/abc.md` - Minimum slug (3 chars)
- **FILE-133**: `src/Xample/content/posts/maximum-length-slug-that-is-exactly-seventy-characters-long-boundary.md` - Maximum slug (70 chars)
- **FILE-134**: `src/Xample/content/posts/mixed-case-slug-test.md` - Mixed case slug
- **FILE-135**: `src/Xample/content/posts/numbers-in-middle-123-test.md` - Numbers in slug
- **FILE-136**: `src/Xample/content/posts/special-post-draft.md` - Draft content
- **FILE-137**: `src/Xample/content/posts/expired-content-example.md` - Expired content
- **FILE-138**: `src/Xample/content/posts/future-publish-date.md` - Future publish date
- **FILE-139**: `src/Xample/content/posts/timezone-edge-case.md` - Extreme timezone
- **FILE-140**: `src/Xample/content/posts/unicode-content-test.md` - Unicode/emoji content
- **FILE-141**: `src/Xample/content/posts/markdown-edge-cases.md` - Complex Markdown
- **FILE-142**: `src/Xample/content/posts/yaml-special-characters.md` - YAML edge cases
- **FILE-143**: `src/Xample/content/posts/empty-optional-fields.md` - Null/empty fields
- **FILE-144**: `src/Xample/content/posts/whitespace-handling.md` - Whitespace edge cases
- **FILE-145**: `src/Xample/content/fallback-only-default.md` - Default culture only
- **FILE-146**: `src/Xample/content/posts/partial-translation.md` - Partial translations
- **FILE-147**: `src/Xample/content/posts/two-letter-culture.de.md` - Two-letter culture file

### 5.9 New Test Files in Xample.AppHost.Tests.Integration

- **FILE-148**: `tests/Xample.AppHost.Tests.Integration/Fixtures/XampleAppFixture.cs`
- **FILE-149**: `tests/Xample.AppHost.Tests.Integration/Helpers/ContentTestHelpers.cs`
- **FILE-150**: `tests/Xample.AppHost.Tests.Integration/Helpers/HtmlAssertions.cs`
- **FILE-151**: `tests/Xample.AppHost.Tests.Integration/Routing/HomePageRoutingTests.cs`
- **FILE-152**: `tests/Xample.AppHost.Tests.Integration/Routing/CultureRoutingTests.cs`
- **FILE-153**: `tests/Xample.AppHost.Tests.Integration/Routing/SegmentRoutingTests.cs`
- **FILE-154**: `tests/Xample.AppHost.Tests.Integration/Routing/PageRoutingTests.cs`
- **FILE-155**: `tests/Xample.AppHost.Tests.Integration/Routing/CanonicalUrlTests.cs`
- **FILE-156**: `tests/Xample.AppHost.Tests.Integration/Pagination/PaginationRoutingTests.cs`
- **FILE-157**: `tests/Xample.AppHost.Tests.Integration/Pagination/NumericPaginationTests.cs`
- **FILE-158**: `tests/Xample.AppHost.Tests.Integration/Pagination/PaginationBoundaryTests.cs`
- **FILE-159**: `tests/Xample.AppHost.Tests.Integration/Content/ContentResolutionTests.cs`
- **FILE-160**: `tests/Xample.AppHost.Tests.Integration/Content/DraftContentTests.cs`
- **FILE-161**: `tests/Xample.AppHost.Tests.Integration/Content/ExpiredContentTests.cs`
- **FILE-162**: `tests/Xample.AppHost.Tests.Integration/Content/FutureDateContentTests.cs`
- **FILE-163**: `tests/Xample.AppHost.Tests.Integration/Slug/SlugValidationTests.cs`
- **FILE-164**: `tests/Xample.AppHost.Tests.Integration/Slug/SlugBoundaryTests.cs`
- **FILE-165**: `tests/Xample.AppHost.Tests.Integration/Culture/CultureFallbackTests.cs`
- **FILE-166**: `tests/Xample.AppHost.Tests.Integration/Culture/CultureNegotiationTests.cs`
- **FILE-167**: `tests/Xample.AppHost.Tests.Integration/EdgeCases/UnicodeContentTests.cs`
- **FILE-168**: `tests/Xample.AppHost.Tests.Integration/EdgeCases/MarkdownRenderingTests.cs`
- **FILE-169**: `tests/Xample.AppHost.Tests.Integration/EdgeCases/YamlParsingTests.cs`
- **FILE-170**: `tests/Xample.AppHost.Tests.Integration/EdgeCases/WhitespaceHandlingTests.cs`
- **FILE-171**: `tests/Xample.AppHost.Tests.Integration/EdgeCases/TimezoneTests.cs`
- **FILE-172**: `tests/Xample.AppHost.Tests.Integration/Security/PathTraversalTests.cs`
- **FILE-173**: `tests/Xample.AppHost.Tests.Integration/Performance/CachingTests.cs`
- **FILE-174**: `tests/Xample.AppHost.Tests.Integration/Startup/ValidationTests.cs`
- **FILE-175**: `tests/Xample.AppHost.Tests.Integration/Routing/ForgingRouterSmokeTests.cs` - Integration smoke tests for ForgingRouter

### 5.10 Modified Files in Xample

- **FILE-175**: `src/Xample/Program.cs` - Complete FluentAPI configuration

## 6. Testing

### 6.1 Unit Tests

- **TEST-001**: `ContentDescriptor` property initialization and required field validation
- **TEST-002**: `ResolvedContent<T>` wrapping and metadata access
- **TEST-003**: `Check.IsValidSlug()` with all edge cases from spec section 9.5.3
- **TEST-004**: `Check.ValidateSlug()` exception throwing for invalid inputs
- **TEST-005**: `RoutingBuilder` FluentAPI method chaining
- **TEST-006**: `SlugRouteConstraint.Match()` with valid and invalid patterns
- **TEST-007**: `RouteResolver` path matching with culture and pagination
- **TEST-008**: `CanonicalUrlGenerator` with `WithPrefix` and `WithoutPrefix`
- **TEST-009**: `CultureResolver` parsing Two-Letter, LCID, and full format
- **TEST-010**: `CultureFallbackChain` complete hierarchy (100% coverage)
- **TEST-011**: `FrontmatterParser` YAML extraction and error handling
- **TEST-012**: `MarkdownRenderer` HTML output for various Markdown constructs
- **TEST-013**: `ContentParser` full pipeline with descriptor instantiation
- **TEST-014**: `FrontmatterValidation` required fields (100% coverage)
- **TEST-015**: `CultureContentLocator` file lookup order
- **TEST-016**: `PaginationService` page calculation and out-of-range handling
- **TEST-017**: `PaginationUrlParser` Numeric and Prefixed format parsing
- **TEST-018**: `FileSystemContentStorageProvider` async file operations
- **TEST-019**: `FileSystemWatcherService` change detection and debouncing
- **TEST-020**: `ContentCacheService` hit/miss and invalidation
- **TEST-021**: `RoutingConfigurationValidation` all validation rules
- **TEST-022**: `ContentStructureValidation` missing file detection
- **TEST-023**: `StorageConfigurationValidation` provider and path validation

### 6.2 Integration Tests

- **TEST-024**: `AzureBlobContentStorageProvider` with Azurite emulator
- **TEST-025**: Full content routing request/response cycle
- **TEST-026**: Publishing workflow: draft → confirm → publish
- **TEST-027**: Content expiration filtering
- **TEST-028**: Multi-culture content resolution with fallback

### 6.3 Xample.AppHost.Tests.Integration (End-to-End Tests)

#### Routing Tests

- **TEST-029**: Home page routing - `/` returns home page, `/en-US` returns home with canonical to `/`, `/de-DE` returns German home
- **TEST-030**: Culture routing - supported cultures return content, unsupported `/es-ES` returns 404, fallback chain works
- **TEST-031**: Segment routing - `/posts` returns index, `/posts/getting-started` returns detail, nested segments work
- **TEST-032**: Page routing - `/about` standalone, `/contact` folder-style, `/legal/privacy` nested page
- **TEST-033**: Canonical URL - canonical link in HTML head, non-canonical URLs include canonical, no HTTP redirects

#### Pagination Tests

- **TEST-034**: Pagination routing - `/posts` page 1, `/posts/page-2` page 2, `/posts/page-1` canonical to `/posts`
- **TEST-035**: Numeric pagination - `/posts/tutorials/2` works, `/posts/tutorials/1` canonical to `/posts/tutorials`
- **TEST-036**: Pagination boundaries - PageSize items = 1 page, PageSize+1 = 2 pages, empty segment handling
- **TEST-037**: Out-of-range pagination - `/posts/page-999` returns 404

#### Content Tests

- **TEST-038**: Content resolution - frontmatter parsed, custom descriptor fields populated, Markdown rendered
- **TEST-039**: Draft content - `draft: true` returns 404 in production, accessible in development
- **TEST-040**: Expired content - past `expiredAt` returns 404, future `expiredAt` renders, null never expires
- **TEST-041**: Future publish date - content with future `publishedDate` handling

#### Slug Tests

- **TEST-042**: Slug validation - valid slugs resolve, invalid patterns return 404, case sensitivity
- **TEST-043**: Slug boundaries - 3-char slug works, 70-char slug works, 2-char returns 404, 71-char returns 404

#### Culture Tests

- **TEST-044**: Culture fallback - `de-DE` → `de` → `en-US` → `en` chain, partial translations, two-letter files
- **TEST-045**: Culture negotiation - Accept-Language header, explicit URL overrides header

#### Edge Case Tests

- **TEST-046**: Unicode content - emoji, CJK, RTL, special symbols render correctly
- **TEST-047**: Markdown rendering - code blocks, tables, task lists, nested lists
- **TEST-048**: YAML parsing - special characters, multiline, complex structures, arrays
- **TEST-049**: Whitespace handling - preserved in content, trimmed in frontmatter
- **TEST-050**: Timezone handling - DateTimeOffset across timezones, UTC normalization

#### Security Tests

- **TEST-051**: Path traversal - `../` rejected, encoded traversal rejected, absolute paths rejected

#### Performance Tests

- **TEST-052**: Caching - second request from cache, invalidation on file change, culture-aware keys

#### Startup Tests

- **TEST-053**: Validation - missing `_index.md` fails, missing default culture fails, invalid config fails

### 6.4 Coverage Requirements

- **TEST-COV-001**: Minimum 80% code coverage for all core libraries
- **TEST-COV-002**: 100% coverage for validation logic (`FrontmatterValidation`, `RoutingConfigurationValidation`, `ContentStructureValidation`, `StorageConfigurationValidation`)
- **TEST-COV-003**: 100% coverage for culture fallback logic (`CultureFallbackChain`, `CultureContentLocator`)
- **TEST-COV-004**: All edge cases from spec section 9.5 covered by Xample integration tests

## 7. Risks & Assumptions

### 7.1 Risks

- **RISK-001**: FileSystemWatcher may have platform-specific behavior differences between Windows, Linux, and macOS. Mitigation: Use FileSystemWatcher only in development mode; provide configuration to disable.
- **RISK-002**: Large content collections may impact startup validation performance. Mitigation: Implement parallel validation; consider lazy validation for non-critical paths.
- **RISK-003**: YamlDotNet and Markdig library updates may introduce breaking changes. Mitigation: Pin to specific versions; add comprehensive parser tests.
- **RISK-004**: Azure Blob Storage connection string exposure in configuration. Mitigation: Document secure configuration practices; support Azure Identity authentication.
- **RISK-005**: Memory pressure from caching large content collections. Mitigation: Implement cache size limits; use sliding expiration; document memory requirements.

### 7.2 Assumptions

- **ASSUMPTION-001**: Content authors will use UTF-8 encoding for all Markdown files
- **ASSUMPTION-002**: Content files will follow the defined folder structure conventions
- **ASSUMPTION-003**: Default culture content will always be provided before translations
- **ASSUMPTION-004**: Production environments will use publishing storage (Azure Blob) rather than FileSystem
- **ASSUMPTION-005**: Development environments have sufficient file system permissions for FileSystemWatcher
- **ASSUMPTION-006**: Azurite emulator is available for Azure Blob Storage integration tests
- **ASSUMPTION-007**: TUnit test framework is properly configured in all test projects

## 8. Related Specifications / Further Reading

- [Dynamic Content Routing Design Specification](../spec/spec-design-dynamic-content-routing.md)
- [Folder Structure and Naming Conventions Decision](../decisions/2025-07-10-folder-structure-and-naming-conventions.md)
- [Centralized Package Version Management Decision](../decisions/2025-07-10-centralized-package-version-management.md)
- [.NET 10 / C# 13 Adoption Decision](../decisions/2025-07-11-dotnet-10-csharp-13-adoption.md)
- [English as Project Language Decision](../decisions/2025-07-11-english-as-project-language.md)
- [DateTimeOffset and TimeProvider Usage Decision](../decisions/2026-01-21-datetimeoffset-and-timeprovider-usage.md)
- [No Code Regions Decision](../decisions/2026-01-21-no-code-regions.md)
- [YamlDotNet Documentation](https://github.com/aaubry/YamlDotNet)
- [Markdig Documentation](https://github.com/xoofx/markdig)
- [Azure Blob Storage Documentation](https://learn.microsoft.com/azure/storage/blobs/)
