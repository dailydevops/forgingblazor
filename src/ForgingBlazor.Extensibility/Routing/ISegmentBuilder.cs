namespace NetEvolve.ForgingBlazor.Routing;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;

public interface ISegmentBuilder
{
    ISegmentBuilder WithAlias(StringValues aliases, bool cascadeValue = false);

    ISegmentBuilder WithAlias(bool cascadeValue = false, params string[] aliases) =>
        WithAlias(new StringValues(aliases), cascadeValue);

    ISegmentBuilder WithLanguage(string language);

    ISegmentBuilder WithPage<TPage>(string segment, Action<IPageBuilder>? pageBuilder = null)
        where TPage : class, IComponent;

    ISegmentBuilder WithPagination(int? pageSize = Defaults.PageSizeDefault, PaginationMode paginationMode = default);

    ISegmentBuilder WithSegment(string segment, Action<ISegmentBuilder> segmentBuilder);
}
