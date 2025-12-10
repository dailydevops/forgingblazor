namespace NetEvolve.ForgingBlazor.Services;

using System;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

internal class ContentRegistration<TPageType> : IContentRegistration
    where TPageType : PageBase
{
    public Type PageType { get; } = typeof(TPageType);
}
