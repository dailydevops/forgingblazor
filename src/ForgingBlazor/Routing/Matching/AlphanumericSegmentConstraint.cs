namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

internal sealed class AlphanumericSegmentConstraint : SegmentConstraintBase
{
    private readonly SearchValues<char> _searchValues = SearchValues.Create(
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-"
    );

    public override Type MatchType => typeof(string);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (segment.AsSpan().ContainsAnyExcept(_searchValues))
        {
            value = null;
            return false;
        }

        value = segment;
        return true;
    }
}
