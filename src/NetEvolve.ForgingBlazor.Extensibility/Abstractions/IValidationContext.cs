namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

/// <summary>
/// Represents a validation context that tracks validation state and diagnostics for a specific page type.
/// </summary>
public interface IValidationContext
{
    /// <summary>
    /// Gets a value indicating whether the validation context contains any error-level diagnostics.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if one or more errors have been recorded during validation; otherwise, <see langword="false"/>.
    /// </value>
    bool HasErrors { get; }

    /// <summary>
    /// Gets a value indicating whether the validation context contains any warning-level diagnostics.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if one or more warnings have been recorded during validation; otherwise, <see langword="false"/>.
    /// </value>
    bool HasWarnings { get; }
}
