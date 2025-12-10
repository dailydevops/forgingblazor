namespace NetEvolve.ForgingBlazor.Extensibility.Models;

using System.Collections.Generic;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

public abstract record BlogPostBase : PageBase, IPagePropertyPublishedOn, IPagePropertyAuthor, IPagePropertyTags
{
    public DateTimeOffset? PublishedOn { get; set; }

    public string Author { get; set; }

    public IReadOnlySet<string> Tags { get; set; }
}
