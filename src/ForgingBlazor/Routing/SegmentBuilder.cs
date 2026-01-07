namespace NetEvolve.ForgingBlazor.Routing;

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

internal sealed class SegmentBuilder : ISegmentBuilder
{
    internal SegmentBuilder(Type pageType, string segment, IRouteMatchingProvider matcher) { }

    internal IReadOnlyCollection<Route> Build()
    {
        throw new NotImplementedException();
    }

    public ISegmentBuilder WithAlias([NotNull] StringValues aliases, bool cascadeValue = false) =>
        throw new NotImplementedException();

    public ISegmentBuilder WithLanguage([NotNull] string language) => throw new NotImplementedException();

    public ISegmentBuilder WithSegment([NotNull] string segment, Action<ISegmentBuilder> segmentBuilder) =>
        throw new NotImplementedException();

    ISegmentBuilder ISegmentBuilder.WithPage<TPage>(string segment, Action<IPageBuilder>? pageBuilder) =>
        throw new NotImplementedException();

    ISegmentBuilder ISegmentBuilder.WithPagination<TPage>(
        Action<IPaginationBuilder>? paginationBuilder,
        int? pageSize,
        PaginationMode? paginationMode
    ) => throw new NotImplementedException();
}
