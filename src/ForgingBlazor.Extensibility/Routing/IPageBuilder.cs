namespace NetEvolve.ForgingBlazor.Routing;

using Microsoft.Extensions.Primitives;

public interface IPageBuilder
{
    IPageBuilder WithAliases(StringValues aliases);

    IPageBuilder WithAliases(params string[] aliases) => WithAliases(new StringValues(aliases));

    IPageBuilder WithLanguage(string language);
}
