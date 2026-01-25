# Route Conflict Resolution

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines how ForgingBlazor handles conflicts between content-based routes and standard Blazor `@page` routes.

---

## 2. Problem Statement

ForgingBlazor supports both:

1. **Content Routes:** Defined via `ConfigureRouting<TApp, THome>()`
2. **Blazor Routes:** Defined via `@page` directives in Razor components

When both route types could match the same URL path, a conflict resolution strategy is required.

### 2.1 Current Behavior

Standard Blazor behavior applies. ForgingBlazor routes take precedence as documented in [Routing Specification §3.5.1](./routing.md#351-forgingrouter).

### 2.2 Conflict Scenarios

| Scenario           | Content Route   | Blazor Route             | Conflict    |
| ------------------ | --------------- | ------------------------ | ----------- |
| **Exact Match**    | `/posts`        | `@page "/posts"`         | Yes         |
| **Parameterized**  | `/posts/{slug}` | `@page "/posts/{id}"`    | Yes         |
| **Admin Override** | `/posts`        | `@page "/posts"` (Admin) | Intentional |

---

## 3. Goals

| Goal                     | Description                              |
| ------------------------ | ---------------------------------------- |
| **Predictable Behavior** | Clear, documented resolution rules       |
| **Developer Control**    | Allow explicit override when needed      |
| **Early Detection**      | Detect conflicts at startup, not runtime |
| **Graceful Handling**    | No runtime exceptions for conflicts      |

---

## 4. Proposed Options

### 4.1 ForgingBlazor First (Default)

Content routes always have priority.

```
Request → ForgingBlazor Route Exists?
            ├── Yes → Serve Content
            └── No  → Check Blazor Routes
```

#### Advantages

| Advantage           | Description                            |
| ------------------- | -------------------------------------- |
| **Content-centric** | Content management is primary use case |
| **Predictable**     | Single clear rule                      |

#### Disadvantages

| Disadvantage    | Description                           |
| --------------- | ------------------------------------- |
| **No Override** | Cannot use Blazor route for same path |

### 4.2 Blazor First (Explicit Override)

Blazor `@page` routes take priority (explicit override capability).

```
Request → Blazor Route with [RouteOverride] Exists?
            ├── Yes → Serve Blazor Component
            └── No  → Check ForgingBlazor Routes
```

```csharp
// Attribute to mark explicit override
[AttributeUsage(AttributeTargets.Class)]
public class RouteOverrideAttribute : Attribute { }
```

```razor
@page "/posts"
@attribute [RouteOverride]

<!-- This component overrides ForgingBlazor content route -->
```

#### Advantages

| Advantage       | Description                      |
| --------------- | -------------------------------- |
| **Flexibility** | Admin pages can override content |
| **Explicit**    | Override must be intentional     |

#### Disadvantages

| Disadvantage   | Description                        |
| -------------- | ---------------------------------- |
| **Complexity** | Additional attribute to understand |

### 4.3 Configurable Priority

Priority controlled via configuration.

```csharp
services.AddForgingBlazor(options =>
{
    options.Routing.ConflictResolution = RouteConflictResolution.ForgingBlazorFirst;
    // or
    options.Routing.ConflictResolution = RouteConflictResolution.BlazorFirst;
});
```

#### Advantages

| Advantage    | Description                  |
| ------------ | ---------------------------- |
| **Flexible** | Application chooses behavior |

#### Disadvantages

| Disadvantage     | Description                       |
| ---------------- | --------------------------------- |
| **Inconsistent** | Different apps behave differently |

### 4.4 Error on Conflict

Startup error on detected conflicts.

```csharp
// During application startup
throw new RouteConflictException(
    "Route '/posts' is defined both as ForgingBlazor content route and Blazor @page route. " +
    "Use [RouteOverride] attribute or remove one of the routes.");
```

#### Advantages

| Advantage           | Description               |
| ------------------- | ------------------------- |
| **Safe**            | No ambiguous behavior     |
| **Early Detection** | Fails fast during startup |

#### Disadvantages

| Disadvantage | Description                   |
| ------------ | ----------------------------- |
| **Strict**   | Requires immediate resolution |

---

## 5. Open Questions

| #   | Question                                                   | Context                          |
| --- | ---------------------------------------------------------- | -------------------------------- |
| 1   | Should ForgingBlazor routes always win by default?         | Current assumption               |
| 2   | Should conflicts be detected at startup and logged/warned? | Early warning system             |
| 3   | Can `@page` routes explicitly override content routes?     | Admin scenario                   |
| 4   | How to handle parameterized route conflicts?               | `/posts/{slug}` vs `/posts/{id}` |

---

## 6. Related Specifications

- [Routing Specification](./routing.md) - Parent specification
- [Routing Specification §3.5](./routing.md#35-router-components) - Router Components

---

## Revision History

| Version | Date       | Author  | Changes       |
| ------- | ---------- | ------- | ------------- |
| 0.1.0   | 2026-01-25 | Initial | Initial draft |
