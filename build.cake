//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0007"
#addin "nuget:?package=SharpCompress&version=0.12.4"
#addin nuget:?package=Cake.Npm&version=0.8.0
#addin "Cake.Gulp"
#addin "Cake.FileHelpers"

using Path = System.IO.Path;
using IO = System.IO;
using SharpCompress;
using SharpCompress.Common;
using SharpCompress.Writer;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var publishDir = "./publish";
var artifactsDir = "./artifacts";
var databaseProject = "./source/Database";
var webProject = "./source/Web";
var consoleProject = "./source/Console";

GitVersion gitVersionInfo;
string nugetVersion;
///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
      gitVersionInfo = GitVersion(new GitVersionSettings {
        OutputType = GitVersionOutput.Json
    });

    if(BuildSystem.IsRunningOnTeamCity)
        BuildSystem.TeamCity.SetBuildNumber(gitVersionInfo.NuGetVersion);

    nugetVersion = gitVersionInfo.NuGetVersion;

    Information("Building ICanHasDotnetCore v{0}", nugetVersion);
    Information("Informational Version {0}", gitVersionInfo.InformationalVersion);
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    CleanDirectory(publishDir);
    CleanDirectories("./source/**/bin");
    CleanDirectories("./source/**/obj");
    CleanDirectories("./source/**/TestResults");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetCoreRestore("source");
        Npm.FromPath(webProject).Install();
        Npm.FromPath(webProject).Install(settings => settings.Package("gulp"));
    });

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{

    ReplaceRegexInFiles("./source/Web/Features/Version.ts", "version = \"[^\"]+", "version = \"" + nugetVersion);

    Gulp.Local.Execute(s => {
        s.WorkingDirectory = webProject;
    });

    DotNetCoreBuild("./source", new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append($"/p:Version={nugetVersion}")
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
     DotNetCoreTest("./source/Tests/Tests.csproj", new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
        ArgumentCustomization = args => args.Append("-l trx")
    });
});

Task("DotnetPublish")
    .IsDependentOn("Test")
    .Does(() =>
{
    DotNetCorePublish(webProject, new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory = Path.Combine(publishDir, "Web")
    });

    DotNetCorePublish(databaseProject, new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory = Path.Combine(publishDir, "Database")
    });

    DotNetCorePublish(consoleProject, new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory = Path.Combine(publishDir, "Console")
    });
});

Task("Zip")
    .IsDependentOn("DotnetPublish")
    .Does(() => {
        var downloadsDir = Path.Combine(publishDir, @"Web\wwwroot\Downloads");
        CreateDirectory(downloadsDir);
        Zip(Path.Combine(publishDir, "Console"), Path.Combine(downloadsDir, @"ICanHasDotnetCore.zip"));
        Zip(Path.Combine(publishDir, "Web"), Path.Combine(artifactsDir, $"ICanHasDotnetCore.Web.{nugetVersion}.zip"));
        Zip(Path.Combine(publishDir, "Database"), Path.Combine(artifactsDir, $"ICanHasDotnetCore.Database.{nugetVersion}.zip"));
    });


Task("Publish")
    .IsDependentOn("Zip")
    .WithCriteria(BuildSystem.IsRunningOnTeamCity)
    .Does(() =>
{
    UploadFile(
        $"{EnvironmentVariable("Octopus3ServerUrl")}/api/packages/raw?apiKey={EnvironmentVariable("Octopus3ApiKey")}",
        $"{artifactsDir}/ICanHasDotnetCore.Web." + nugetVersion + ".zip"
    );

    UploadFile(
        $"{EnvironmentVariable("Octopus3ServerUrl")}/api/packages/raw?apiKey={EnvironmentVariable("Octopus3ApiKey")}",
        $"{artifactsDir}/ICanHasDotnetCore.Database." + nugetVersion + ".zip"
    );
});


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
