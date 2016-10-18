using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public class PackageStatisticResponse
    {
        public PackageStatistic Statistic { get; set; }
        public MoreInformation MoreInformation { get; set; }
    }
}