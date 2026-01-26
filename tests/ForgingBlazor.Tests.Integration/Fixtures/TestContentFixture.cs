namespace NetEvolve.ForgingBlazor.Tests.Integration.Fixtures;

using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Test fixture that provides sample Markdown content files for integration testing.
/// Creates temporary directory structure with multi-culture content, draft/published states, and expiration scenarios.
/// </summary>
public sealed class TestContentFixture : IAsyncDisposable
{
    private readonly string _baseDirectory;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestContentFixture"/> class.
    /// </summary>
    public TestContentFixture()
    {
        _baseDirectory = Path.Combine(Path.GetTempPath(), "ForgingBlazor.IntegrationTests", Guid.NewGuid().ToString());
        _ = Directory.CreateDirectory(_baseDirectory);
    }

    /// <summary>
    /// Gets the base directory path where test content is stored.
    /// </summary>
    public string BaseDirectory => _baseDirectory;

    /// <summary>
    /// Initializes the fixture with sample content files.
    /// </summary>
    public async Task InitializeAsync()
    {
        await CreateBlogContentAsync();
        await CreatePageContentAsync();
        await CreateMultiCultureContentAsync();
        await CreateDraftContentAsync();
        await CreateExpiredContentAsync();
    }

    /// <summary>
    /// Adds a custom content file to the test directory.
    /// </summary>
    /// <param name="relativePath">The relative path (including filename) where to save the content.</param>
    /// <param name="markdownContent">The Markdown content with frontmatter.</param>
    public async Task AddContentAsync(string relativePath, string markdownContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(markdownContent);

        var fullPath = Path.Combine(_baseDirectory, relativePath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        await File.WriteAllTextAsync(fullPath, markdownContent);
    }

    /// <summary>
    /// Removes a content file from the test directory.
    /// </summary>
    /// <param name="relativePath">The relative path to the file to remove.</param>
    public void RemoveContent(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        var fullPath = Path.Combine(_baseDirectory, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        await Task.Run(() =>
        {
            if (Directory.Exists(_baseDirectory))
            {
                Directory.Delete(_baseDirectory, recursive: true);
            }
        });

        _disposed = true;
    }

    private async Task CreateBlogContentAsync()
    {
        // Published blog post in English
        var publishedPost = """
            ---
            title: Getting Started with Blazor
            slug: getting-started
            publisheddate: 2026-01-20T10:00:00Z
            draft: false
            ---
            # Getting Started with Blazor

            This is a comprehensive guide to getting started with Blazor.

            ## Introduction

            Blazor is a web framework for building interactive web UIs.
            """;

        await AddContentAsync("blog/EN-US/getting-started.md", publishedPost);

        // Another published post
        var secondPost = """
            ---
            title: Advanced Routing Techniques
            slug: advanced-routing
            publisheddate: 2026-01-21T14:30:00Z
            draft: false
            ---
            # Advanced Routing Techniques

            Learn about advanced routing patterns in web applications.
            """;

        await AddContentAsync("blog/EN-US/advanced-routing.md", secondPost);

        // Index page for blog segment
        var indexPage = """
            ---
            title: Blog
            slug: blog-index
            publisheddate: 2026-01-15T09:00:00Z
            draft: false
            ---
            # Blog

            Welcome to our blog. Here you'll find articles about web development.
            """;

        await AddContentAsync("blog/EN-US/blog-index.md", indexPage);
    }

    private async Task CreatePageContentAsync()
    {
        // About page
        var aboutPage = """
            ---
            title: About Us
            slug: about
            publisheddate: 2026-01-10T08:00:00Z
            draft: false
            ---
            # About Us

            We are a team dedicated to building great web applications.
            """;

        await AddContentAsync("pages/EN-US/about.md", aboutPage);

        // Contact page
        var contactPage = """
            ---
            title: Contact
            slug: contact
            publisheddate: 2026-01-10T08:00:00Z
            draft: false
            ---
            # Contact Us

            Get in touch with our team.
            """;

        await AddContentAsync("pages/EN-US/contact.md", contactPage);
    }

    private async Task CreateMultiCultureContentAsync()
    {
        // German translation of getting started post
        var germanPost = """
            ---
            title: Erste Schritte mit Blazor
            slug: getting-started
            publisheddate: 2026-01-20T10:00:00Z
            draft: false
            ---
            # Erste Schritte mit Blazor

            Dies ist eine umfassende Anleitung für den Einstieg in Blazor.

            ## Einführung

            Blazor ist ein Web-Framework zum Erstellen interaktiver Web-UIs.
            """;

        await AddContentAsync("blog/DE-DE/getting-started.md", germanPost);

        // German about page
        var germanAbout = """
            ---
            title: Über uns
            slug: about
            publisheddate: 2026-01-10T08:00:00Z
            draft: false
            ---
            # Über uns

            Wir sind ein Team, das sich der Entwicklung großartiger Webanwendungen widmet.
            """;

        await AddContentAsync("pages/DE-DE/about.md", germanAbout);
    }

    private async Task CreateDraftContentAsync()
    {
        // Draft blog post (should not be visible in production)
        var draftPost = """
            ---
            title: Work in Progress
            slug: work-in-progress
            publisheddate: 2026-01-25T10:00:00Z
            draft: true
            ---
            # Work in Progress

            This post is still being written and should not be visible in production.
            """;

        await AddContentAsync("blog/EN-US/work-in-progress.md", draftPost);
    }

    private async Task CreateExpiredContentAsync()
    {
        // Expired blog post (past expiration date)
        var expiredPost = """
            ---
            title: Limited Time Offer
            slug: limited-offer
            publisheddate: 2026-01-15T10:00:00Z
            draft: false
            expiredat: 2026-01-18T23:59:59Z
            ---
            # Limited Time Offer

            This offer has expired and should no longer be visible.
            """;

        await AddContentAsync("blog/EN-US/limited-offer.md", expiredPost);

        // Future-dated post (not yet published)
        var futurePost = """
            ---
            title: Coming Soon
            slug: coming-soon
            publisheddate: 2026-02-15T10:00:00Z
            draft: false
            ---
            # Coming Soon

            This post is scheduled for future publication.
            """;

        await AddContentAsync("blog/EN-US/coming-soon.md", futurePost);
    }
}
