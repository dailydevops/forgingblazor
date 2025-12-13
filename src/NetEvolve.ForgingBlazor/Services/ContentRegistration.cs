namespace NetEvolve.ForgingBlazor.Services;

using System;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides a generic implementation of <see cref="IContentRegistration"/> for custom page types.
/// </summary>
/// <typeparam name="TPageType">The page type being registered, which must derive from <see cref="PageBase"/>.</typeparam>
/// <remarks>
/// This class allows registration of custom page types with the ForgingBlazor framework.
/// It serves as a reusable registration container for any page type that inherits from <see cref="PageBase"/>.
/// </remarks>
/// <seealso cref="IContentRegistration"/>
/// <seealso cref="DefaultContentRegistration{TPageType}"/>
internal sealed class ContentRegistration<TPageType> : IContentRegistration
    where TPageType : PageBase
{
    /// <summary>
    /// Gets the type of the page registered with the ForgingBlazor framework.
    /// </summary>
    /// <value>
    /// A <see cref="Type"/> instance representing <typeparamref name="TPageType"/>.
    /// </value>
    public Type PageType { get; } = typeof(TPageType);
}
