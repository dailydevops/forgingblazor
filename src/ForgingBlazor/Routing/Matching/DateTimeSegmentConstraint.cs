namespace NetEvolve.ForgingBlazor.Routing.Matching;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

internal sealed class DateTimeSegmentConstraint : SegmentConstraintBase
{
    public override Type MatchType => typeof(DateTime);

    public override bool IsMatch(string segment, [NotNullWhen(true)] out object? value)
    {
        if (DateTime.TryParse(segment, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTimeValue))
        {
            value = dateTimeValue;
            return true;
        }

        value = null;
        return false;
    }
}
