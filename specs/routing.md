# Routing Specification

> **Status:** Accepted  
> **Version:** 1.0.0  
> **Created:** 2026-01-24  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **ADR:** [Content-Based Routing Architecture](../decisions/2026-01-25-content-based-routing-architecture.md)

---

## Table of Contents

1. [Overview](#1-overview)
2. [Goals and Objectives](#2-goals-and-objectives)
3. [Routing Architecture](#3-routing-architecture)
4. [Segment Constraints](#4-segment-constraints)
5. [Content Storage Structure](#5-content-storage-structure)
6. [Property Inheritance](#6-property-inheritance)
7. [Storage Providers](#7-storage-providers)
8. [Content Synchronization](#8-content-synchronization)
9. [Content Descriptor](#9-content-descriptor)
10. [Routing Resolution Pipeline](#10-routing-resolution-pipeline)
11. [Future Considerations](#11-future-considerations)

---

## 1. Overview

This specification defines a content-based routing mechanism for ForgingBlazor that enables serving content from multiple storage providers. The routing system is designed to support:

- **Physical FileSystem** for local development and content management
- **Azure Blob Storage** (via a separate library) for production and distributed scenarios

The primary use case is operating a cost-effective, decentralized blog system where content can be exchanged between local systems and cloud storage, enabling scenarios like scheduled publishing.

---

## 2. Goals and Objectives

### 2.1 Primary Goals

| Goal                         | Description                                                                                                       |
| ---------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| **Content-Based Routing**    | Route requests based on content structure in storage providers                                                    |
| **Multi-Storage Support**    | Support Physical FileSystem and Azure Blob Storage as interchangeable backends                                    |
| **Content Synchronization**  | Enable transfer of content between storage providers (e.g., local to Azure Blob Storage for scheduled publishing) |
| **Cost-Effective Operation** | Allow decentralized blog operation with minimal infrastructure costs                                              |

### 2.2 Secondary Goals

| Goal                               | Description                                                                 |
| ---------------------------------- | --------------------------------------------------------------------------- |
| **Declarative Routing Definition** | Define routing structure in `Program.cs` that mirrors storage structure     |
| **Property Inheritance**           | Support property inheritance (e.g., language) through the routing hierarchy |
| **Configurable Constraints**       | Provide route constraints for filtering and validation                      |
| **Unique Segment Enforcement**     | Ensure segment uniqueness across the routing tree                           |

---

## 3. Routing Architecture

### 3.1 Routing Hierarchy

The routing system uses a recursive hierarchy that supports nested segments:

```
Root
├── Segments (e.g., "posts", "projects", "about")
│   ├── Nested Segments (e.g., "posts/category")
│   │   ├── Further Nested Segments (unlimited depth)
│   │   ├── Pages
│   │   └── Pagination (optional)
│   ├── Pages (e.g., "{slug}", "detail")
│   └── Pagination (optional)
└── Pages (e.g., "error", "notfound")
```

### 3.2 Interface Definitions

The routing system is built on the following interfaces defined in `ForgingBlazor.Extensibility`:

#### IRootBuilder

```csharp
public interface IRootBuilder
{
    IRootBuilder ExcludeFromCache();

    IRootBuilder WithErrorPage<TPageTemplate>()
        where TPageTemplate : class, IComponent;

    IRootBuilder WithNotFoundPage<TPageTemplate>()
        where TPageTemplate : class, IComponent;

    IRootBuilder WithPage<TPageTemplate>(string segment, Action<IPageBuilder> pageBuilder)
        where TPageTemplate : class, IComponent;

    IRootBuilder WithSegment<TSegmentTemplate>(string segment, Action<ISegmentBuilder> segmentBuilder)
        where TSegmentTemplate : class, IComponent;
}
```

#### ISegmentBuilder

```csharp
public interface ISegmentBuilder
{
    ISegmentBuilder ExcludeFromCache();

    ISegmentBuilder WithPage<TPageTemplate>(string segment, Action<IPageBuilder> pageBuilder)
        where TPageTemplate : class, IComponent;

    ISegmentBuilder WithPagination<TPageTemplate>(PaginationMode? mode = null)
        where TPageTemplate : class, IComponent;

    ISegmentBuilder WithSegment<TSegmentTemplate>(string segment, Action<ISegmentBuilder> segmentBuilder)
        where TSegmentTemplate : class, IComponent;
}
```

> **Note:** `ISegmentBuilder.WithSegment` enables recursive nesting of segments to any depth.

#### IPageBuilder

```csharp
public interface IPageBuilder
{
    IPageBuilder DisableMarkdown();

    IPageBuilder ExcludeFromCache();
}
```

### 3.3 Example Usage

```csharp
var builder = ForgingBlazorApplication
    .CreateDefaultBuilder(args)
    .ConfigureRouting<App, Home>(root =>
    {
        _ = root
            .WithErrorPage<Error>()
            .WithNotFoundPage<NotFound>()
            .WithSegment<PostList>(
                "posts",
                segment => _ = segment
                    .WithPagination<PostList>()
                    .WithPage<PostDetail>("{slug}", page => { })
                    // Nested segment example
                    .WithSegment<CategoryList>(
                        "category",
                        categorySegment => _ = categorySegment
                            .WithPage<CategoryDetail>("{category}", page => { })
                    )
            );
    });
```

#### Resulting Routes

| Route                        | Component        | Content Path                                 |
| ---------------------------- | ---------------- | -------------------------------------------- |
| `/`                          | `Home`           | `content/_index.md`                          |
| `/posts`                     | `PostList`       | `content/posts/_index.md`                    |
| `/posts/{slug}`              | `PostDetail`     | `content/posts/{slug}/index.md`              |
| `/posts/category`            | `CategoryList`   | `content/posts/category/_index.md`           |
| `/posts/category/{category}` | `CategoryDetail` | `content/posts/category/{category}/index.md` |

### 3.4 Segment Discovery Mode

Segments are discovered at **compile-time only**. All segments must be explicitly defined in `ConfigureRouting()`. Runtime discovery from storage structure is not supported.

| Aspect             | Benefit                                                             |
| ------------------ | ------------------------------------------------------------------- |
| **Type Safety**    | All routes are validated at compile-time, preventing runtime errors |
| **Intellisense**   | Full IDE support for route configuration                            |
| **Performance**    | No storage scanning required at startup or request time             |
| **Security**       | No risk of unintended routes being exposed from storage structure   |
| **Predictability** | Route structure is explicit and documented in code                  |

### 3.5 Router Components

ForgingBlazor provides custom router components that replace Blazor's standard `Router` and `RouteView` while maintaining feature compatibility.

#### 3.5.1 ForgingRouter

`ForgingRouter` replaces Blazor's `Router` component and integrates with ForgingBlazor's content-based routing system.

##### Parameters (Blazor-Compatible)

| Parameter              | Type                               | Required | Description                                   |
| ---------------------- | ---------------------------------- | -------- | --------------------------------------------- |
| `AppAssembly`          | `Assembly`                         | Yes      | Assembly to search for routable components    |
| `AdditionalAssemblies` | `IEnumerable<Assembly>`            | No       | Additional assemblies for route discovery     |
| `Found`                | `RenderFragment<RouteData>`        | Yes      | Content to render when route is matched       |
| `NotFound`             | `RenderFragment`                   | No       | _(Deprecated)_ Content when no route matches  |
| `NotFoundPage`         | `Type`                             | No       | Component type for 404 pages                  |
| `Navigating`           | `RenderFragment`                   | No       | Content during async navigation               |
| `OnNavigateAsync`      | `EventCallback<NavigationContext>` | No       | Handler before navigation                     |
| `PreferExactMatches`   | `bool`                             | No       | Prefer exact route matches over parameterized |

##### ForgingBlazor Extensions

| Parameter            | Type                  | Description                                          |
| -------------------- | --------------------- | ---------------------------------------------------- |
| `ContentResolver`    | `IContentResolver`    | _(Injected)_ Resolves content from storage providers |
| `RouteConfiguration` | `IRouteConfiguration` | _(Injected)_ Access to configured routing tree       |

##### Usage Example

```razor
@* Routes.razor *@
<ForgingRouter AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <ForgingRouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)" />
        <FocusOnNavigate RouteData="routeData" Selector="h1" />
    </Found>
    <NotFound>
        <LayoutView Layout="typeof(Layout.MainLayout)">
            <p>Page not found.</p>
        </LayoutView>
    </NotFound>
</ForgingRouter>
```

##### Integration with ConfigureRouting

`ForgingRouter` automatically integrates with routes defined via `ConfigureRouting<TApp, THome>()`:

1. **Route Discovery:** Combines Blazor `@page` routes with ForgingBlazor configured routes
2. **Priority:** ForgingBlazor routes take precedence over `@page` routes for same paths
3. **Content Resolution:** Matched ForgingBlazor routes trigger content loading from storage providers

##### Route Matching Behavior

| Scenario                     | Behavior                                                   |
| ---------------------------- | ---------------------------------------------------------- |
| ForgingBlazor route matches  | Load content from storage, render with configured template |
| Blazor `@page` route matches | Standard Blazor routing (for admin pages, etc.)            |
| No match                     | Render `NotFoundPage` or `NotFound` content                |

#### 3.5.2 ForgingRouteView

`ForgingRouteView` replaces Blazor's `RouteView` and handles content injection into template components.

##### Parameters (Blazor-Compatible)

| Parameter       | Type        | Required | Description                                     |
| --------------- | ----------- | -------- | ----------------------------------------------- |
| `RouteData`     | `RouteData` | Yes      | Route data from `ForgingRouter`                 |
| `DefaultLayout` | `Type`      | No       | Default layout if component doesn't specify one |

##### ForgingBlazor Extensions

| Parameter | Type                                  | Description                                 |
| --------- | ------------------------------------- | ------------------------------------------- |
| `Content` | `ResolvedContent<ContentDescriptor>?` | _(Cascading)_ Resolved content from storage |

##### Content Injection

`ForgingRouteView` provides resolved content to template components via:

1. **CascadingValue:** `ResolvedContent<TContentType>` cascaded to child components
2. **Parameter Binding:** Components can receive content via `[Parameter]` or `[CascadingParameter]`

```razor
@* PostDetail.razor - Template component *@
<article>
    <h1>@Content?.Descriptor.Title</h1>
    <div class="content">
        @((MarkupString)(Content?.RenderedHtml ?? string.Empty))
    </div>
</article>

@code {
    [CascadingParameter]
    public ResolvedContent<ContentDescriptor>? Content { get; set; }
}
```

##### Layout Resolution

Layout is resolved in the following order:

1. `@layout` directive on component _(forbidden for ForgingBlazor templates)_
2. `DefaultLayout` parameter on `ForgingRouteView`
3. No layout (component renders directly)

#### 3.5.3 Feature Compatibility Matrix

| Feature                      | Blazor Router | ForgingRouter | Notes                            |
| ---------------------------- | ------------- | ------------- | -------------------------------- |
| `@page` route discovery      | ✅            | ✅            | Standard Blazor routes supported |
| `AppAssembly`                | ✅            | ✅            | Required parameter               |
| `AdditionalAssemblies`       | ✅            | ✅            | Full support                     |
| `Found` / `NotFound`         | ✅            | ✅            | Standard render fragments        |
| `NotFoundPage`               | ✅            | ✅            | Component-based 404              |
| `Navigating`                 | ✅            | ✅            | Loading indicator support        |
| `OnNavigateAsync`            | ✅            | ✅            | Pre-navigation handler           |
| `PreferExactMatches`         | ✅            | ✅            | Route matching preference        |
| Content-based routing        | ❌            | ✅            | ForgingBlazor extension          |
| Storage provider integration | ❌            | ✅            | ForgingBlazor extension          |
| Culture-aware routing        | ❌            | ✅            | Via `{culture:lang}` constraint  |

#### 3.5.4 Migration from Blazor Router

Migrating from standard Blazor `Router` to `ForgingRouter`:

**Before (Blazor Standard):**

```razor
<Router AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)" />
    </Found>
    <NotFound>
        <p>Not found</p>
    </NotFound>
</Router>
```

**After (ForgingBlazor):**

```razor
<ForgingRouter AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <ForgingRouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)" />
    </Found>
    <NotFound>
        <p>Not found</p>
    </NotFound>
</ForgingRouter>
```

> **Note:** The migration is a drop-in replacement. Existing `@page` routes continue to work alongside ForgingBlazor content routes.

---

## 4. Segment Constraints

### 4.1 Segment Naming Rules

| Rule                 | Constraint                                                                    |
| -------------------- | ----------------------------------------------------------------------------- |
| **Character Set**    | Alphanumeric characters only (`a-z`, `A-Z`, `0-9`, `-`)                       |
| **Minimum Length**   | 3 characters                                                                  |
| **Maximum Length**   | 70 characters                                                                 |
| **Uniqueness**       | Must be unique within the current segment (parent scope), not the entire tree |
| **Case Sensitivity** | Case-insensitive comparison for uniqueness                                    |

#### Uniqueness Scope Examples

```
Root
├── posts              ✅ Unique within Root
│   ├── featured       ✅ Unique within "posts"
│   └── category       ✅ Unique within "posts"
│       └── featured   ✅ Unique within "category" (same name as in "posts" is allowed)
├── projects           ✅ Unique within Root
│   └── featured       ✅ Unique within "projects" (same name as in "posts" is allowed)
└── featured           ✅ Unique within Root (no conflict with nested "featured" segments)
```

**Valid scenario:** The segment name `featured` can appear at multiple levels:

- `/posts/featured`
- `/posts/category/featured`
- `/projects/featured`

**Invalid scenario:** Two segments with the same name at the same level:

```csharp
.WithSegment<PostList>("posts", segment => { })
.WithSegment<OtherList>("posts", segment => { })  // ❌ Duplicate within Root
```

### 4.2 Reserved Segments

The following segments are reserved and cannot be used:

| Segment    | Purpose                                 |
| ---------- | --------------------------------------- |
| `error`    | Error page (via `WithErrorPage`)        |
| `notfound` | Not found page (via `WithNotFoundPage`) |
| `_index`   | Reserved for index files                |

### 4.3 Component Template Restrictions

Razor components used as `TPageTemplate` or `TSegmentTemplate` **MUST NOT** contain the following directives:

| Forbidden Directive | Reason                                                                                        |
| ------------------- | --------------------------------------------------------------------------------------------- |
| `@page`             | Routing is managed by ForgingBlazor's routing configuration, not by Blazor's built-in router  |
| `@layout`           | Layout is determined by the application structure and inherited through the routing hierarchy |

#### Rationale

- **Centralized Routing Control:** ForgingBlazor manages all routing through the `ConfigureRouting<TApp, THome>()` configuration. Using `@page` directives would create conflicts and bypass the content-based routing mechanism.
- **Layout Inheritance:** Layouts are controlled at the application level to ensure consistent rendering across content types. Individual components should not override this behavior.
- **Template Reusability:** Components without routing directives can be reused across different route configurations without modification.

**Valid Example:**

```razor
@* PostDetail.razor *@
@using NetEvolve.ForgingBlazor.Content

<article>
    <h1>@Content?.Title</h1>
    <div class="content">
        @((MarkupString)(Content?.RenderedHtml ?? string.Empty))
    </div>
</article>

@code {
    [Parameter]
    public ResolvedContent<ContentDescriptor>? Content { get; set; }
}
```

**Invalid Example:**

```razor
@* PostDetail.razor - INVALID *@
@page "/posts/{slug}"           @* ❌ FORBIDDEN *@
@layout MainLayout              @* ❌ FORBIDDEN *@

<article>
    <h1>@Content?.Title</h1>
</article>
```

#### Enforcement

Template restrictions are enforced through a combination of compile-time and runtime validation:

| Layer                              | Behavior                            | Purpose                                             |
| ---------------------------------- | ----------------------------------- | --------------------------------------------------- |
| **Compile-time (Roslyn Analyzer)** | Warning (FB0001, FB0002)            | Early feedback during development, IDE integration  |
| **Runtime (Build validation)**     | Error (`InvalidOperationException`) | Prevents application startup with invalid templates |

```csharp
// Analyzer diagnostics
// FB0001: Template component should not contain @page directive
// FB0002: Template component should not contain @layout directive
```

### 4.4 Route Constraints

Route constraints allow filtering and validation of route parameters. ForgingBlazor adopts the standard Blazor/ASP.NET Core route constraints for consistency and developer familiarity.

#### Supported Constraints

| Constraint | Example           | Example Matches                        | Invariant Culture |
| ---------- | ----------------- | -------------------------------------- | ----------------- |
| `alpha`    | `{slug:alpha}`    | `a-z`, `A-Z`, `0-9`, `-`               | No                |
| `bool`     | `{active:bool}`   | `true`, `FALSE`                        | No                |
| `datetime` | `{dob:datetime}`  | `2016-12-31`, `2016-12-31 7:32pm`      | Yes               |
| `decimal`  | `{price:decimal}` | `49.99`, `-1,000.01`                   | Yes               |
| `double`   | `{weight:double}` | `1.234`, `-1,001.01e8`                 | Yes               |
| `float`    | `{weight:float}`  | `1.234`, `-1,001.01e8`                 | Yes               |
| `guid`     | `{id:guid}`       | `00001111-aaaa-2222-bbbb-3333cccc4444` | No                |
| `int`      | `{id:int}`        | `123456789`, `-123456789`              | Yes               |
| `lang`     | `{culture:lang}`  | `en-US`, `de`, `fr-FR`                 | No                |
| `long`     | `{ticks:long}`    | `123456789`, `-123456789`              | Yes               |
| `nonfile`  | `{param:nonfile}` | Not `file.css`, not `favicon.ico`      | Yes               |

> **Note:** The `alpha` constraint is a ForgingBlazor extension that matches URL-friendly slugs containing lowercase/uppercase letters, digits, and hyphens (`a-zA-Z0-9-`).

> **Note:** The `lang` constraint is a ForgingBlazor extension that validates against the configured supported languages. Only culture codes registered via application configuration are accepted. **Important:** When a route parameter uses the `lang` constraint, the matched culture is automatically applied to the current segment and all nested elements (segments, pages, pagination). This enables culture-specific content resolution without additional configuration.

#### Multi-Language Support Configuration

The `lang` constraint requires multi-language support to be configured using ASP.NET Core's [Request Localization Middleware](https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?view=aspnetcore-10.0).

##### Configuration Example

```csharp
// Program.cs
builder.Services.AddLocalization();

var supportedCultures = new[] { "en-US", "de-DE", "fr-FR" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])  // Primary language
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
```

##### Primary Language and Alias Behavior

When multi-language support is configured:

| Behavior                | Description                                                                                                                    |
| ----------------------- | ------------------------------------------------------------------------------------------------------------------------------ |
| **Primary Language**    | The primary language (first in `supportedCultures`) is always served at the **root** path without a language segment           |
| **Secondary Languages** | Secondary languages use the `{culture:lang}` segment as part of the URL path                                                   |
| **Alias Treatment**     | The language segment acts as an **alias** - content is stored in the same location but resolved with culture-specific suffixes |

##### Route Mapping Example

With `supportedCultures = ["en-US", "de-DE", "fr-FR"]` (primary: `en-US`):

| URL                        | Culture | Content Path (fallback chain applied)                                      |
| -------------------------- | ------- | -------------------------------------------------------------------------- |
| `/posts/hello-world`       | `en-US` | `posts/hello-world/index.md`                                               |
| `/de-DE/posts/hello-world` | `de-DE` | `posts/hello-world/index.de-DE.md` → `posts/hello-world/index.de.md` → ... |
| `/fr-FR/posts/hello-world` | `fr-FR` | `posts/hello-world/index.fr-FR.md` → `posts/hello-world/index.fr.md` → ... |

##### Configuration with ForgingBlazor Routing

```csharp
var builder = ForgingBlazorApplication
    .CreateDefaultBuilder(args)
    .ConfigureRouting<App, Home>(root =>
    {
        _ = root
            .WithErrorPage<Error>()
            .WithNotFoundPage<NotFound>()
            // Primary language (en-US) - served at root
            .WithSegment<PostList>("posts", segment => _ = segment
                .WithPage<PostDetail>("{slug:alpha}", page => { })
            )
            // Secondary languages - served under language segment (alias)
            .WithSegment<LocalizedRoot>("{culture:lang}", langSegment => _ = langSegment
                .WithSegment<PostList>("posts", segment => _ = segment
                    .WithPage<PostDetail>("{slug:alpha}", page => { })
                )
            );
    });
```

> **Note:** The content is **not duplicated** in storage. Both `/posts/hello-world` and `/de-DE/posts/hello-world` resolve to content in `posts/hello-world/`, with the culture suffix determining which file variant is loaded.

#### Constraint Usage Examples

```csharp
// Integer ID constraint
.WithPage<PostDetail>("{id:int}", page => { })

// GUID constraint
.WithPage<ProjectDetail>("{projectId:guid}", page => { })

// Alpha slug constraint (ForgingBlazor extension)
.WithPage<PostDetail>("{slug:alpha}", page => { })

// Language constraint - automatically sets culture for segment and children
.WithSegment<LocalizedPostList>("{culture:lang}", segment =>
{
    // All nested elements inherit the matched culture (e.g., "de-DE", "en", "fr-FR")
    segment.WithPage<PostDetail>("{slug:alpha}", page => { });
})

// Boolean optional parameter
.WithPage<FeatureToggle>("{enabled:bool?}", page => { })
```

#### Optional Parameters

Route parameters can be made optional by appending `?`:

```csharp
.WithPage<PostDetail>("{slug:alpha?}", page => { })
```

### 4.5 Maximum Nesting Depth

Segment nesting is limited to ensure maintainable URL structures:

| Level          | Depth | Behavior                                                      |
| -------------- | ----- | ------------------------------------------------------------- |
| **Normal**     | 1-4   | No warnings or errors                                         |
| **Soft Limit** | 5-7   | Analyzer Warning (FB0010) + Runtime Warning in logs           |
| **Hard Limit** | 8+    | Analyzer Error (FB0011) + Runtime `InvalidOperationException` |

```
Root
├── posts (depth 1)
│   ├── category (depth 2)
│   │   └── featured (depth 3)
│   │       └── archived (depth 4)
│   │           └── year (depth 5)      ← Soft Limit Warning
│   │               └── month (depth 6)
│   │                   └── day (depth 7)
│   │                       └── item (depth 8)  ← Hard Limit Error
```

---

## 5. Content Storage Structure

### 5.1 Base Content Path

The `{storage-root}/` represents the root directory of the configured storage provider(s). Multiple storage providers can be configured, each with its own root path.

```
{storage-root}/
├── _index.md           # Root index (Home page content)
├── _index.de-DE.md     # Root index (German)
├── posts/
│   ├── _index.md       # Segment index (Posts list)
│   ├── _index.de-DE.md # Segment index (German)
│   ├── hello-world/
│   │   ├── index.md    # Page content
│   │   └── index.de-DE.md
│   └── another-post.md # Alternative: single file
└── about/
    └── _index.md       # About page
```

> **Note:** Assets (images, CSS, etc.) are **not stored within content folders**. See §5.4 Asset Provider for asset handling.

### 5.2 Content Lookup Rules

The content resolver follows a priority-based lookup sequence for each content type. The lookup applies the **culture fallback chain** at each level.

#### 5.2.1 Root Pages (Home)

For a requested culture `de-DE`, the lookup order is:

| Priority | Pattern                          | Description                           |
| -------- | -------------------------------- | ------------------------------------- |
| 1        | `{storage-root}/_index.de-DE.md` | Full culture code (region-specific)   |
| 2        | `{storage-root}/_index.de.md`    | Two-letter code (language only)       |
| 3        | `{storage-root}/_index.en-US.md` | Default culture (full)                |
| 4        | `{storage-root}/_index.en.md`    | Default culture (two-letter)          |
| 5        | `{storage-root}/_index.md`       | No culture suffix (ultimate fallback) |

#### 5.2.2 Segment Index Pages

For a requested culture `de-DE`, the lookup order is:

| Priority | Pattern                                         | Description                           |
| -------- | ----------------------------------------------- | ------------------------------------- |
| 1        | `{storage-root}/{segment-path}/_index.de-DE.md` | Full culture code (region-specific)   |
| 2        | `{storage-root}/{segment-path}/_index.de.md`    | Two-letter code (language only)       |
| 3        | `{storage-root}/{segment-path}/_index.en-US.md` | Default culture (full)                |
| 4        | `{storage-root}/{segment-path}/_index.en.md`    | Default culture (two-letter)          |
| 5        | `{storage-root}/{segment-path}/_index.md`       | No culture suffix (ultimate fallback) |

> **Note:** `{segment-path}` represents the full nested path, e.g., `posts/category` for nested segments.

#### 5.2.3 Detail Pages

For a requested culture `de-DE`, the lookup order is:

| Priority | Pattern                                                   | Description                          |
| -------- | --------------------------------------------------------- | ------------------------------------ |
| 1        | `{storage-root}/{segment-path}/{pageSlug}/index.de-DE.md` | Folder: Full culture code            |
| 2        | `{storage-root}/{segment-path}/{pageSlug}.de-DE.md`       | File: Full culture code              |
| 3        | `{storage-root}/{segment-path}/{pageSlug}/index.de.md`    | Folder: Two-letter code              |
| 4        | `{storage-root}/{segment-path}/{pageSlug}.de.md`          | File: Two-letter code                |
| 5        | `{storage-root}/{segment-path}/{pageSlug}/index.en-US.md` | Folder: Default culture (full)       |
| 6        | `{storage-root}/{segment-path}/{pageSlug}.en-US.md`       | File: Default culture (full)         |
| 7        | `{storage-root}/{segment-path}/{pageSlug}/index.en.md`    | Folder: Default culture (two-letter) |
| 8        | `{storage-root}/{segment-path}/{pageSlug}.en.md`          | File: Default culture (two-letter)   |
| 9        | `{storage-root}/{segment-path}/{pageSlug}/index.md`       | Folder: No culture suffix            |
| 10       | `{storage-root}/{segment-path}/{pageSlug}.md`             | File: No culture suffix              |

> **Note:** `{segment-path}` represents the full nested path from root to the containing segment.

### 5.3 Culture Codes

Culture codes follow the BCP 47 standard and support both full codes and two-letter variants.

| Format              | Example | Description                                         |
| ------------------- | ------- | --------------------------------------------------- |
| **Full Code**       | `de-DE` | Language + Region (e.g., German as used in Germany) |
| **Two-Letter Code** | `de`    | Language only (e.g., German)                        |

#### Culture Fallback Chain

The fallback chain applies in the following order (case-insensitive):

```
{requested-culture} → {two-letter} → {default-culture} → {default-two-letter} → {no-suffix}
```

**Example for `de-DE`:**

```
de-DE → de → en-US → en → (no suffix)
```

**Example for `fr`:**

```
fr → en-US → en → (no suffix)
```

| Property              | Value                                                                        |
| --------------------- | ---------------------------------------------------------------------------- |
| **Default Culture**   | `en-US`                                                                      |
| **Case Sensitivity**  | Case-insensitive (`de-DE`, `de-de`, `DE-DE` are equivalent)                  |
| **Fallback Behavior** | Full chain: specific → two-letter → default → default-two-letter → no suffix |

### 5.4 Asset Provider

Assets are managed via a separate `IAssetProvider` interface with dedicated storage directory.

| Principle               | Description                                                      |
| ----------------------- | ---------------------------------------------------------------- |
| **Separate Storage**    | Assets stored in dedicated directory, not within content folders |
| **Dedicated Interface** | `IAssetProvider` separate from `IContentStorageProvider`         |
| **CDN Support**         | Built-in CDN integration for production environments             |
| **Optimization**        | Image optimization for size, format, and compression             |

#### Asset Storage Structure

```
{asset-root}/                    # Separate from {storage-root}
├── images/
│   ├── hero.png
│   ├── logo.svg
│   └── posts/
│       ├── hello-world-hero.png
│       └── another-post-diagram.svg
├── css/
│   └── custom.css
└── fonts/
    └── custom-font.woff2
```

#### Interface Definition

```csharp
public interface IAssetProvider
{
    /// <summary>
    /// Resolves an asset URL, optionally applying CDN and optimization.
    /// </summary>
    ValueTask<string> ResolveUrlAsync(
        string assetPath,
        AssetOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves asset content.
    /// </summary>
    ValueTask<AssetResult?> GetAssetAsync(
        string assetPath,
        CancellationToken cancellationToken = default);
}

public record AssetOptions
{
    public int? Width { get; init; }
    public int? Height { get; init; }
    public ImageFormat? Format { get; init; }  // WebP, AVIF, PNG, JPG
    public int? Quality { get; init; }         // 1-100
}
```

#### Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Assets.RootPath = "./assets";

    // CDN Configuration
    options.Assets.Cdn.Enabled = true;
    options.Assets.Cdn.BaseUrl = "https://cdn.example.com";

    // Optimization Pipeline
    options.Assets.Optimization.Enabled = true;
    options.Assets.Optimization.DefaultFormat = ImageFormat.WebP;
    options.Assets.Optimization.DefaultQuality = 85;
    options.Assets.Optimization.GenerateResponsiveSizes = true;
});
```

#### Markdown Usage

```markdown
![Hero Image](/images/posts/hello-world-hero.png)
```

#### Rendered Output (with CDN + Optimization)

```html
<img
  src="https://cdn.example.com/images/posts/hello-world-hero.webp?w=800&q=85"
  srcset="
    https://cdn.example.com/images/posts/hello-world-hero.webp?w=400&q=85   400w,
    https://cdn.example.com/images/posts/hello-world-hero.webp?w=800&q=85   800w,
    https://cdn.example.com/images/posts/hello-world-hero.webp?w=1200&q=85 1200w
  "
  alt="Hero Image"
/>
```

| Aspect               | Behavior                                                 |
| -------------------- | -------------------------------------------------------- |
| **Relative Paths**   | Not supported - use absolute paths from asset root       |
| **Culture Fallback** | Assets do not follow culture fallback (language-neutral) |
| **Caching**          | CDN handles caching with cache-busting via query params  |
| **Security**         | Path validation prevents directory traversal             |

---

## 6. Property Inheritance

Properties can be inherited through the routing hierarchy or defined locally.

### 6.1 Inheritable Properties

| Property      | Default Value       | Inheritance Behavior                        |
| ------------- | ------------------- | ------------------------------------------- |
| `Culture`     | `en-US`             | Inherited from parent if not explicitly set |
| `Author`      | `null`              | Inherited from parent if not explicitly set |
| `ContentType` | `ContentDescriptor` | Inherited from root configuration           |

### 6.2 Non-Inheritable Properties

| Property      | Description                            |
| ------------- | -------------------------------------- |
| `Slug`        | Unique identifier for the page         |
| `Title`       | Page-specific title                    |
| `Description` | Page-specific description              |
| `Summary`     | Page-specific summary                  |
| `PublishDate` | Date when content becomes visible      |
| `ExpiryDate`  | Date when content is no longer visible |

---

## 7. Storage Providers

### 7.1 Storage Provider Interface

```csharp
public interface IContentStorageProvider
{
    /// <summary>
    /// Checks if content exists at the specified path.
    /// </summary>
    ValueTask<bool> ExistsAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves content from the specified path.
    /// </summary>
    ValueTask<ContentResult?> GetContentAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all content items under the specified path.
    /// </summary>
    IAsyncEnumerable<ContentMetadata> ListContentAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves content to the specified path.
    /// </summary>
    ValueTask SaveContentAsync(string path, ContentData content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes content at the specified path.
    /// </summary>
    ValueTask DeleteContentAsync(string path, CancellationToken cancellationToken = default);
}
```

### 7.2 Physical FileSystem Provider

**Package:** `ForgingBlazor` (built-in)

```csharp
services.AddForgingBlazorStorage(options =>
{
    options.UsePhysicalFileSystem(config =>
    {
        config.RootPath = "./content";
        config.WatchForChanges = true;  // Enable file system watcher
    });
});
```

### 7.3 Azure Blob Storage Provider

**Package:** `ForgingBlazor.Providers.AzureBlob` (separate library)

```csharp
services.AddForgingBlazorStorage(options =>
{
    options.UseAzureBlobStorage(config =>
    {
        config.ConnectionString = "...";
        config.ContainerName = "content";
    });
});
```

---

## 8. Content Synchronization

### 8.1 Synchronization Scenarios

| Scenario                 | Source              | Target              | Trigger          |
| ------------------------ | ------------------- | ------------------- | ---------------- |
| **Local Development**    | Physical FileSystem | -                   | Immediate        |
| **Scheduled Publishing** | Physical FileSystem | Azure Blob Storage  | Scheduled job    |
| **Content Migration**    | Azure Blob Storage  | Physical FileSystem | Manual/On-demand |

### 8.2 Synchronization Interface

```csharp
public interface IContentSynchronizer
{
    /// <summary>
    /// Synchronizes content from source to target storage.
    /// </summary>
    ValueTask SynchronizeAsync(
        IContentStorageProvider source,
        IContentStorageProvider target,
        SyncOptions options,
        CancellationToken cancellationToken = default);
}

public class SyncOptions
{
    /// <summary>
    /// Paths to include in synchronization (glob patterns supported).
    /// </summary>
    public IReadOnlyList<string> IncludePaths { get; init; } = ["**/*"];

    /// <summary>
    /// Paths to exclude from synchronization (glob patterns supported).
    /// </summary>
    public IReadOnlyList<string> ExcludePaths { get; init; } = [];

    /// <summary>
    /// Whether to delete target content that doesn't exist in source.
    /// </summary>
    public bool DeleteOrphans { get; init; } = false;

    /// <summary>
    /// Whether to overwrite existing content in target.
    /// </summary>
    public bool OverwriteExisting { get; init; } = true;
}
```

### 8.3 Scheduled Content Publishing

Content can be marked for scheduled publishing using frontmatter:

```yaml
---
title: "Upcoming Feature Announcement"
publishDate: 2026-02-01T09:00:00Z
draft: false
---
```

The synchronization job should:

1. Scan content for `publishDate` in frontmatter
2. Transfer content to production storage when `publishDate <= now`

### 8.4 Synchronization Location

Synchronization is part of the **Core library** (`ForgingBlazor`) with the following design principles:

| Principle               | Implementation                                                                            |
| ----------------------- | ----------------------------------------------------------------------------------------- |
| **Fast Exit**           | Single storage provider configured → methods return immediately without processing        |
| **Lazy Initialization** | Synchronization infrastructure only initialized when multiple providers are registered    |
| **No-Op Pattern**       | `IContentSynchronizer` uses a no-op implementation when synchronization is not applicable |

### 8.5 Draft Content Handling

Drafts are managed via frontmatter `draft: true` flag:

```yaml
---
title: "Work in Progress Article"
draft: true
---
```

| Environment     | `draft: true` Content  |
| --------------- | ---------------------- |
| **Development** | Visible (configurable) |
| **Production**  | Hidden by default      |

```csharp
services.AddForgingBlazor(options =>
{
    // Show drafts in development
    options.Content.ShowDrafts = builder.Environment.IsDevelopment();

    // Or explicitly control
    options.Content.ShowDrafts = true;  // Show all drafts
});
```

Content with `draft: true` is hidden regardless of `PublishDate`.

### 8.6 Content Caching Strategy

Content caching uses a **Static Assets** approach. When a request arrives, content is loaded from storage, parsed/rendered, and saved as a static file in `wwwroot`. Subsequent requests are served directly by the web server.

```
Request → Static Asset exists?
            ├── Yes → Serve from wwwroot (web server)
            └── No  → Load from Storage
                        → Parse Markdown
                        → Render HTML
                        → Save to wwwroot
                        → Return Response
```

#### Static Asset Path Mapping

| Request URL                | Static Asset Path                            |
| -------------------------- | -------------------------------------------- |
| `/posts/hello-world`       | `wwwroot/posts/hello-world/index.html`       |
| `/de-DE/posts/hello-world` | `wwwroot/de-DE/posts/hello-world/index.html` |
| `/posts`                   | `wwwroot/posts/index.html`                   |

#### Cache Invalidation Triggers

| Trigger                 | Action                                             |
| ----------------------- | -------------------------------------------------- |
| **Content Change**      | Remove specific static asset for changed content   |
| **Layout Change**       | Clear all static assets (full regeneration)        |
| **Application Restart** | Clear all static assets                            |
| **Manual Invalidation** | API to clear specific paths or all assets          |
| **Storage Sync**        | Clear affected static assets after synchronization |

### 8.7 Cache Exclusion for Dynamic Content

Dynamic content is excluded from static caching via `ExcludeFromCache()` method on routing builders.

| Builder           | Method               | Effect                                             |
| ----------------- | -------------------- | -------------------------------------------------- |
| `IRootBuilder`    | `ExcludeFromCache()` | Excludes entire site from caching (development)    |
| `ISegmentBuilder` | `ExcludeFromCache()` | Excludes segment and all nested pages from caching |
| `IPageBuilder`    | `ExcludeFromCache()` | Excludes specific page from caching                |

#### Inheritance Behavior

| Level       | Behavior                                             |
| ----------- | ---------------------------------------------------- |
| **Root**    | All segments and pages are excluded                  |
| **Segment** | Segment index and all nested pages/segments excluded |
| **Page**    | Only the specific page is excluded                   |

#### Configuration Example

```csharp
var builder = ForgingBlazorApplication
    .CreateDefaultBuilder(args)
    .ConfigureRouting<App, Home>(root =>
    {
        _ = root
            .WithErrorPage<Error>()
            .WithNotFoundPage<NotFound>()
            // Segment with dynamic content - exclude entire segment
            .WithSegment<Dashboard>("dashboard", segment => _ = segment
                .ExcludeFromCache()  // Dashboard and all nested pages excluded
                .WithPage<UserProfile>("{userId:int}", page => { })
            )
            // Mixed segment - only specific pages excluded
            .WithSegment<PostList>("posts", segment => _ = segment
                .WithPage<PostDetail>("{slug:alpha}", page => { })  // Cached
                .WithPage<PostComments>("{slug:alpha}/comments", page => _ = page
                    .ExcludeFromCache()  // Dynamic comments - excluded
                )
            );
    });
```

#### Use Cases

| Scenario         | Configuration                | Reason                |
| ---------------- | ---------------------------- | --------------------- |
| User Dashboard   | `segment.ExcludeFromCache()` | Personalized content  |
| Comments Section | `page.ExcludeFromCache()`    | Frequently changing   |
| Real-time Data   | `page.ExcludeFromCache()`    | Live counters, status |
| Development Mode | `root.ExcludeFromCache()`    | Disable all caching   |

---

## 9. Content Descriptor

### 9.1 Current ContentDescriptor

```csharp
public record ContentDescriptor
{
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? LinkTitle { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string? Author { get; set; }
}
```

### 9.2 Extended ContentDescriptor

`ContentDescriptor` remains a **record** type to provide value-based equality, immutability, and support for `with` expressions. Custom content types can inherit via `record CustomDescriptor : ContentDescriptor`.

```csharp
public record ContentDescriptor
{
    // Existing properties
    public string? Slug { get; set; }
    public string? Title { get; set; }
    public string? LinkTitle { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string? Author { get; set; }

    // Extended properties
    public string? Culture { get; set; }
    public DateTimeOffset? PublishDate { get; set; }
    public DateTimeOffset? ExpiryDate { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public bool IsDraft { get; set; }
    public IReadOnlyList<string>? Tags { get; set; }
    public IReadOnlyList<string>? Categories { get; set; }
    public int? Weight { get; set; }  // For ordering
    public string? FeaturedImage { get; set; }
}
```

#### Custom Content Type Example

```csharp
// Custom Content Type for Blog Posts
public record BlogPostDescriptor : ContentDescriptor
{
    public string? Series { get; init; }
    public int? ReadingTimeMinutes { get; init; }
}

// Usage in routing
.WithSegment<PostList>("posts", segment => _ = segment
    .WithPage<PostDetail, BlogPostDescriptor>("{slug:alpha}", page => { })
)
```

### 9.3 TimeProvider Usage

Per [Decision 2026-01-21: DateTimeOffset and TimeProvider Usage](../decisions/2026-01-21-datetimeoffset-and-timeprovider-usage.md), `TimeProvider` must be provided via constructor injection.

```csharp
public class ContentResolver : IContentResolver
{
    private readonly TimeProvider _timeProvider;

    public ContentResolver(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public ValueTask<ResolvedContent<TContentType>?> ResolveAsync<TContentType>(
        RouteContext context,
        CancellationToken cancellationToken = default)
        where TContentType : ContentDescriptor, new()
    {
        var now = _timeProvider.GetUtcNow();

        // Content visibility check
        if (descriptor.PublishDate.HasValue && descriptor.PublishDate.Value > now)
        {
            return ValueTask.FromResult<ResolvedContent<TContentType>?>(null);
        }

        // Content expiry check
        if (descriptor.ExpiryDate.HasValue && descriptor.ExpiryDate.Value <= now)
        {
            return ValueTask.FromResult<ResolvedContent<TContentType>?>(null);
        }

        // ...
    }
}
```

#### Service Registration

```csharp
// Default registration (uses TimeProvider.System)
services.AddForgingBlazor();

// Custom TimeProvider for testing
services.AddSingleton<TimeProvider>(new FakeTimeProvider(startDateTime));
services.AddForgingBlazor();
```

| Operation              | TimeProvider Usage              |
| ---------------------- | ------------------------------- |
| **Content Visibility** | `PublishDate <= GetUtcNow()`    |
| **Content Expiry**     | `ExpiryDate > GetUtcNow()`      |
| **Draft Scheduling**   | Sync job uses same TimeProvider |

---

## 10. Routing Resolution Pipeline

### 10.1 Request Flow

```
HTTP Request
    │
    ▼
┌─────────────────────┐
│   Route Matching    │  Match against configured segments
└─────────────────────┘
    │
    ▼
┌─────────────────────┐
│  Content Lookup     │  Apply lookup rules from §5.2
└─────────────────────┘
    │
    ▼
┌─────────────────────┐
│  Culture Resolution │  Resolve culture from URL, header, or default
└─────────────────────┘
    │
    ▼
┌─────────────────────┐
│  Content Loading    │  Load and parse Markdown/frontmatter
└─────────────────────┘
    │
    ▼
┌─────────────────────┐
│  Template Binding   │  Bind content to Blazor component
└─────────────────────┘
    │
    ▼
HTTP Response
```

### 10.2 Content Resolution Service

```csharp
public interface IContentResolver
{
    /// <summary>
    /// Resolves content for the given route and culture.
    /// </summary>
    ValueTask<ResolvedContent<TContentType>?> ResolveAsync<TContentType>(
        RouteContext context,
        CancellationToken cancellationToken = default)
        where TContentType : ContentDescriptor, new();
}

public record RouteContext
{
    public required string Path { get; init; }
    public required string? Culture { get; init; }
    public required IReadOnlyDictionary<string, string> RouteValues { get; init; }
}

public record ResolvedContent<TContentType>
    where TContentType : ContentDescriptor
{
    public required TContentType Descriptor { get; init; }
    public required string RawContent { get; init; }
    public required string RenderedHtml { get; init; }
    public required string StoragePath { get; init; }
}
```

---

## 11. Future Considerations

The following topics are documented as separate draft specifications for future implementation:

| Topic                         | Specification                                                  | Description                                           |
| ----------------------------- | -------------------------------------------------------------- | ----------------------------------------------------- |
| **Layout Change Detection**   | [layout-change-detection.md](./layout-change-detection.md)     | Automatic cache invalidation when layouts change      |
| **Route Conflict Resolution** | [route-conflict-resolution.md](./route-conflict-resolution.md) | Handling conflicts between content and `@page` routes |
| **Content Authorization**     | [content-authorization.md](./content-authorization.md)         | Authorization support for content-based routes        |
| **Content SEO**               | [content-seo.md](./content-seo.md)                             | Automatic `<head>` injection from frontmatter         |

### 11.1 Potential Features (Not in Scope)

The following features are documented as separate draft specifications for potential future implementation:

| Topic                     | Specification                                                      | Description                                        |
| ------------------------- | ------------------------------------------------------------------ | -------------------------------------------------- |
| **Content Aliases**       | [content-aliases.md](./content-aliases.md)                         | Alternative URL paths and canonical URLs           |
| **Content Feeds**         | [content-feeds.md](./content-feeds.md)                             | Content syndication via RSS 2.0 and Atom 1.0 feeds |
| **Content Sitemap**       | [content-sitemap.md](./content-sitemap.md)                         | XML sitemaps for search engine optimization        |
| **Content Search**        | [content-search-indexing.md](./content-search-indexing.md)         | Full-text search and indexing                      |
| **Image Optimization**    | [image-optimization-pipeline.md](./image-optimization-pipeline.md) | Automatic image processing and optimization        |
| **Markdown Extensions**   | [markdown-extensions.md](./markdown-extensions.md)                 | Syntax highlighting, diagrams, math expressions    |
| **Content Relationships** | [content-relationships.md](./content-relationships.md)             | Related posts, series, and content linking         |
| **Content Taxonomy**      | [content-taxonomy.md](./content-taxonomy.md)                       | Tags, categories, and custom taxonomies            |

---

## Appendix A: Glossary

| Term                   | Definition                                                                 |
| ---------------------- | -------------------------------------------------------------------------- |
| **Segment**            | A URL path component that groups related pages (e.g., "posts", "projects") |
| **Page**               | A leaf node in the routing tree representing a single content item         |
| **Root**               | The top-level routing configuration                                        |
| **Culture**            | A BCP 47 language tag (e.g., "en-US", "de-DE")                             |
| **Frontmatter**        | YAML metadata at the beginning of Markdown files                           |
| **Slug**               | A URL-friendly identifier derived from the content title or explicitly set |
| **Storage Provider**   | An abstraction for content storage (FileSystem, Azure Blob, etc.)          |
| **Storage Root**       | The root directory (`{storage-root}/`) of a configured storage provider    |
| **ForgingRouter**      | ForgingBlazor's router component replacing Blazor's `Router`               |
| **ForgingRouteView**   | ForgingBlazor's route view component replacing Blazor's `RouteView`        |
| **Template Component** | A Blazor component used to render content (without `@page` directive)      |

---

## Appendix B: Example Content Structure

```
{storage-root}/                         # Root of the storage provider
├── _index.md                           # Home page (en-US)
├── _index.de-DE.md                     # Home page (German)
│
├── posts/                              # Blog segment
│   ├── _index.md                       # Posts list page
│   ├── _index.de-DE.md                 # Posts list (German)
│   │
│   ├── getting-started/                # Post with folder structure
│   │   ├── index.md                    # Content
│   │   └── index.de-DE.md              # German translation
│   │
│   ├── quick-tip.md                    # Post as single file
│   └── quick-tip.de-DE.md              # German translation
│
├── projects/                           # Projects segment
│   ├── _index.md
│   ├── forgingblazor/
│   │   └── index.md
│   └── another-project.md
│
└── about/                              # About page (single page segment)
    └── _index.md

{asset-root}/                           # Separate asset storage
├── images/
│   ├── logo.svg
│   └── posts/
│       ├── getting-started-hero.png
│       └── quick-tip-diagram.svg
└── css/
    └── custom.css
```

---

## Appendix C: Frontmatter Schema

```yaml
---
# Required
title: "Page Title"

# Optional - Routing
slug: "custom-slug" # Override URL slug
linkTitle: "Short Title" # Title for navigation/links
weight: 100 # Ordering weight (lower = first)

# Optional - Metadata
description: "Page description for SEO"
summary: "Brief summary for listings"
author: "Author Name"
tags: ["tag1", "tag2"]
categories: ["category1"]

# Optional - Publishing
draft: false # true = not published
publishDate: 2026-01-24T09:00:00Z
expiryDate: 2027-01-24T09:00:00Z
lastModified: 2026-01-24T12:00:00Z

# Optional - Display
featuredImage: "./images/hero.png"
showToc: true # Show table of contents
showComments: false # Enable comments
---
# Content starts here...
```

---

## Revision History

| Version | Date       | Author                                        | Changes                                                                         |
| ------- | ---------- | --------------------------------------------- | ------------------------------------------------------------------------------- |
| 0.1.0   | 2026-01-24 | [Martin Stühmer](https://github.com/samtrion) | Initial draft                                                                   |
| 0.2.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Router components (ForgingRouter, ForgingRouteView)                             |
| 1.0.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Finalized specification, extracted future topics to separate specs, created ADR |
