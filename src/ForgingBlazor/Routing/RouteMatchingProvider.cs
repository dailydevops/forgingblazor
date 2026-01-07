namespace NetEvolve.ForgingBlazor.Routing;

using NetEvolve.ForgingBlazor.Routing.Matching;

internal sealed class RouteMatchingProvider : IRouteMatchingProvider
{
    private static readonly SegmentConstraintBase _defaultConstraint = new AlphanumericSegmentConstraint();
    private static readonly Dictionary<string, SegmentConstraintBase> _routeMatching = new(StringComparer.Ordinal)
    {
        { "alpha", _defaultConstraint },
        { "bool", new BooleanSegmentConstraint() },
        { "date", new DateTimeSegmentConstraint() },
        { "datetime", new DateTimeSegmentConstraint() },
        { "decimal", new DecimalSegmentConstraint() },
        { "double", new DoubleSegmentConstraint() },
        { "float", new FloatSegmentConstraint() },
        { "guid", new GuidSegmentConstraint() },
        { "int", new IntSegmentConstraint() },
        { "long", new LongSegmentConstraint() },
    };

    public IRouteMatching Compile(string route)
    {
        var matcher = new List<(string Segment, SegmentConstraintBase? Constraint)>();

        foreach (var segment in route.Split('/'))
        {
            var indexOfOpenBrace = segment.IndexOf('{', StringComparison.Ordinal);
            var indexOfCloseBrace = segment.IndexOf('}', indexOfOpenBrace + 1);

            if (indexOfOpenBrace < 0 || indexOfCloseBrace < indexOfOpenBrace)
            {
                matcher.Add((segment, null));
                continue;
            }

            var parameter = segment[(indexOfOpenBrace + 1)..indexOfCloseBrace];
            var parameterParts = parameter.Split(':');

            if (parameterParts.Length == 2)
            {
                var name = parameterParts[0];
                var constraint = parameterParts[1];

                if (!_routeMatching.TryGetValue(constraint, out var routeMatching))
                {
                    throw new InvalidOperationException($"The route constraint '{constraint}' is not supported.");
                }

                matcher.Add((name, routeMatching));
            }
            else
            {
                matcher.Add((parameter, _defaultConstraint));
            }
        }

        return new RouteMatching(matcher);
    }
}

internal sealed class RouteMatching : IRouteMatching
{
    private readonly List<(string Segment, SegmentConstraintBase? Constraint)> _matcher;

    public RouteMatching(List<(string Segment, SegmentConstraintBase? Constraint)> matcher) => _matcher = matcher;

    public bool IsMatch(string relativeRoute, Dictionary<string, object> routeValues)
    {
        var segments = relativeRoute.Split('/');

        if (segments.Length != _matcher.Count)
        {
            return false;
        }

        var tempRouteValues = new Dictionary<string, object>();

        for (var sc = 0; sc < segments.Length; sc++)
        {
            var segment = segments[sc];
            var (segmentName, constraint) = _matcher[sc];

            if (constraint is null)
            {
                if (!segment.Equals(segmentName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            else
            {
                if (!constraint.IsMatch(segment, out var value))
                {
                    return false;
                }

                tempRouteValues[segmentName] = value;
            }
        }

        foreach (var key in tempRouteValues.Keys)
        {
            routeValues[key] = tempRouteValues[key];
        }

        return true;
    }
}
