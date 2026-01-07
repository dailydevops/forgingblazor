namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

internal sealed class DecimalSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(decimal);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (decimal.TryParse(segment, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue))
        {
            value = decimalValue;
            return true;
        }

        value = default(decimal);
        return false;
    }
}
