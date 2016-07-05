namespace ICanHasDotnetCore.NugetPackages
{
    public enum SupportType
    {
        Unknown = 0,
        Supported = 1,
        PreRelease = 2,
        Unsupported = 3,
        KnownReplacementAvailable = 4,
        InvestigationTarget = 5
    }
}