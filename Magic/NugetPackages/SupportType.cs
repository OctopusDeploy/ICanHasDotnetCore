namespace ICanHasDotnetCore.NugetPackages
{
    public enum SupportType
    {
        NotFound = 0,
        Supported = 1,
        PreRelease = 2,
        Unsupported = 3,
        NoDotNetLibraries = 4,
        KnownReplacementAvailable = 5,
        InvestigationTarget = 6,
        Error = 7
    }
}