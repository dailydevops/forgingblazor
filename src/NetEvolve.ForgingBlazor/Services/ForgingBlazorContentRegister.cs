namespace NetEvolve.ForgingBlazor.Services;

using NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Provides the default implementation of <see cref="IContentRegister"/> for the ForgingBlazor framework.
/// </summary>
/// <remarks>
/// This sealed class serves as the marker service for content registration within the ForgingBlazor dependency injection container.
/// It is used to identify and filter content registration services during the startup phase.
/// </remarks>
/// <seealso cref="IContentRegister"/>
internal sealed class ForgingBlazorContentRegister : IContentRegister { }
