namespace NetEvolve.ForgingBlazor.Tests.Unit.Storage;

using System.Diagnostics.CodeAnalysis;
using NetEvolve.ForgingBlazor;

/// <summary>
/// Test content descriptor for unit tests.
/// </summary>
internal sealed class TestContentDescriptor : ContentDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestContentDescriptor"/> class.
    /// </summary>
    [SetsRequiredMembers]
    public TestContentDescriptor()
    {
        Title = string.Empty;
        Slug = string.Empty;
    }
}
