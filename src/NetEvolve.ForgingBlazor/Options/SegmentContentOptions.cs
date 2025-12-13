namespace NetEvolve.ForgingBlazor.Options;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Provides configuration options for content segments in a ForgingBlazor application.
/// </summary>
/// <typeparam name="TPageType">
/// The page type that inherits from <see cref="PageBase"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// This sealed class represents the configuration for content pages that belong to a specific segment.
/// It inherits all base configuration options from <see cref="ContentOptionsBase{TPageType}"/> without adding additional properties.
/// </para>
/// <para>
/// Use this options class when configuring content segments through the AddSegment extension method.
/// </para>
/// </remarks>
/// <seealso cref="ContentOptionsBase{TPageType}"/>
/// <seealso cref="PageBase"/>
public sealed class SegmentContentOptions<TPageType> : ContentOptionsBase<TPageType>
    where TPageType : PageBase;
