using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public class PackageStatistic
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public SupportType LatestSupportType { get; set; }
    }
}