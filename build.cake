//////////////////////////////////////////////////////////////////////
// ADDINS
//////////////////////////////////////////////////////////////////////

#addin "Cake.FileHelpers"

//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////

#tool "GitVersion.CommandLine"
#tool "OpenCover"
#tool "ReportGenerator"
#tool "nuget:?package=vswhere"
#tool "nuget:?package=xunit.runner.console"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Version
var gitVersion = GitVersion();
var majorMinorPatch = gitVersion.MajorMinorPatch;
var informationalVersion = gitVersion.InformationalVersion;
var nugetVersion = gitVersion.NuGetVersion;
var buildVersion = gitVersion.FullBuildMetaData;

var solutionFile = "./src/BoxKite.Twitter.sln";

// Artifacts
var artifactDirectory ="./artifacts/";
var testCoverageOutputFile = artifactDirectory + "OpenCover.xml";

// Bootstrap
CreateDirectory(artifactDirectory);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Restore")
    .Does(() =>
    {
        NuGetRestore(solutionFile);

        // do it twice due to dotnet core toolchain v1.x issue.
        NuGetRestore(solutionFile);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        // multiple versions of visual studio 2017 (and upwards) can be installed 
        // side by side and in different locations. You can no longer assume that
        // msbuild is installed in C:\Program Files :)
        FilePath msBuildPath = VSWhereLatest().CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe");

        MSBuild(solutionFile, new MSBuildSettings() {
                ToolPath= msBuildPath
            }
            .WithTarget("restore;build;pack")
            .WithProperty("PackageOutputPath",  MakeAbsolute(Directory(artifactDirectory)).ToString())
            .WithProperty("TreatWarningsAsErrors", "true")
            .SetConfiguration("Release")

            // Due to https://github.com/NuGet/Home/issues/4790 and https://github.com/NuGet/Home/issues/4337 we
            // have to pass a version explicitly
            .WithProperty("Version", nugetVersion.ToString())
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));
    });

Task("Test")
    .Does(() =>
    {
        Action<ICakeContext> testAction = tool => {
            tool.XUnit2("./src/BoxKite.Twitter.Tests/bin/Release/BoxKite.Twitter.Tests.dll", new XUnit2Settings {
                OutputDirectory = artifactDirectory,
                XmlReportV1 = true,
                NoAppDomain = true
            });
        };

        OpenCover(testAction,
            testCoverageOutputFile,
            new OpenCoverSettings {
                ReturnTargetCodeOffset = 0,
                ArgumentCustomization = args => args.Append("-mergeoutput")
            }
            .WithFilter("+[*]* -[*.Tests*]*")
            .ExcludeByAttribute("*.ExcludeFromCodeCoverage*")
            .ExcludeByFile("*/*Designer.cs;*/*.g.cs;*/*.g.i.cs"));

        ReportGenerator(testCoverageOutputFile, artifactDirectory);
    });
    
Task("Package")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .Does(() =>
    {
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");
    

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
