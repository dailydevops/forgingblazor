namespace NetEvolve.ForgingBlazor.Services;

using System;
using NetEvolve.ForgingBlazor.Extensibility.Abstractions;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides the default implementation of <see cref="IContentRegistration"/> for registering the default page type.
/// </summary>
/// <typeparam name="TPageType">The default page type being registered, which must derive from <see cref="PageBase"/>.</typeparam>
/// <remarks>
/// This class is used to register the default page type when the application is configured with default pages.
/// It differs from <see cref="ContentRegistration{TPageType}"/> only semantically to identify it as the default registration.
/// </remarks>
/// <seealso cref="IContentRegistration"/>
/// <seealso cref="ContentRegistration{TPageType}"/>
internal class DefaultContentRegistration<TPageType> : IContentRegistration
    where TPageType : PageBase
{
    /// <summary>
    /// Gets the type of the default page registered with the ForgingBlazor framework.
    /// </summary>
    /// <value>
    /// A <see cref="Type"/> instance representing <typeparamref name="TPageType"/>.
    /// </value>
    public Type PageType { get; } = typeof(TPageType);
}
