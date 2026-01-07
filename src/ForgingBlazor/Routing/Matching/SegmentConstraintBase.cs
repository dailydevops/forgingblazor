namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;

internal abstract class SegmentConstraintBase
{
    public abstract Type MatchType { get; }

    public abstract bool IsMatch(string segment, [NotNullWhen(true)] out object? value);
}
