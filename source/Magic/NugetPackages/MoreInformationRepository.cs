namespace ICanHasDotnetCore.NugetPackages
{
    public class MoreInformationRepository : EmbeddedResourceRepository, IMoreInformationRepository
    {
        public MoreInformationRepository() : base("ICanHasDotnetCore.NugetPackages.Data.MoreInformation.json")
        {
        }
    }
}