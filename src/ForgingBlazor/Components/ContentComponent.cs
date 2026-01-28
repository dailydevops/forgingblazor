namespace NetEvolve.ForgingBlazor.Components;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Base class for content components that receive resolved content via parameters.
/// Components deriving from this class receive a <see cref="ResolvedContent{TDescriptor}"/> parameter
/// containing the resolved content descriptor and associated metadata.
/// </summary>
/// <typeparam name="TDescriptor">The content descriptor type, which must inherit from <see cref="ContentDescriptor"/>.</typeparam>
/// <remarks>
/// This component follows the requirements:
/// <list type="bullet">
/// <item><description>REQ-CMP-007: Components implement <see cref="IComponent"/> (via <see cref="ComponentBase"/>).</description></item>
/// <item><description>REQ-CMP-009: Components must NOT define @page directive.</description></item>
/// <item><description>REQ-CMP-010: Components must NOT define @layout directive.</description></item>
/// <item><description>REQ-CMP-011: Components receive <see cref="ResolvedContent{TDescriptor}"/> as parameter.</description></item>
/// </list>
/// </remarks>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "As required.")]
public abstract class ContentComponent<TDescriptor> : ComponentBase
    where TDescriptor : ContentDescriptor
{
    /// <summary>
    /// Gets or sets the resolved content containing the content descriptor and associated metadata.
    /// </summary>
    [Parameter]
    public ResolvedContent<TDescriptor> ResolvedContent { get; set; } = default!;

    /// <summary>
    /// Gets the content descriptor from the resolved content.
    /// </summary>
    protected TDescriptor Content => ResolvedContent.Descriptor;
}
