namespace NetEvolve.ForgingBlazor.Tests.Integration;

using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;

internal static class Predefined
{
    [ModuleInitializer]
    public static void Init()
    {
        DerivePathInfo(
            (sourceFile, projectDirectory, method, type) =>
            {
                var snapshots = Path.Combine(projectDirectory, "_snapshots", Namer.TargetFrameworkNameAndVersion);
                _ = Directory.CreateDirectory(snapshots);
                return new(snapshots, type.Name, method.Name);
            }
        );

        VerifierSettings.SortJsonObjects();
        VerifierSettings.SortPropertiesAlphabetically();

        VerifierSettings.AutoVerify(includeBuildServer: false, throwException: true);
    }
}
