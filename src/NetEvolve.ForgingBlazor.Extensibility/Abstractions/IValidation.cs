namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Defines a validation service that validates a specific page type within a validation context.
/// </summary>
/// <typeparam name="TPageType">The type of page to validate. Must derive from <see cref="PageBase"/>.</typeparam>
/// <remarks>
/// Implementations of this interface perform validation logic on pages and report issues through the provided <see cref="IValidationContext{TPageType}"/>.
/// </remarks>
public interface IValidation<TPageType>
    where TPageType : PageBase
{
    /// <summary>
    /// Validates the specified page and reports any errors or warnings to the validation context.
    /// </summary>
    /// <param name="page">The page instance to validate.</param>
    /// <param name="context">The validation context used to record errors and warnings discovered during validation.</param>
    void Validate(TPageType page, IValidationContext<TPageType> context);
}
