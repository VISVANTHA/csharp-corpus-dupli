// Cake orchestrates the build; it does not perform it.
// The compilation below is delegated to MSBuild, so it is byte-for-byte the same
// work the sdk-msbuild cell does. Recorded in dataset.json as a degenerate axis.
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean").Does(() => {
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
});

Task("Restore").IsDependentOn("Clean").Does(() => {
    NuGetRestore("./OrderKit.sln");
});

Task("Build").IsDependentOn("Restore").Does(() => {
    MSBuild("./OrderKit.sln", settings => settings
        .SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal));
});

Task("Test").IsDependentOn("Build").Does(() => {
    NUnit("./tests/**/bin/" + configuration + "/*.Tests.dll");
});

Task("Default").IsDependentOn("Test");

RunTarget(target);
