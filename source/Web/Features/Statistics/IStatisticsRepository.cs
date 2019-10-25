using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public interface IStatisticsRepository
    {
        Task AddStatisticsForResultAsync(InvestigationResult result, CancellationToken cancellationToken);
        Task<IReadOnlyList<PackageStatistic>> GetAllPackageStatisticsAsync(CancellationToken cancellationToken);
        Task UpdateSupportTypeAsync(PackageStatistic stat, SupportType supportType, CancellationToken cancellationToken);
    }
}