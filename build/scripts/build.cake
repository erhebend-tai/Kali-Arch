var target  = Argument("target",        "Default");
var config  = Argument("configuration", "Release");
var version = Argument("version",       "1.0.0");

Task("Clean").Does(() =>
{
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
    CleanDirectory("artifacts");
});

Task("Restore").IsDependentOn("Clean")
    .Does(() => DotNetRestore("CleanArch.slnx"));

// Analyzers + source generators run inside the build —
// TreatWarningsAsErrors turns every YP#### violation into a CI failure.
Task("Analyze").IsDependentOn("Restore").Does(() =>
{
    DotNetBuild("CleanArch.slnx", new DotNetBuildSettings
    {
        Configuration   = config,
        NoRestore       = true,
        MSBuildSettings = new DotNetMSBuildSettings()
            .WithProperty("TreatWarningsAsErrors", "true")
            .WithProperty("EnforceCodeStyleInBuild", "true")
    });
});

Task("Test").IsDependentOn("Analyze").Does(() =>
{
    DotNetTest("CleanArch.slnx", new DotNetTestSettings
    {
        Configuration    = config,
        NoRestore        = true,
        NoBuild          = true,
        Collectors       = new[] { "XPlat Code Coverage" },
        ResultsDirectory = "artifacts/test-results"
    });
});

Task("Pack-NuGet").IsDependentOn("Test").Does(() =>
{
    var packProjects = new[]
    {
        "tools/analyzers/CleanArch.Analyzers/CleanArch.Analyzers.csproj",
        "tools/codefixes/CleanArch.CodeFixes/CleanArch.CodeFixes.csproj",
        "shared/CleanArch.Shared.Contracts/CleanArch.Shared.Contracts.csproj",
        "shared/CleanArch.Shared.Domain/CleanArch.Shared.Domain.csproj"
    };
    foreach (var csproj in packProjects)
        DotNetPack(csproj, new DotNetPackSettings
        {
            Configuration   = config,
            OutputDirectory = "artifacts/nuget",
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Version", version)
        });
});

Task("Build-Docs").Does(() =>
{
    // dotnet tool install -g docfx; docfx docs/docfx.json
    StartProcess("docfx", "docs/docfx.json");
});

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Pack-NuGet")
    .IsDependentOn("Build-Docs");

RunTarget(target);
