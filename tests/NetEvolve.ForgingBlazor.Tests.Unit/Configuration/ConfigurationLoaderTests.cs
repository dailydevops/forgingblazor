namespace NetEvolve.ForgingBlazor.Tests.Unit.Configuration;

using NetEvolve.ForgingBlazor.Configurations;

public sealed class ConfigurationLoaderTests
{
    [Test]
    [Arguments(null!)]
    [Arguments("  ")]
    [Arguments("")]
    public void Load_ConfigurationFile_ThrowsArgumentException_WhenProjectPathIsNullOrWhiteSpace(string? projectPath) =>
        _ = Assert.Throws<ArgumentException>(() => ConfigurationLoader.Load(null!, projectPath));
}
