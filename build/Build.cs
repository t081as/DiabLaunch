using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build")]
    readonly Configuration Configuration = Configuration.Debug;

    [Parameter("The build number provided by the continuous integration system")]
    readonly ulong Buildnumber = 0;

    [Solution]
    readonly Solution Solution;

    AbsolutePath OutputDirectory => RootDirectory / "output";

    string shortVersion = "0.0.0";
    string version = "0.0.0.0";
    string semanticVersion = "0.0.0+XXXXXXXX";

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetClean();
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Version => _ => _
        .Executes(() =>
        {
            if (Configuration == Configuration.Release)
            {
                try
                {
                    (string shortVersion, string version, string semanticVersion) = GitVersion.Get(RootDirectory, Buildnumber);

                    this.shortVersion = shortVersion;
                    this.version = version;
                    this.semanticVersion = semanticVersion;
                }
                catch
                {
                    Logger.Info("Ignoring version detection problems");
                }

                Logger.Info($"Version: {version}");
                Logger.Info($"Short Version: {shortVersion}");
                Logger.Info($"Semantic Version: {semanticVersion}");
                Logger.Info($"Buildnumber: {Buildnumber}");
            }
            else
            {
                Logger.Info("Debug build - skipping version");
            }
        });

    Target Compile => _ => _
        .DependsOn(Restore, Version)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(OutputDirectory)
                .SetVersion(semanticVersion)
                .SetAssemblyVersion(version)
                .SetFileVersion(version)
                .EnableNoRestore());

            CopyFile(RootDirectory / "AUTHORS.txt", OutputDirectory / "AUTHORS.txt");
            CopyFile(RootDirectory / "CHANGELOG.md", OutputDirectory / "CHANGELOG.txt");
            CopyFile(RootDirectory / "LICENSE.md", OutputDirectory / "LICENSE.txt");
            CopyFile(RootDirectory / "USAGE.md", OutputDirectory / "README.txt");
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            if (Configuration == Configuration.Release)
            {
                RootDirectory.GlobFiles("*.zip").ForEach(DeleteFile);
                OutputDirectory.GlobFiles("*.dev.*").ForEach(DeleteFile);
                OutputDirectory.GlobFiles("*.deps.json").ForEach(DeleteFile); // If there are any dependencies they will be shipped
                DeleteFile(OutputDirectory / "DiabLaunch.xml"); // Remove source code documentation xml

                string archiveFileName;

                if (semanticVersion.Contains(GitVersion.DevMarker, StringComparison.InvariantCultureIgnoreCase))
                {
                    archiveFileName = $"DiabLaunch-{semanticVersion}-win32-x64.zip";
                }
                else
                {
                    archiveFileName = $"DiabLaunch-{shortVersion}-win32-x64.zip";
                }

                CompressionTasks.CompressZip(OutputDirectory, RootDirectory / archiveFileName, null, System.IO.Compression.CompressionLevel.Optimal, System.IO.FileMode.CreateNew);
            }
            else
            {
                Logger.Info("Debug build - skipping pack");
            }
        });
}