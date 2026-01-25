# Layout Change Detection for Cache Invalidation

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines the mechanism for detecting layout changes and invalidating cached static assets in ForgingBlazor.

When layouts change (navigation, footer, CSS), all cached static assets in `wwwroot` become stale. This specification addresses automatic detection and cache invalidation strategies.

---

## 2. Problem Statement

ForgingBlazor uses static asset caching (see [Routing Specification §8.6](./routing.md#86-content-caching-strategy)) where rendered content is saved as HTML files in `wwwroot`. When shared layouts change, these cached files become stale and must be regenerated.

### 2.1 Current Behavior

Cache invalidation is triggered by application restart only.

### 2.2 Limitations

| Limitation                  | Impact                                     |
| --------------------------- | ------------------------------------------ |
| **Manual Restart Required** | Layout changes require application restart |
| **Full Cache Clear**        | No granular invalidation possible          |
| **Development Friction**    | Slows down layout iteration                |

---

## 3. Goals

| Goal                      | Description                                       |
| ------------------------- | ------------------------------------------------- |
| **Automatic Detection**   | Detect layout changes without manual intervention |
| **Granular Invalidation** | Clear only affected pages when possible           |
| **Performance**           | Minimal impact on startup and request time        |
| **Development Support**   | Fast iteration during development                 |

---

## 4. Proposed Options

### 4.1 Hash-based Detection

Store a hash of layout files. Invalidate all pages when hash changes.

```csharp
public interface ILayoutHashProvider
{
    ValueTask<string> ComputeHashAsync(CancellationToken cancellationToken = default);
    ValueTask<bool> HasChangedAsync(string previousHash, CancellationToken cancellationToken = default);
}
```

#### Advantages

| Advantage        | Description                      |
| ---------------- | -------------------------------- |
| **Simple**       | Easy to implement and understand |
| **Reliable**     | Hash comparison is deterministic |
| **Startup-safe** | Hash computed once at startup    |

#### Disadvantages

| Disadvantage             | Description                                      |
| ------------------------ | ------------------------------------------------ |
| **All-or-nothing**       | Changes to any layout file invalidates all cache |
| **No runtime detection** | Requires restart to detect changes               |

### 4.2 File Watcher

Monitor layout files and auto-regenerate on change.

```csharp
public interface ILayoutWatcher : IDisposable
{
    event EventHandler<LayoutChangedEventArgs>? LayoutChanged;
    void Start();
    void Stop();
}
```

#### Advantages

| Advantage                | Description                  |
| ------------------------ | ---------------------------- |
| **Real-time**            | Changes detected immediately |
| **Development-friendly** | No restart required          |

#### Disadvantages

| Disadvantage        | Description                                       |
| ------------------- | ------------------------------------------------- |
| **Complexity**      | File system watchers can be unreliable            |
| **Resource Usage**  | Continuous monitoring consumes resources          |
| **Not Cloud-ready** | File watchers don't work in all hosting scenarios |

### 4.3 Manual Trigger

Admin API endpoint to trigger cache clear.

```csharp
public interface ICacheInvalidationService
{
    ValueTask InvalidateAllAsync(CancellationToken cancellationToken = default);
    ValueTask InvalidatePathAsync(string path, CancellationToken cancellationToken = default);
}
```

#### Advantages

| Advantage     | Description                              |
| ------------- | ---------------------------------------- |
| **Control**   | Explicit control over cache invalidation |
| **Simple**    | No detection logic needed                |
| **Universal** | Works in all hosting scenarios           |

#### Disadvantages

| Disadvantage    | Description                      |
| --------------- | -------------------------------- |
| **Manual**      | Requires human intervention      |
| **Error-prone** | Easy to forget after deployments |

### 4.4 Dependency Graph

Track layout dependencies, invalidate only affected pages.

```csharp
public interface ILayoutDependencyTracker
{
    void RegisterDependency(string pagePath, string layoutPath);
    IEnumerable<string> GetAffectedPages(string layoutPath);
}
```

#### Advantages

| Advantage     | Description                     |
| ------------- | ------------------------------- |
| **Granular**  | Only affected pages regenerated |
| **Efficient** | Minimal cache churn             |

#### Disadvantages

| Disadvantage    | Description                        |
| --------------- | ---------------------------------- |
| **Complex**     | Requires tracking all dependencies |
| **Memory**      | Dependency graph consumes memory   |
| **Maintenance** | Graph must be kept in sync         |

---

## 5. Open Questions

| #   | Question                                                    | Context                              |
| --- | ----------------------------------------------------------- | ------------------------------------ |
| 1   | Which option(s) should be implemented?                      | May combine multiple approaches      |
| 2   | Should detection differ between Development and Production? | File watcher for dev, hash for prod? |
| 3   | How to handle CSS/JS changes?                               | Part of layout or separate?          |
| 4   | Should cache be disabled entirely in Development?           | Simplest dev experience              |

---

## 6. Related Specifications

- [Routing Specification](./routing.md) - Parent specification
- [Routing Specification §8.6](./routing.md#86-content-caching-strategy) - Content Caching Strategy
- [Routing Specification §8.7](./routing.md#87-cache-exclusion-for-dynamic-content) - Cache Exclusion

---

## Revision History

| Version | Date       | Author  | Changes       |
| ------- | ---------- | ------- | ------------- |
| 0.1.0   | 2026-01-25 | Initial | Initial draft |
