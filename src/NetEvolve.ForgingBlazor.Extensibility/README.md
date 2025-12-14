# NetEvolve.ForgingBlazor.Extensibility

[![NuGet](https://img.shields.io/nuget/v/NetEvolve.ForgingBlazor.Extensibility?logo=nuget)](https://www.nuget.org/packages/NetEvolve.ForgingBlazor.Extensibility/)
[![NuGet](https://img.shields.io/nuget/dt/NetEvolve.ForgingBlazor.Extensibility?logo=nuget)](https://www.nuget.org/packages/NetEvolve.ForgingBlazor.Extensibility/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/dailydevops/forgingblazor/blob/main/LICENSE)

## Overview

`NetEvolve.ForgingBlazor.Extensibility` is the core extensibility package for the ForgingBlazor framework, providing the fundamental abstractions, interfaces, and base models required to build custom content processors and extend the framework's functionality. This package serves as the foundation for creating plugins, content handlers, and custom page types within the ForgingBlazor ecosystem.

## Purpose

The extensibility package is designed to enable developers to:

* **Extend Content Types**: Create custom page types and blog post structures by inheriting from base models
* **Build Custom Processors**: Implement content registration and validation logic for specialized workflows
* **Develop Plugins**: Create reusable components that integrate seamlessly with the ForgingBlazor application lifecycle
* **Define Custom Segments**: Implement specialized builders for content organization and navigation
* **Integrate Services**: Transfer and manage services across application scopes efficiently

## Key Features

### Abstraction Layer

Provides a comprehensive set of interfaces that define the contracts for:

* **Application Lifecycle** (`IApplication`, `IApplicationBuilder`): Core application initialization and execution
* **Content Management** (`IContentRegister`, `IContentRegistration`): Content discovery and registration
* **Validation Framework** (`IValidation`, `IValidationContext`): Content and configuration validation
* **Page Properties**: Modular interfaces for blog post metadata including authors, tags, categories, series, publication dates, and summaries
* **Segment Building** (`ISegmentBuilder`, `IBlogSegmentBuilder`): Content organization and navigation structure

### Base Models

Robust foundation classes for content creation:

* **`PageBase`**: Abstract record providing core page properties (slug, title, link title) with YAML serialization support
* **`BlogPostBase`**: Extends `PageBase` with blogging-specific properties including publication dates, author attribution, and tagging capabilities

### Service Management

Advanced dependency injection utilities:

* **Service Transfer**: Efficiently migrate services between different service providers while maintaining scope integrity
* **Startup Isolation**: Automatic filtering of startup-specific services to prevent lifecycle conflicts
* **Scope Management**: Support for hierarchical service provider structures

## Target Frameworks

* **.NET 9.0** (`net9.0`)
* **.NET 10.0** (`net10.0`)

## Dependencies

This package leverages the following dependencies:

* **Microsoft.AspNetCore.App**: Core ASP.NET functionality and web framework support
* **System.CommandLine**: Command-line interface infrastructure for extensible applications
* **YamlDotNet**: YAML serialization and deserialization for content metadata

## Installation

```bash
dotnet add package NetEvolve.ForgingBlazor.Extensibility
```

## Usage

### Creating a Custom Page Type

```csharp
using NetEvolve.ForgingBlazor.Extensibility.Models;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

public record CustomPage : PageBase, IPropertySummary
{
    public string? Summary { get; set; }
    
    // Add custom properties specific to your page type
    public string? CustomField { get; set; }
}
```

### Implementing a Custom Blog Post

```csharp
using NetEvolve.ForgingBlazor.Extensibility.Models;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

public record TutorialPost : BlogPostBase, IPropertySeries, IPropertyCategories
{
    public string? Series { get; set; }
    public IReadOnlySet<string>? Categories { get; set; }
    
    // Tutorial-specific properties
    public TimeSpan EstimatedDuration { get; set; }
    public string DifficultyLevel { get; set; } = "Beginner";
}
```

## Architecture Considerations

### Extensibility Points

The package provides multiple extension points:

1. **Page Models**: Inherit from `PageBase` or `BlogPostBase` to create custom content types
2. **Property Interfaces**: Implement property interfaces (`IPropertyAuthor`, `IPropertyTags`, etc.) for metadata composition
3. **Lifecycle Hooks**: Implement `IApplication` for custom application execution logic
4. **Validation Pipeline**: Add validators through `IValidation` for content integrity
5. **Content Registration**: Use `IContentRegister` for custom content discovery mechanisms

### Design Principles

* **Interface Segregation**: Modular property interfaces enable composition over inheritance
* **Immutability**: Record types encourage immutable data structures for thread safety
* **Separation of Concerns**: Clear boundaries between content models, validation, and registration
* **YAML-First**: Built-in support for YAML frontmatter in content files
* **Async-First**: All primary operations support asynchronous execution patterns

## Best Practices

1. **Use Records for Content Models**: Leverage C# record types for immutable, value-based content representations
2. **Implement Property Interfaces**: Compose page capabilities through interface implementation rather than deep inheritance
3. **Validate Early**: Implement `IValidation` to catch content issues during build time
4. **Document YAML Attributes**: Use `YamlMember` attributes with descriptions for clear metadata documentation
5. **Respect Cancellation**: Always honor `CancellationToken` parameters in async methods
6. **Keep Models Simple**: Avoid business logic in model classes; delegate to services or handlers

## Contributing

Contributions are welcome! Please ensure that:

* All code follows the project's established patterns and conventions
* New abstractions are well-documented with XML comments
* Changes maintain backward compatibility where possible
* Unit tests cover new functionality

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/dailydevops/forgingblazor/blob/main/LICENSE) file for details.

## Related Packages

* [**NetEvolve.ForgingBlazor**](https://www.nuget.org/packages/NetEvolve.ForgingBlazor): Main framework implementation consuming this extensibility package
* [**NetEvolve.ForgingBlazor.Logging**](https://www.nuget.org/packages/NetEvolve.ForgingBlazor.Logging): Logging extensions for the ForgingBlazor framework

---

**Made with ❤️ by the NetEvolve Team**