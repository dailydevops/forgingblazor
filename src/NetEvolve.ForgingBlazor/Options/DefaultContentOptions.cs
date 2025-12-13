namespace NetEvolve.ForgingBlazor.Options;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides configuration options for default content (pages) in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TPageType">
/// The page type that inherits from <see cref="PageBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This sealed class represents the configuration for default content pages that don't belong to a specific segment or blog section.
/// It inherits all base configuration options from <see cref="ContentOptionsBase{TPageType}"/> without adding additional properties.
/// </para>
/// <para>
/// Use this options class when configuring default pages through the AddDefaultContent extension method.
/// </para>
/// </remarks>
/// <seealso cref="ContentOptionsBase{TPageType}"/>
/// <seealso cref="PageBase"/>
public sealed class DefaultContentOptions<TPageType> : ContentOptionsBase<TPageType>
    where TPageType : PageBase;
