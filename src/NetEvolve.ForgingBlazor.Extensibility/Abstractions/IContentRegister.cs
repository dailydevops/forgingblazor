namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

internal interface IContentRegister { }

public interface IContentRegistration
{
    Type PageType { get; }
}
