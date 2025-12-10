namespace NetEvolve.ForgingBlazor.Extensibility.Abstractions;

public interface IPagePropertyCategories
{
    IReadOnlySet<string> Categories { get; set; }
}
