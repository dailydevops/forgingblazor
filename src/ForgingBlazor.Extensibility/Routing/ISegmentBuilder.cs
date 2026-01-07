namespace NetEvolve.ForgingBlazor.Routing;

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;

public interface ISegmentBuilder
{
    ISegmentBuilder WithAlias([NotNull] StringValues aliases, bool cascadeValue = false);

    ISegmentBuilder WithAlias(bool cascadeValue = false, [NotNull] params string[] aliases) =>
        WithAlias(new StringValues(aliases), cascadeValue);

    ISegmentBuilder WithLanguage([NotNull] string language);

    ISegmentBuilder WithPage<TPage>([NotNull] string segment, Action<IPageBuilder>? pageBuilder = null)
        where TPage : class, IComponent;

    ISegmentBuilder WithPagination<TPage>(
        Action<IPaginationBuilder>? paginationBuilder = null,
        int? pageSize = null,
        PaginationMode? paginationMode = null
    )
        where TPage : class, IComponent;

    ISegmentBuilder WithSegment([NotNull] string segment, Action<ISegmentBuilder> segmentBuilder);
}
