namespace NetEvolve.ForgingBlazor.Content.Validation;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NetEvolve.ForgingBlazor.Routing;

/// <summary>
/// Internal class for validating content structure: segment <c>_index.md</c> files exist, page content files exist in default culture.
/// </summary>
internal sealed class ContentStructureValidation
{
    private readonly RouteRegistry _routeRegistry;
    private readonly string _contentBasePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentStructureValidation"/> class.
    /// </summary>
    /// <param name="routeRegistry">The route registry containing all registered routes.</param>
    /// <param name="contentBasePath">The base path for content files.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="routeRegistry"/> or <paramref name="contentBasePath"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="contentBasePath"/> is empty or whitespace.</exception>
    public ContentStructureValidation(RouteRegistry routeRegistry, string contentBasePath)
    {
        ArgumentNullException.ThrowIfNull(routeRegistry);
        ArgumentNullException.ThrowIfNull(contentBasePath);

        if (string.IsNullOrWhiteSpace(contentBasePath))
        {
            throw new ArgumentException("Content base path cannot be empty or whitespace.", nameof(contentBasePath));
        }

        _routeRegistry = routeRegistry;
        _contentBasePath = contentBasePath;
    }

    /// <summary>
    /// Validates that all required content files exist.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous validation operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required content files are missing.</exception>
    public async Task ValidateAsync(CancellationToken cancellationToken = default)
    {
        var allRoutes = _routeRegistry.GetAll();
        var missingFiles = new List<string>();

        foreach (var route in allRoutes)
        {
            // Check if route requires an index file (segment)
            if (route.PathPattern.Contains('/', StringComparison.Ordinal) || route.PaginationSettings is not null)
            {
                var indexPath = Path.Combine(_contentBasePath, route.PathPattern, "_index.md");
                if (!File.Exists(indexPath))
                {
                    missingFiles.Add(indexPath);
                }
            }
            // Otherwise, check for page content file
            else
            {
                var pagePath = Path.Combine(_contentBasePath, $"{route.PathPattern}.md");
                if (!File.Exists(pagePath))
                {
                    missingFiles.Add(pagePath);
                }
            }

            await Task.Yield(); // Allow cancellation
            cancellationToken.ThrowIfCancellationRequested();
        }

        if (missingFiles.Count > 0)
        {
            var fileList = string.Join(Environment.NewLine, missingFiles);
            throw new InvalidOperationException($"Missing required content files:{Environment.NewLine}{fileList}");
        }
    }
}
