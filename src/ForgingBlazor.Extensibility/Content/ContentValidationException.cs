namespace NetEvolve.ForgingBlazor;

using System;

/// <summary>
/// Represents an exception thrown when content validation fails.
/// </summary>
public sealed class ContentValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    public ContentValidationException()
        : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message explaining the validation failure.</param>
    public ContentValidationException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message explaining the validation failure.</param>
    /// <param name="innerException">The exception that caused this validation exception.</param>
    public ContentValidationException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentValidationException"/> class.
    /// </summary>
    /// <param name="fieldName">The name of the field that failed validation.</param>
    /// <param name="expectedType">The expected type for the field value.</param>
    /// <param name="actualValue">The actual value that was provided.</param>
    public ContentValidationException(string fieldName, Type expectedType, object? actualValue)
        : base(CreateMessage(fieldName, expectedType, actualValue))
    {
        ArgumentNullException.ThrowIfNull(fieldName);
        ArgumentNullException.ThrowIfNull(expectedType);

        FieldName = fieldName;
        ExpectedType = expectedType;
        ActualValue = actualValue;
    }

    private static string CreateMessage(string fieldName, Type expectedType, object? actualValue)
    {
        ArgumentNullException.ThrowIfNull(expectedType);
        return $"Content validation failed for field '{fieldName}'. Expected type '{expectedType.Name}', but received value: {actualValue ?? "null"}.";
    }

    /// <summary>
    /// Gets the name of the field that failed validation.
    /// </summary>
    public string? FieldName { get; }

    /// <summary>
    /// Gets the expected type for the field value.
    /// </summary>
    public Type? ExpectedType { get; }

    /// <summary>
    /// Gets the actual value that was provided.
    /// </summary>
    public object? ActualValue { get; }
}
