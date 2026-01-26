# ForgingBlazor.Storage.AzureBlob

[![NuGet Version](https://img.shields.io/nuget/v/NetEvolve.ForgingBlazor.Storage.AzureBlob.svg)](https://www.nuget.org/packages/NetEvolve.ForgingBlazor.Storage.AzureBlob/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NetEvolve.ForgingBlazor.Storage.AzureBlob.svg)](https://www.nuget.org/packages/NetEvolve.ForgingBlazor.Storage.AzureBlob/)
[![License](https://img.shields.io/github/license/dailydevops/ForgingBlazor.svg)](https://github.com/dailydevops/ForgingBlazor/blob/main/LICENSE)

Azure Blob Storage provider for ForgingBlazor Dynamic Content Routing system. Enables publishing Markdown content and assets to Azure Blob Storage for production deployments.

## Features

- **Production-Ready Storage**: Store Markdown content files in Azure Blob Storage for scalable production deployments
- **Asset Management**: Separate storage provider for images, PDFs, and other media files
- **Publishing Workflow**: Designate Azure Blob as publishing target for draft-to-production workflows
- **Connection Security**: Supports connection strings and Azure Identity authentication
- **Automatic Content Type Detection**: Proper MIME types for Markdown files and common asset formats
- **Culture-Aware Paths**: Blob paths organized by segment and culture for efficient lookups

## Installation

### NuGet Package Manager

```powershell
Install-Package NetEvolve.ForgingBlazor.Storage.AzureBlob
```

### .NET CLI

```bash
dotnet add package NetEvolve.ForgingBlazor.Storage.AzureBlob
```

### PackageReference

```xml
<PackageReference Include="NetEvolve.ForgingBlazor.Storage.AzureBlob" />
```

## Quick Start

```csharp
using NetEvolve.ForgingBlazor.Storage.AzureBlob;

var builder = WebApplication.CreateBuilder(args);

// Add ForgingBlazor with Azure Blob Storage
builder.Services.AddForgingBlazor(app =>
{
    app.AddContentStorage(storage =>
    {
        storage.UseAzureBlobStorage(options =>
        {
            options.WithConnectionString(builder.Configuration["AzureStorage:ConnectionString"]!)
                   .WithContainerName("content")
                   .AsPublishingTarget();
        });
    });

    app.AddAssetStorage(storage =>
    {
        storage.UseAzureBlobStorage(options =>
        {
            options.WithConnectionString(builder.Configuration["AzureStorage:ConnectionString"]!)
                   .WithContainerName("assets");
        });
    });
});

var app = builder.Build();
app.Run();
```

## Usage

### Basic Content Storage Configuration

Configure Azure Blob Storage for content files:

```csharp
app.AddContentStorage(storage =>
{
    storage.UseAzureBlobStorage(options =>
    {
        options.WithConnectionString("DefaultEndpointsProtocol=https;AccountName=myaccount;...")
               .WithContainerName("content");
    });
});
```

### Asset Storage Configuration

Configure separate container for assets:

```csharp
app.AddAssetStorage(storage =>
{
    storage.UseAzureBlobStorage(options =>
    {
        options.WithConnectionString("DefaultEndpointsProtocol=https;AccountName=myaccount;...")
               .WithContainerName("assets");
    });
});
```

### Publishing Target

Designate Azure Blob Storage as the publishing destination for production content:

```csharp
storage.UseAzureBlobStorage(options =>
{
    options.WithConnectionString(connectionString)
           .WithContainerName("content")
           .AsPublishingTarget(); // This storage receives published content
});
```

### Development and Production Workflow

Use FileSystem storage for development and Azure Blob for production:

```csharp
if (builder.Environment.IsDevelopment())
{
    // Draft storage with hot-reload
    app.AddContentStorage(storage =>
    {
        storage.UseFileSystem(options =>
        {
            options.WithBasePath("content")
                   .WatchForChanges();
        });
    });
}
else
{
    // Production storage
    app.AddContentStorage(storage =>
    {
        storage.UseAzureBlobStorage(options =>
        {
            options.WithConnectionString(builder.Configuration["AzureStorage:ConnectionString"]!)
                   .WithContainerName("content")
                   .AsPublishingTarget();
        });
    });
}
```

### Blob Path Structure

Content is stored with the following path structure:

```
content/
├── posts/
│   ├── en-US/
│   │   ├── getting-started.md
│   │   └── advanced-guide.md
│   ├── de-DE/
│   │   └── getting-started.md
│   └── fr-FR/
│       └── getting-started.md
└── pages/
    └── en-US/
        ├── about.md
        └── contact.md

assets/
├── images/
│   ├── hero.jpg
│   └── logo.png
└── documents/
    └── whitepaper.pdf
```

### Azure Identity Authentication

For production scenarios, use Azure Managed Identity instead of connection strings:

```csharp
// Note: Azure Identity support planned for future release
// Current version supports connection string authentication
```

## Configuration Options

### IAzureBlobStorageOptions

| Method                        | Description                                     | Required |
| ----------------------------- | ----------------------------------------------- | -------- |
| `WithConnectionString(string)` | Sets the Azure Storage connection string       | Yes      |
| `WithContainerName(string)`    | Sets the blob container name                    | Yes      |
| `AsPublishingTarget()`         | Marks this storage as the publishing target     | No       |

## Content Type Mapping

The provider automatically sets Content-Type headers for blobs:

| Extension               | Content-Type          |
| ----------------------- | --------------------- |
| `.md`                   | `text/markdown`       |
| `.jpg`, `.jpeg`         | `image/jpeg`          |
| `.png`                  | `image/png`           |
| `.gif`                  | `image/gif`           |
| `.svg`                  | `image/svg+xml`       |
| `.webp`                 | `image/webp`          |
| `.pdf`                  | `application/pdf`     |
| `.json`                 | `application/json`    |
| `.xml`                  | `application/xml`     |
| Others                  | `application/octet-stream` |

## Security Best Practices

### Connection String Storage

**Never** hardcode connection strings in source code. Use secure configuration:

```csharp
// appsettings.json
{
  "AzureStorage": {
    "ConnectionString": "..." // Store in Azure Key Vault or User Secrets
  }
}

// User Secrets (Development)
dotnet user-secrets set "AzureStorage:ConnectionString" "DefaultEndpointsProtocol=https;..."

// Azure App Service (Production)
// Configure connection string as Application Setting
```

### Container Access Control

Set appropriate access levels on blob containers:

- **Content Container**: Private (default) - accessed only by the application
- **Assets Container**: Blob-level read access if serving assets directly from blob URLs

## Requirements

- .NET 10.0 or later
- Azure Storage Account (General Purpose v2 recommended)
- ForgingBlazor.Extensibility package
- Azure.Storage.Blobs package (installed automatically)

## Limitations

- Current version supports connection string authentication only
- Azure Identity (Managed Identity) support planned for future release
- Blob metadata not currently utilized for caching headers

## Related Documentation

- [ForgingBlazor Documentation](../ForgingBlazor/README.md)
- [Dynamic Content Routing Specification](../../spec/spec-design-dynamic-content-routing.md)
- [Azure Blob Storage Documentation](https://learn.microsoft.com/azure/storage/blobs/)

## Contributing

See the [Contributing Guide](../../CONTRIBUTING.md) for details on how to contribute to this project.

## License

This project is licensed under the MIT License - see the [LICENSE](../../LICENSE) file for details.
