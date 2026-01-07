namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;

internal sealed class GuidSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(Guid);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (Guid.TryParse(segment, out var guidValue))
        {
            value = guidValue;
            return true;
        }

        value = null;
        return false;
    }
}
