namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using System.Runtime.CompilerServices;
using NetEvolve.ForgingBlazor.Extensibility.Models;

/// <summary>
/// Represents a validation context that tracks validation state and diagnostics for a specific page type.
/// </summary>
public interface IValidationContext<TPageType>
    where TPageType : PageBase
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

    /// <summary>
    /// Validates a property value against a condition and records an error if the condition is met.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    /// <param name="propertyValue">
    /// A function that extracts the property value from the page instance.
    /// </param>
    /// <param name="condition">
    /// A predicate function that returns <see langword="true"/> if the property value represents an error condition.
    /// </param>
    /// <param name="errorMessage">
    /// The error message to record if the condition evaluates to <see langword="true"/>.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property being validated. This is automatically captured from the <paramref name="propertyValue"/> expression.
    /// </param>
    /// <returns>
    /// The same <see cref="IValidationContext{TPageType}"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// This method uses caller argument expression to automatically capture the property name from the lambda expression,
    /// providing clear diagnostic information without manual string literals.
    /// </remarks>
    IValidationContext<TPageType> ValidateError<TProperty>(
        Func<TPageType, TProperty> propertyValue,
        Func<TProperty, bool> condition,
        string errorMessage,
        [CallerArgumentExpression(nameof(propertyValue))] string? propertyName = null
    );

    /// <summary>
    /// Validates a property value against a condition and records a warning if the condition is met.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    /// <param name="propertyValue">
    /// A function that extracts the property value from the page instance.
    /// </param>
    /// <param name="condition">
    /// A predicate function that returns <see langword="true"/> if the property value represents a warning condition.
    /// </param>
    /// <param name="warningMessage">
    /// The warning message to record if the condition evaluates to <see langword="true"/>.
    /// </param>
    /// <param name="propertyName">
    /// The name of the property being validated. This is automatically captured from the <paramref name="propertyValue"/> expression.
    /// </param>
    /// <returns>
    /// The same <see cref="IValidationContext{TPageType}"/> instance for method chaining.
    /// </returns>
    /// <remarks>
    /// This method uses caller argument expression to automatically capture the property name from the lambda expression,
    /// providing clear diagnostic information without manual string literals.
    /// </remarks>
    IValidationContext<TPageType> ValidateWarning<TProperty>(
        Func<TPageType, TProperty> propertyValue,
        Func<TProperty, bool> condition,
        string warningMessage,
        [CallerArgumentExpression(nameof(propertyValue))] string? propertyName = null
    );
}
