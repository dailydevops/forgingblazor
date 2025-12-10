namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Internal marker interface used to identify the content registration service during the startup phase.
/// This interface is used to register the core content registration implementation in the dependency
/// injection container and enables the filtering mechanism to exclude startup-specific services from
/// the command execution pipeline.
/// </summary>
/// <remarks>
/// This is an intentionally empty marker interface that serves as a metadata tag for service identification.
/// It should be implemented by the internal content registration service that handles content-related
/// initialization during application startup.
/// </remarks>
internal interface IContentRegister { }
