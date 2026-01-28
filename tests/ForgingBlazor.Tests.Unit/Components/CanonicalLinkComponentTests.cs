namespace NetEvolve.ForgingBlazor.Tests.Unit.Components;

using System;
using System.Threading.Tasks;
using Bunit;
using NetEvolve.ForgingBlazor.Components;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class CanonicalLinkComponentTests : IDisposable
{
    private readonly Bunit.TestContext _testContext = new();

    [Test]
    public async Task Render_WithCanonicalUrl_OutputsLinkTag()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "https://example.com/page")
        );

        var linkElement = cut.Find("link[rel='canonical']");
        var href = linkElement.GetAttribute("href");

        _ = await Assert.That(href).IsEqualTo("https://example.com/page");
    }

    [Test]
    public async Task Render_WithoutCanonicalUrl_UsesCurrentUrl()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, null)
        );

        var linkElement = cut.Find("link[rel='canonical']");
        var href = linkElement.GetAttribute("href");

        _ = await Assert.That(href).IsNotNull();
    }

    [Test]
    public async Task Render_WithEmptyCanonicalUrl_UsesCurrentUrl()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "")
        );

        var linkElement = cut.Find("link[rel='canonical']");
        var href = linkElement.GetAttribute("href");

        _ = await Assert.That(href).IsNotNull();
    }

    [Test]
    public async Task Render_WithWhitespaceCanonicalUrl_UsesCurrentUrl()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "   ")
        );

        var linkElement = cut.Find("link[rel='canonical']");
        var href = linkElement.GetAttribute("href");

        _ = await Assert.That(href).IsNotNull();
    }

    [Test]
    public async Task Render_LinkElement_HasCorrectRelAttribute()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "https://example.com")
        );

        var linkElement = cut.Find("link");
        var rel = linkElement.GetAttribute("rel");

        _ = await Assert.That(rel).IsEqualTo("canonical");
    }

    [Test]
    public async Task ParameterUpdate_ChangesCanonicalUrl()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "https://example.com/page1")
        );

        var linkElement = cut.Find("link[rel='canonical']");
        var initialHref = linkElement.GetAttribute("href");

        cut.SetParametersAndRender(parameters => parameters.Add(c => c.CanonicalUrl, "https://example.com/page2"));

        var updatedHref = cut.Find("link[rel='canonical']").GetAttribute("href");

        using (Assert.Multiple())
        {
            _ = await Assert.That(initialHref).IsEqualTo("https://example.com/page1");
            _ = await Assert.That(updatedHref).IsEqualTo("https://example.com/page2");
        }
    }

    [Test]
    public async Task Render_WithSpecialCharactersInUrl_PreservesUrl()
    {
        var specialUrl = "https://example.com/page?id=123&name=test%20value#section";
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, specialUrl)
        );

        var linkElement = cut.Find("link[rel='canonical']");
        var href = linkElement.GetAttribute("href");

        _ = await Assert.That(href).IsEqualTo(specialUrl);
    }

    [Test]
    public async Task Render_WithRelativeUrl_PreservesUrl()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "/blog/my-post")
        );

        var linkElement = cut.Find("link[rel='canonical']");
        var href = linkElement.GetAttribute("href");

        _ = await Assert.That(href).IsEqualTo("/blog/my-post");
    }

    [Test]
    public async Task Render_OutputsLinkTagInHead()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "https://example.com")
        );

        var linkElements = cut.FindAll("link[rel='canonical']");

        _ = await Assert.That(linkElements.Count).IsGreaterThanOrEqualTo(1);
    }

    [Test]
    public async Task Render_ParameterPropertyCanBeNull()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>();

        // When rendered without parameters, OnParametersSet sets it to current URL
        _ = await Assert.That(cut.Instance.CanonicalUrl).IsNotEqualTo(string.Empty);
    }

    [Test]
    public async Task Render_ParameterPropertyCanBeSet()
    {
        var cut = _testContext.RenderComponent<CanonicalLinkComponent>(parameters =>
            parameters.Add(c => c.CanonicalUrl, "https://example.com")
        );

        _ = await Assert.That(cut.Instance.CanonicalUrl).IsEqualTo("https://example.com");
    }

    public void Dispose() => _testContext.Dispose();
}
