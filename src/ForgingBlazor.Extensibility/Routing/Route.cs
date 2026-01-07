namespace NetEvolve.ForgingBlazor.Routing;

using System.Diagnostics.CodeAnalysis;

public sealed record Route
{
    private readonly IReadOnlyDictionary<Type, object> _metadata;

    public Type PageType { get; init; }

    public Route? Parent { get; init; }

    public string Template { get; init; }

    public bool TryGetMetadata<T>([NotNullWhen(true)] out T? metadata)
    {
        if (TryGetMetadata(typeof(T), out var obj) && obj is T t)
        {
            metadata = t;
            return true;
        }

        metadata = default;
        return false;
    }

    public bool TryGetMetadata(Type pageType, [NotNullWhen(true)] out object? metadata) =>
        _metadata.TryGetValue(pageType, out metadata);
}
