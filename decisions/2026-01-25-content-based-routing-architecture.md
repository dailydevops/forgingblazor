---
authors:
  - Martin Stühmer (https://github.com/samtrion)

applyTo:
  - "src/ForgingBlazor/**/*.cs"
  - "src/ForgingBlazor/**/*.razor"
  - "src/ForgingBlazor.Extensibility/**/*.cs"
  - "specs/routing.md"

created: 2026-01-25

lastModified: 2026-01-25

state: accepted

instructions: |
  ForgingBlazor uses content-based routing with compile-time segment discovery.
  MUST define all routes via ConfigureRouting<TApp, THome>() - no runtime discovery.
  MUST use ForgingRouter/ForgingRouteView instead of Blazor Router/RouteView.
  Template components MUST NOT contain @page or @layout directives.
  MUST use Static Assets caching in wwwroot with ExcludeFromCache() for dynamic content.
  MUST use TimeProvider via constructor injection for time-based operations.
  MUST use separate IAssetProvider for images/CSS (not in content folders).
  Route constraints: Blazor standard + alpha (slugs) + lang (culture with inheritance).
  Segment nesting: Soft limit 5 levels (warning), hard limit 8 levels (error).
---

# Content-Based Routing Architecture

This ADR documents the architectural decisions for ForgingBlazor's content-based routing system.

## Context

ForgingBlazor requires a routing mechanism that serves content from multiple storage providers (Physical FileSystem, Azure Blob Storage) while maintaining compatibility with Blazor's component model.

Key requirements:

- Support content-based routing from configurable storage providers
- Enable content synchronization between local and cloud storage
- Maintain Blazor compatibility for non-content routes
- Support multi-language content with culture fallback
- Provide efficient caching for static content

## Decision

### 1. Compile-Time Segment Discovery

All routes MUST be explicitly defined via `ConfigureRouting<TApp, THome>()`. Runtime discovery from storage structure is not supported.

**Rationale:**

- Type safety: All routes validated at compile-time
- Performance: No storage scanning at startup or runtime
- Security: No unintended routes exposed
- Predictability: Route structure explicit in code

### 2. ForgingRouter and ForgingRouteView

Custom router components replace Blazor's `Router` and `RouteView`:

- `ForgingRouter`: Combines Blazor `@page` routes with ForgingBlazor content routes
- `ForgingRouteView`: Provides content via `CascadingParameter<ResolvedContent<T>>`

ForgingBlazor routes take precedence over `@page` routes for same paths.

### 3. Template Component Restrictions

Components used as `TPageTemplate` or `TSegmentTemplate` MUST NOT contain:

- `@page` directive (routing managed by ForgingBlazor)
- `@layout` directive (layout inherited through routing hierarchy)

**Enforcement:** Roslyn Analyzer (warning) + Runtime validation (error).

### 4. Route Constraints

Blazor/ASP.NET Core standard constraints plus ForgingBlazor extensions:

- `alpha`: URL-friendly slugs (`a-zA-Z0-9-`)
- `lang`: Culture codes with automatic inheritance to nested elements

### 5. Static Asset Caching

Content is rendered to static HTML files in `wwwroot`. Web server serves files directly, bypassing application code.

**Cache Invalidation Triggers:**

- Content change: Remove specific static asset
- Application restart: Clear all static assets
- `ExcludeFromCache()`: Opt-out for dynamic content

### 6. Segment Nesting Limits

- Soft Limit: 5 levels (warning)
- Hard Limit: 8 levels (error)

### 7. Asset Management

Assets managed via separate `IAssetProvider` interface with dedicated storage directory. Assets are NOT stored within content folders.

### 8. TimeProvider Usage

`TimeProvider` provided via constructor injection for all time-based operations (publish dates, expiry dates).

### 9. Storage Provider Architecture

- Core library (`ForgingBlazor`): Physical FileSystem provider + synchronization
- Separate library (`ForgingBlazor.Providers.AzureBlob`): Azure Blob Storage provider
- Synchronization uses fast-exit pattern (no-op for single provider)

### 10. Culture Fallback Chain

For requested culture `de-DE`:

```
de-DE → de → en-US → en → (no suffix)
```

## Consequences

### Benefits

| Benefit                  | Description                                           |
| ------------------------ | ----------------------------------------------------- |
| **Type Safety**          | Compile-time route validation                         |
| **Performance**          | Static file serving, no runtime content resolution    |
| **Blazor Compatibility** | Standard `@page` routes work alongside content routes |
| **Flexibility**          | Multiple storage providers, culture support           |
| **Testability**          | TimeProvider injection enables time-based testing     |

### Trade-offs

| Trade-off                       | Mitigation                                              |
| ------------------------------- | ------------------------------------------------------- |
| **No Dynamic Segments**         | Use parameterized pages (`{slug}`) for flexible content |
| **Deployment for New Segments** | Redeploy application with updated segment definitions   |
| **Cache Staleness**             | Application restart or explicit invalidation            |

### Risks

| Risk               | Mitigation                                         |
| ------------------ | -------------------------------------------------- |
| **Disk Space**     | Monitor `wwwroot` folder size in production        |
| **Layout Changes** | Application restart invalidates all cached content |

## Alternatives Considered

### Runtime Segment Discovery

**Rejected:** Would compromise type safety, performance, and security.

### In-Memory Caching

**Rejected:** Static assets provide better performance and CDN compatibility.

### Integrated Asset Storage

**Rejected:** Separate storage enables CDN integration and independent asset management.

### TimeProvider via Static Property

**Rejected:** Constructor injection provides better testability and consistency.

## Related Decisions

- [DateTimeOffset and TimeProvider Usage](./2026-01-21-datetimeoffset-and-timeprovider-usage.md) - TimeProvider injection pattern
- [Folder Structure and Naming Conventions](./2025-07-10-folder-structure-and-naming-conventions.md) - Project organization
- [.NET 10 / C# 13 Adoption](./2025-07-11-dotnet-10-csharp-13-adoption.md) - Framework version

## Related Specifications

- [Routing Specification](../specs/routing.md) - Complete technical specification
- [Layout Change Detection](../specs/layout-change-detection.md) - Future consideration
- [Route Conflict Resolution](../specs/route-conflict-resolution.md) - Future consideration
- [Content Authorization](../specs/content-authorization.md) - Future consideration
- [Content SEO](../specs/content-seo.md) - Future consideration
