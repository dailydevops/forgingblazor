namespace NetEvolve.ForgingBlazor.Tests.Unit.Validation;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NetEvolve.ForgingBlazor.Content.Validation;
using NetEvolve.ForgingBlazor.Pagination;
using NetEvolve.ForgingBlazor.Routing;
using TUnit.Assertions.Extensions;
using TUnit.Core;

/// <summary>
/// Unit tests for <see cref="ContentStructureValidation"/>.
/// Tests content directory structure validation including required index files,
/// page content files, and proper file organization.
/// </summary>
public sealed class ContentStructureValidationTests
{
    [Test]
    public async Task Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var contentBasePath = Path.Combine(Path.GetTempPath(), "content");

        // Act
        var validation = new ContentStructureValidation(routeRegistry, contentBasePath);

        // Assert
        _ = await Assert.That(validation).IsNotNull();
    }

    [Test]
    public void Constructor_WithNullRouteRegistry_ThrowsArgumentNullException()
    {
        // Arrange
        var contentBasePath = Path.Combine(Path.GetTempPath(), "content");

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "routeRegistry",
            () => _ = new ContentStructureValidation(null!, contentBasePath)
        );
    }

    [Test]
    public void Constructor_WithNullContentBasePath_ThrowsArgumentNullException()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();

        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(
            "contentBasePath",
            () => _ = new ContentStructureValidation(routeRegistry, null!)
        );
    }

    [Test]
    public void Constructor_WithEmptyContentBasePath_ThrowsArgumentException()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(
            "contentBasePath",
            () => _ = new ContentStructureValidation(routeRegistry, string.Empty)
        );
    }

    [Test]
    public void Constructor_WithWhitespaceContentBasePath_ThrowsArgumentException()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();

        // Act & Assert
        _ = Assert.Throws<ArgumentException>(
            "contentBasePath",
            () => _ = new ContentStructureValidation(routeRegistry, "   ")
        );
    }

    [Test]
    public async Task ValidateAsync_WithNoRoutes_CompletesSuccessfully()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            await validation.ValidateAsync().ConfigureAwait(false);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithValidPageRoute_CompletesSuccessfully()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var route = new RouteDefinition("about", typeof(object), null, null, null, null);
            routeRegistry.Register("about", route);

            var contentFile = Path.Combine(tempDir, "about.md");
            await File.WriteAllTextAsync(contentFile, "# About").ConfigureAwait(false);

            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            await validation.ValidateAsync().ConfigureAwait(false);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithMissingPageFile_ThrowsInvalidOperationException()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var route = new RouteDefinition("about", typeof(object), null, null, null, null);
            routeRegistry.Register("about", route);

            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validation.ValidateAsync().ConfigureAwait(false)
            );

            using (Assert.Multiple())
            {
                _ = await Assert.That(exception).IsNotNull();
                _ = await Assert.That(exception.Message).Contains("Missing required content files");
                _ = await Assert.That(exception.Message).Contains("about.md");
            }
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithValidSegmentRoute_CompletesSuccessfully()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var route = new RouteDefinition("blog/posts", typeof(object), null, null, null, null);
            routeRegistry.Register("blog/posts", route);

            var segmentDir = Path.Combine(tempDir, "blog", "posts");
            _ = Directory.CreateDirectory(segmentDir);
            var indexFile = Path.Combine(segmentDir, "_index.md");
            await File.WriteAllTextAsync(indexFile, "# Posts").ConfigureAwait(false);

            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            await validation.ValidateAsync().ConfigureAwait(false);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithMissingSegmentIndexFile_ThrowsInvalidOperationException()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var route = new RouteDefinition("blog/posts", typeof(object), null, null, null, null);
            routeRegistry.Register("blog/posts", route);

            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validation.ValidateAsync().ConfigureAwait(false)
            );

            using (Assert.Multiple())
            {
                _ = await Assert.That(exception).IsNotNull();
                _ = await Assert.That(exception.Message).Contains("Missing required content files");
                _ = await Assert.That(exception.Message).Contains("_index.md");
            }
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithPaginatedRoute_RequiresIndexFile()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var paginationSettings = new PaginationSettings(10, PaginationUrlFormat.Numeric, null);
            var route = new RouteDefinition("articles", typeof(object), null, null, paginationSettings, null);
            routeRegistry.Register("articles", route);

            var segmentDir = Path.Combine(tempDir, "articles");
            _ = Directory.CreateDirectory(segmentDir);
            var indexFile = Path.Combine(segmentDir, "_index.md");
            await File.WriteAllTextAsync(indexFile, "# Articles").ConfigureAwait(false);

            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            await validation.ValidateAsync().ConfigureAwait(false);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithMultipleRoutes_ValidatesAll()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var route1 = new RouteDefinition("about", typeof(object), null, null, null, null);
            var route2 = new RouteDefinition("contact", typeof(object), null, null, null, null);
            routeRegistry.Register("about", route1);
            routeRegistry.Register("contact", route2);

            await File.WriteAllTextAsync(Path.Combine(tempDir, "about.md"), "# About").ConfigureAwait(false);
            await File.WriteAllTextAsync(Path.Combine(tempDir, "contact.md"), "# Contact").ConfigureAwait(false);

            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            await validation.ValidateAsync().ConfigureAwait(false);
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithMultipleMissingFiles_IncludesAllInException()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            var route1 = new RouteDefinition("about", typeof(object), null, null, null, null);
            var route2 = new RouteDefinition("contact", typeof(object), null, null, null, null);
            routeRegistry.Register("about", route1);
            routeRegistry.Register("contact", route2);

            var validation = new ContentStructureValidation(routeRegistry, tempDir);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await validation.ValidateAsync().ConfigureAwait(false)
            );

            using (Assert.Multiple())
            {
                _ = await Assert.That(exception).IsNotNull();
                _ = await Assert.That(exception.Message).Contains("about.md");
                _ = await Assert.That(exception.Message).Contains("contact.md");
            }
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Test]
    public async Task ValidateAsync_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        var routeRegistry = new RouteRegistry();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(tempDir);

        try
        {
            for (var i = 0; i < 100; i++)
            {
                var route = new RouteDefinition($"page{i}", typeof(object), null, null, null, null);
                routeRegistry.Register($"page{i}", route);
            }

            var validation = new ContentStructureValidation(routeRegistry, tempDir);
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync().ConfigureAwait(false);

            // Act & Assert
            _ = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await validation.ValidateAsync(cts.Token).ConfigureAwait(false)
            );
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
