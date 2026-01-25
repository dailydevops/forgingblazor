# Authorization for Content Routes

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines how ForgingBlazor integrates with ASP.NET Core authorization for content-based routes.

Blazor provides `AuthorizeRouteView` for protected routes. ForgingBlazor requires equivalent functionality for content-based routes.

---

## 2. Problem Statement

Content served via ForgingBlazor may require authorization:

- Premium content for subscribers
- Draft preview for editors
- Admin-only sections

### 2.1 Current Behavior

Standard Blazor authorization applies. No content-specific authorization support.

### 2.2 Limitations

| Limitation                 | Impact                                 |
| -------------------------- | -------------------------------------- |
| **No Frontmatter Support** | Cannot define authorization in content |
| **No Segment-level Auth**  | Cannot protect entire segments         |
| **No Role-based Content**  | Cannot show different content per role |

---

## 3. Goals

| Goal                         | Description                                |
| ---------------------------- | ------------------------------------------ |
| **ASP.NET Core Integration** | Compatible with Authorization Policies     |
| **Granular Control**         | Segment-level and page-level authorization |
| **Declarative**              | Authorization configurable in frontmatter  |
| **Fallback Handling**        | Clear behavior on unauthorized access      |

---

## 4. Proposed Options

### 4.1 ForgingAuthorizeRouteView

Separate component with authorization support.

```razor
<ForgingRouter AppAssembly="typeof(Program).Assembly">
    <Found Context="routeData">
        <ForgingAuthorizeRouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)">
            <NotAuthorized>
                <p>You are not authorized to view this content.</p>
            </NotAuthorized>
            <Authorizing>
                <p>Checking authorization...</p>
            </Authorizing>
        </ForgingAuthorizeRouteView>
    </Found>
</ForgingRouter>
```

#### Advantages

| Advantage             | Description                                |
| --------------------- | ------------------------------------------ |
| **Familiar**          | Mirrors `AuthorizeRouteView` pattern       |
| **Blazor-compatible** | Uses existing authorization infrastructure |

#### Disadvantages

| Disadvantage           | Description                              |
| ---------------------- | ---------------------------------------- |
| **Separate Component** | Two route view components to choose from |

### 4.2 Built-in Parameter

`ForgingRouteView` with optional `Authorize` parameter.

```razor
<ForgingRouteView
    RouteData="routeData"
    DefaultLayout="typeof(MainLayout)"
    AuthorizePolicy="PremiumContent">
    <NotAuthorized>
        <SubscriptionPrompt />
    </NotAuthorized>
</ForgingRouteView>
```

#### Advantages

| Advantage            | Description                            |
| -------------------- | -------------------------------------- |
| **Single Component** | No separate authorize component needed |
| **Optional**         | Authorization opt-in per usage         |

#### Disadvantages

| Disadvantage   | Description                         |
| -------------- | ----------------------------------- |
| **Complexity** | More parameters on single component |

### 4.3 Frontmatter-based Authorization

Authorization defined in content frontmatter.

```yaml
---
title: "Premium Article"
authorize: true
roles: ["Premium", "Admin"]
policy: "PremiumContentPolicy"
---
```

```csharp
public record ContentDescriptor
{
    // ... existing properties

    public bool Authorize { get; set; }
    public IReadOnlyList<string>? Roles { get; set; }
    public string? Policy { get; set; }
}
```

#### Advantages

| Advantage           | Description                      |
| ------------------- | -------------------------------- |
| **Content-centric** | Authorization lives with content |
| **Flexible**        | Per-content authorization rules  |

#### Disadvantages

| Disadvantage      | Description                          |
| ----------------- | ------------------------------------ |
| **Decentralized** | Authorization scattered across files |
| **Security Risk** | Content authors control access       |

### 4.4 Routing Builder Configuration

Authorization configured via routing builder.

```csharp
.WithSegment<PremiumList>("premium", segment => _ = segment
    .RequireAuthorization("PremiumPolicy")  // Segment-level
    .WithPage<PremiumDetail>("{slug}", page => _ = page
        .RequireAuthorization()  // Inherit from segment
    )
)
.WithSegment<MixedContent>("articles", segment => _ = segment
    .WithPage<PublicArticle>("{slug}", page => { })  // Public
    .WithPage<ProtectedArticle>("internal/{slug}", page => _ => page
        .RequireAuthorization("InternalPolicy")  // Page-level only
    )
)
```

#### Advantages

| Advantage        | Description                                  |
| ---------------- | -------------------------------------------- |
| **Centralized**  | All authorization in routing config          |
| **Type-safe**    | Compile-time policy name validation possible |
| **Hierarchical** | Segment policies inherited by pages          |

#### Disadvantages

| Disadvantage   | Description                          |
| -------------- | ------------------------------------ |
| **Code-based** | Cannot be changed without deployment |

### 4.5 Hybrid Approach

Combine routing builder and frontmatter.

```csharp
// Default authorization via routing
.WithSegment<PremiumList>("premium", segment => _ => segment
    .RequireAuthorization("DefaultPremiumPolicy")
)
```

```yaml
---
title: "Special Premium Article"
policy: "VIPOnlyPolicy" # Override segment policy
---
```

**Resolution Order:**

1. Frontmatter `policy` (if specified)
2. Frontmatter `roles` (if specified)
3. Frontmatter `authorize: true` with segment policy
4. Segment/Page routing builder policy
5. No authorization (public)

---

## 5. Open Questions

| #   | Question                                                      | Context                           |
| --- | ------------------------------------------------------------- | --------------------------------- |
| 1   | Should authorization be per-segment, per-page, or both?       | Granularity                       |
| 2   | How to integrate with ASP.NET Core Authorization Policies?    | Reuse existing infrastructure     |
| 3   | What happens on unauthorized access?                          | Redirect, 403, or custom content? |
| 4   | How to handle mixed public/protected content in same segment? | Flexible configuration            |
| 5   | Should content authors be able to define authorization?       | Security consideration            |

---

## 6. Related Specifications

- [Routing Specification](./routing.md) - Parent specification
- [Routing Specification §3.5.2](./routing.md#352-forgingrouteview) - ForgingRouteView

---

## 7. References

- [ASP.NET Core Blazor Authorization](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/)
- [AuthorizeRouteView](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.components.authorization.authorizerouteview)

---

## Revision History

| Version | Date       | Author  | Changes       |
| ------- | ---------- | ------- | ------------- |
| 0.1.0   | 2026-01-25 | Initial | Initial draft |
