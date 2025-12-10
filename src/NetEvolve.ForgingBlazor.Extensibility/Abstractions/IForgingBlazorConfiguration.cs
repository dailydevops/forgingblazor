namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

using NetEvolve.ForgingBlazor.Extensibility.Models;

public interface IForgingBlazorValidation<in TPageType>
    where TPageType : PageBase
{
    ValidationResult Validate(TPageType page);
}

public readonly record struct ValidationResult
{
    public static ValidationResult Success(string? message = null) => new(false, message);

    public static ValidationResult Warning(string? message) => new(false, message);

    public static ValidationResult Error(string? message) => new(true, message);

    private ValidationResult(bool isError, string message) { }
}

public record Lala : BlogPostBase;

public class LalaValidator : IForgingBlazorValidation<Lala>
{
    public ValidationResult Validate(Lala page)
    {
        throw new NotImplementedException();
    }
}
