namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;

internal sealed class IntSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(int);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (int.TryParse(segment, out var intValue))
        {
            value = intValue;
            return true;
        }

        value = default(int);
        return false;
    }
}
