namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Marker interface used to identify services registered during the startup phase of the application.
/// This interface is used internally to filter and distinguish command-related services from other
/// registered services in the dependency injection container. Services implementing this interface
/// are excluded from the command execution pipeline after the startup phase completes.
/// </summary>
/// <remarks>
/// This is an intentionally empty marker interface that serves as a metadata tag for service filtering.
/// It should be implemented by services that are only needed during application initialization and
/// should not be exposed as command-related services during runtime execution.
/// </remarks>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "As Designed.")]
public interface IStartUpMarker;
