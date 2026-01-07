namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;

internal sealed class LongSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(long);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (long.TryParse(segment, out var longValue))
        {
            value = longValue;
            return true;
        }

        value = default(long);
        return false;
    }
}
