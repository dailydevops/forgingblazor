namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;

internal sealed class BooleanSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(bool);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (bool.TryParse(segment, out var boolValue))
        {
            value = boolValue;
            return true;
        }

        value = default(bool);
        return false;
    }
}
