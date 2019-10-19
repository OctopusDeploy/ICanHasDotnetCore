using System.Collections.Generic;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public interface IStatisticsRepository
    {
        Task AddStatisticsForResult(InvestigationResult result);
        IReadOnlyList<PackageStatistic> GetAllPackageStatistics();
        void UpdateSupportTypeFor(PackageStatistic stat, SupportType supportType);
    }
}