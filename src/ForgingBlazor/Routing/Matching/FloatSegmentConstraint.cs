namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

internal sealed class FloatSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(float);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (
            float.TryParse(
                segment,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var floatValue
            )
        )
        {
            value = floatValue;
            return true;
        }

        value = default(float);
        return false;
    }
}
