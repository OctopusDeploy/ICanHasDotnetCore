namespace ICanHasDotnetCore.NugetPackages
{
    public class KnownReplacementsRepository : EmbeddedResourceRepository, IKnownReplacementsRepository
    {
        public KnownReplacementsRepository() : base("ICanHasDotnetCore.NugetPackages.Data.KnownReplacements.json")
        {
        }
    }
}