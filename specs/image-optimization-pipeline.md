# Image Optimization Pipeline

> **Status:** Draft  
> **Version:** 0.1.0  
> **Created:** 2026-01-25  
> **Last Modified:** 2026-01-25  
> **Author:** [Martin Stühmer](https://github.com/samtrion)  
> **Parent:** [Routing Specification](./routing.md)

---

## 1. Overview

This specification defines the image optimization pipeline for ForgingBlazor.

Image optimization automatically processes images to reduce file size, convert to modern formats, and generate responsive variants, improving page load performance.

---

## 2. Problem Statement

Images often account for the majority of page weight. ForgingBlazor's asset provider (see [Routing Specification §5.4](./routing.md#54-asset-provider)) defines the interface but lacks detailed optimization pipeline specification.

### 2.1 Requirements

| Requirement              | Description                                  |
| ------------------------ | -------------------------------------------- |
| **Format Conversion**    | Convert to WebP, AVIF for modern browsers    |
| **Responsive Images**    | Generate multiple sizes for srcset           |
| **Quality Optimization** | Compress while maintaining visual quality    |
| **Lazy Processing**      | On-demand or build-time processing           |
| **Caching**              | Cache optimized images to avoid reprocessing |

---

## 3. Goals

| Goal              | Description                                    |
| ----------------- | ---------------------------------------------- |
| **Performance**   | Reduce image payload by 50-80%                 |
| **Compatibility** | Fallback to original format for older browsers |
| **Automation**    | Zero-config optimization for common scenarios  |
| **Quality**       | Preserve visual quality at reduced file sizes  |

---

## 4. Proposed Interface

```csharp
public interface IImageOptimizer
{
    ValueTask<OptimizedImage> OptimizeAsync(
        Stream sourceImage,
        ImageOptimizationOptions options,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<ResponsiveImage>> GenerateResponsiveSetAsync(
        Stream sourceImage,
        ResponsiveImageOptions options,
        CancellationToken cancellationToken = default);
}

public record ImageOptimizationOptions
{
    public ImageFormat TargetFormat { get; init; } = ImageFormat.WebP;
    public int Quality { get; init; } = 85;
    public int? MaxWidth { get; init; }
    public int? MaxHeight { get; init; }
    public bool PreserveMetadata { get; init; } = false;
}

public record ResponsiveImageOptions
{
    public IReadOnlyList<int> Widths { get; init; } = [400, 800, 1200, 1600];
    public ImageFormat Format { get; init; } = ImageFormat.WebP;
    public int Quality { get; init; } = 85;
}

public record OptimizedImage
{
    public required Stream Content { get; init; }
    public required ImageFormat Format { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required long SizeBytes { get; init; }
}

public enum ImageFormat
{
    Original,
    WebP,
    AVIF,
    PNG,
    JPEG
}
```

---

## 5. Configuration

```csharp
services.AddForgingBlazor(options =>
{
    options.Assets.Optimization.Enabled = true;
    options.Assets.Optimization.DefaultFormat = ImageFormat.WebP;
    options.Assets.Optimization.DefaultQuality = 85;
    options.Assets.Optimization.GenerateResponsiveSizes = true;
    options.Assets.Optimization.ResponsiveWidths = [400, 800, 1200, 1600];
    options.Assets.Optimization.AvifSupport = true;
    options.Assets.Optimization.CacheOptimizedImages = true;
});
```

---

## 6. Processing Modes

### 6.1 On-Demand Processing

Images processed when first requested, then cached.

| Aspect       | Description                            |
| ------------ | -------------------------------------- |
| **Pros**     | No build step, always up-to-date       |
| **Cons**     | First request slower, server CPU usage |
| **Use Case** | Dynamic content, development           |

### 6.2 Build-Time Processing

Images pre-processed during build/publish.

| Aspect       | Description                              |
| ------------ | ---------------------------------------- |
| **Pros**     | Zero runtime overhead, CDN-friendly      |
| **Cons**     | Longer build times, storage for variants |
| **Use Case** | Static sites, production deployment      |

---

## 7. Markdown Integration

```markdown
![Hero Image](/images/hero.png)
```

Rendered output with responsive images:

```html
<picture>
  <source
    type="image/avif"
    srcset="/images/hero.avif?w=400 400w, /images/hero.avif?w=800 800w, /images/hero.avif?w=1200 1200w"
  />
  <source
    type="image/webp"
    srcset="/images/hero.webp?w=400 400w, /images/hero.webp?w=800 800w, /images/hero.webp?w=1200 1200w"
  />
  <img src="/images/hero.png" alt="Hero Image" loading="lazy" decoding="async" />
</picture>
```

---

## 8. Open Questions

- Should we support image processing libraries like ImageSharp or SkiaSharp?
- How to handle animated images (GIF, animated WebP)?
- Should we support blur-up placeholder images?

---

## Revision History

| Version | Date       | Author                                        | Changes       |
| ------- | ---------- | --------------------------------------------- | ------------- |
| 0.1.0   | 2026-01-25 | [Martin Stühmer](https://github.com/samtrion) | Initial draft |
