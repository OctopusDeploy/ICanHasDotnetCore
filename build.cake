//////////////////////////////////////////////////////////////////////
// TOOLS
//////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0-beta0007"
#addin "nuget:?package=Newtonsoft.Json&version=9.0.1"
#addin "nuget:?package=SharpCompress&version=0.12.4"
#addin "Cake.Npm"
#addin "Cake.Gulp"

using Path = System.IO.Path;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
var isContinuousIntegrationBuild = !BuildSystem.IsLocalBuild;

var gitVersionInfo = GitVersion(new GitVersionSettings {
    OutputType = GitVersionOutput.Json
});

var nugetVersion = gitVersionInfo.NuGetVersion;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////
Setup(context =>
{
    Information("Building ICanHasDotnetCore v{0}", nugetVersion);
});

Teardown(context =>
{
    Information("Finished running tasks.");
});

//////////////////////////////////////////////////////////////////////
//  PRIVATE TASKS
//////////////////////////////////////////////////////////////////////

Task("__Default")
    .IsDependentOn("__Clean")
    .IsDependentOn("__Restore")
    .IsDependentOn("__UpdateAssemblyVersionInformation")
    .IsDependentOn("__Build")
    .IsDependentOn("__Publish")
    .IsDependentOn("__Zip");

Task("__Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    CleanDirectory(publishDir);
    CleanDirectories("./source/*/bin");
    CleanDirectories("./source/*/obj");
});

Task("__Restore")
    .Does(() => {
        DotNetCoreRestore();
        Npm.FromPath(webProject).Install();
    });

Task("__UpdateAssemblyVersionInformation")
    .WithCriteria(isContinuousIntegrationBuild)
    .Does(() =>
{
     GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true
    });

    Information("AssemblyVersion -> {0}", gitVersionInfo.AssemblySemVer);
    Information("AssemblyFileVersion -> {0}", $"{gitVersionInfo.MajorMinorPatch}.0");
    Information("AssemblyInformationalVersion -> {0}", gitVersionInfo.InformationalVersion);
    if(BuildSystem.IsRunningOnTeamCity)
        BuildSystem.TeamCity.SetBuildNumber(gitVersionInfo.NuGetVersion);
    if(BuildSystem.IsRunningOnAppVeyor)
        AppVeyor.UpdateBuildVersion(gitVersionInfo.NuGetVersion);
});

Task("__Build")
    .IsDependentOn("__Restore")
    .IsDependentOn("__UpdateProjectJsonVersion")
    .IsDependentOn("__UpdateAssemblyVersionInformation")
    .Does(() =>
{
    Gulp.Local.Execute(s => {
        s.WorkingDirectory = webProject;
    });

    DotNetCoreBuild("**/project.json", new DotNetCoreBuildSettings
    {
        Configuration = configuration
    });
});

Task("__Test")
    .IsDependentOn("__Restore")
    .Does(() =>
{
    GetFiles("**/*Tests/project.json")
        .ToList()
        .ForEach(testProjectFile => 
        {
            DotNetCoreTest(testProjectFile.ToString(), new DotNetCoreTestSettings
            {
                Configuration = configuration,
                WorkingDirectory = Path.GetDirectoryName(testProjectFile.ToString())
            });
        });
});

Task("__UpdateProjectJsonVersion")
    //.WithCriteria(isContinuousIntegrationBuild)
    .Does(() =>
{
    Information("Updating project.json versions to {0}", nugetVersion);
    ModifyJson(Path.Combine(webProject, "project.json"), json => json["version"] = nugetVersion);
    ModifyJson(Path.Combine(databaseProject, "project.json"), json => json["version"] = nugetVersion);
    ModifyJson(Path.Combine(consoleProject, "project.json"), json => json["version"] = nugetVersion);
});

private void ModifyJson(string jsonFile, Action<JObject> modify)
{   
    var json = JsonConvert.DeserializeObject<JObject>(IO.File.ReadAllText(jsonFile));
    modify(json);
    IO.File.WriteAllText(jsonFile, JsonConvert.SerializeObject(json, Formatting.Indented));
}


Task("__DotnetPublish")
    .IsDependentOn("__Test")
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

Task("__Zip")
    .IsDependentOn("__DotnetPublish")
    .Does(() => {
        var downloadsDir = Path.Combine(publishDir, @"Web\wwwroot\Downloads");
        CreateDirectory(downloadsDir);
        Zip(Path.Combine(publishDir, "Console"), Path.Combine(downloadsDir, @"ICanHasDotnetCore.zip"));
        Zip(Path.Combine(publishDir, "Web"), Path.Combine(artifactsDir, $"ICanHasDotnetCore.Web.{nugetVersion}.zip"));
        Zip(Path.Combine(publishDir, "Database"), Path.Combine(artifactsDir, $"ICanHasDotnetCore.Database.{nugetVersion}.zip"));
    });


Task("__Publish")
    .IsDependentOn("__Zip")
    .WithCriteria(BuildSystem.IsRunningOnTeamCity)
    .Does(() =>
{
    NuGetPush($"{artifactsDir}/ICanHasDotnetCore.Web." + nugetVersion + ".zip", new NuGetPushSettings {
        Source =EnvironmentVariable("Octopus3ServerUrl"),
        ApiKey = EnvironmentVariable("Octopus3ApiKey")
    });
    NuGetPush($"{artifactsDir}/ICanHasDotnetCore.Database." + nugetVersion + ".zip", new NuGetPushSettings {
        Source =EnvironmentVariable("Octopus3ServerUrl"),
        ApiKey = EnvironmentVariable("Octopus3ApiKey")
    });
    NuGetPush($"{artifactsDir}/ICanHasDotnetCore.Database." + nugetVersion + ".zip", new NuGetPushSettings {
        Source =EnvironmentVariable("Octopus3ServerUrl"),
        ApiKey = EnvironmentVariable("Octopus3ApiKey")
    });
});


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("__Default");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);
