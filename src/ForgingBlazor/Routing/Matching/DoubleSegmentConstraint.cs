namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

internal sealed class DoubleSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(double);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (
            double.TryParse(
                segment,
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var doubleValue
            )
        )
        {
            value = doubleValue;
            return true;
        }

        value = default(double);
        return false;
    }
}
