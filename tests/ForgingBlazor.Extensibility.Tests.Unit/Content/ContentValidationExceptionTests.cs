namespace NetEvolve.ForgingBlazor.Extensibility.Tests.Unit.Content;

using System;
using global::NetEvolve.ForgingBlazor;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public sealed class ContentValidationExceptionTests
{
    [Test]
    public async Task Constructor_Default_CreatesException()
    {
        var exception = new ContentValidationException();

        using (Assert.Multiple())
        {
            _ = await Assert.That(exception).IsNotNull();
            _ = await Assert.That(exception.FieldName).IsNull();
            _ = await Assert.That(exception.ExpectedType).IsNull();
            _ = await Assert.That(exception.ActualValue).IsNull();
        }
    }

    [Test]
    public async Task Constructor_WithMessage_SetsMessage()
    {
        const string message = "Test validation error";

        var exception = new ContentValidationException(message);

        using (Assert.Multiple())
        {
            _ = await Assert.That(exception.Message).IsEqualTo(message);
            _ = await Assert.That(exception.FieldName).IsNull();
            _ = await Assert.That(exception.ExpectedType).IsNull();
            _ = await Assert.That(exception.ActualValue).IsNull();
        }
    }

    [Test]
    public async Task Constructor_WithMessageAndInnerException_SetsBoth()
    {
        const string message = "Test validation error";
        var inner = new InvalidOperationException("Inner error");

        var exception = new ContentValidationException(message, inner);

        using (Assert.Multiple())
        {
            _ = await Assert.That(exception.Message).IsEqualTo(message);
            _ = await Assert.That(exception.InnerException).IsEqualTo(inner);
            _ = await Assert.That(exception.FieldName).IsNull();
            _ = await Assert.That(exception.ExpectedType).IsNull();
            _ = await Assert.That(exception.ActualValue).IsNull();
        }
    }

    [Test]
    public async Task Constructor_WithFieldNameExpectedTypeActualValue_SetsAllProperties()
    {
        const string fieldName = "TestField";
        var expectedType = typeof(string);
        const int actualValue = 123;

        var exception = new ContentValidationException(fieldName, expectedType, actualValue);

        using (Assert.Multiple())
        {
            _ = await Assert.That(exception.FieldName).IsEqualTo(fieldName);
            _ = await Assert.That(exception.ExpectedType).IsEqualTo(expectedType);
            _ = await Assert.That(exception.ActualValue).IsEqualTo(actualValue);
            _ = await Assert.That(exception.Message).Contains($"Content validation failed for field '{fieldName}'");
            _ = await Assert.That(exception.Message).Contains($"Expected type '{expectedType.Name}'");
            _ = await Assert.That(exception.Message).Contains($"received value: {actualValue}");
        }
    }

    [Test]
    public async Task Constructor_WithNullActualValue_SetsNullInMessage()
    {
        const string fieldName = "TestField";
        var expectedType = typeof(string);

        var exception = new ContentValidationException(fieldName, expectedType, null);

        using (Assert.Multiple())
        {
            _ = await Assert.That(exception.FieldName).IsEqualTo(fieldName);
            _ = await Assert.That(exception.ExpectedType).IsEqualTo(expectedType);
            _ = await Assert.That(exception.ActualValue).IsNull();
            _ = await Assert.That(exception.Message).Contains("received value: null");
        }
    }

    [Test]
    public void Constructor_WhenFieldNameNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "fieldName",
            () => _ = new ContentValidationException(null!, typeof(string), "value")
        );

    [Test]
    public void Constructor_WhenExpectedTypeNull_ThrowsArgumentNullException() =>
        _ = Assert.Throws<ArgumentNullException>(
            "expectedType",
            () => _ = new ContentValidationException("field", null!, "value")
        );

    [Test]
    public async Task FieldName_WhenSetViaConstructor_ReturnsValue()
    {
        const string fieldName = "MyField";

        var exception = new ContentValidationException(fieldName, typeof(int), "invalid");

        _ = await Assert.That(exception.FieldName).IsEqualTo(fieldName);
    }

    [Test]
    public async Task ExpectedType_WhenSetViaConstructor_ReturnsValue()
    {
        var expectedType = typeof(DateTimeOffset);

        var exception = new ContentValidationException("field", expectedType, "invalid");

        _ = await Assert.That(exception.ExpectedType).IsEqualTo(expectedType);
    }

    [Test]
    public async Task ActualValue_WhenSetViaConstructor_ReturnsValue()
    {
        const string actualValue = "test-value";

        var exception = new ContentValidationException("field", typeof(int), actualValue);

        _ = await Assert.That(exception.ActualValue).IsEqualTo(actualValue);
    }
}
