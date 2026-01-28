namespace NetEvolve.ForgingBlazor.Routing.Configurations;

using System;
using System.Globalization;
using NetEvolve.ForgingBlazor.Routing;

/// <summary>
/// Implements the culture configuration fluent API.
/// </summary>
internal sealed class CultureConfiguration : ICultureConfiguration
{
    private readonly CultureConfigurationBuilderState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureConfiguration"/> class.
    /// </summary>
    /// <param name="state">The culture configuration state.</param>
    internal CultureConfiguration(CultureConfigurationBuilderState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    /// <inheritdoc />
    public ICultureConfiguration Default(CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        _state.SetDefaultCulture(CultureInfo.ReadOnly(culture));
        return this;
    }

    /// <inheritdoc />
    public ICultureConfiguration Default(string culture)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(culture);
        return Default(CultureInfo.GetCultureInfo(culture));
    }

    /// <inheritdoc />
    public ICultureConfiguration Default(int lcid) => Default(CultureInfo.GetCultureInfo(lcid));

    /// <inheritdoc />
    public ICultureConfiguration Supported(params CultureInfo[] cultures)
    {
        ArgumentNullException.ThrowIfNull(cultures);

        foreach (var culture in cultures)
        {
            ArgumentNullException.ThrowIfNull(culture);
            _state.AddSupportedCulture(CultureInfo.ReadOnly(culture));
        }

        return this;
    }

    /// <inheritdoc />
    public ICultureConfiguration Supported(params string[] cultures)
    {
        ArgumentNullException.ThrowIfNull(cultures);

        foreach (var culture in cultures)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(culture);
            _state.AddSupportedCulture(CultureInfo.GetCultureInfo(culture));
        }

        return this;
    }

    /// <inheritdoc />
    public ICultureConfiguration Supported(params int[] lcids)
    {
        ArgumentNullException.ThrowIfNull(lcids);

        foreach (var lcid in lcids)
        {
            _state.AddSupportedCulture(CultureInfo.GetCultureInfo(lcid));
        }

        return this;
    }
}
