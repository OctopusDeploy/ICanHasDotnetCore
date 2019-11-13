using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    [SuppressMessage("ReSharper", "VSTHRD200")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IMoreInformationRepository _moreInformationRepository;

        public StatisticsController(IStatisticsRepository statisticsRepository, IMoreInformationRepository moreInformationRepository)
        {
            _statisticsRepository = statisticsRepository;
            _moreInformationRepository = moreInformationRepository;
        }

        [HttpGet("api/Statistics")]
        public async Task<IReadOnlyList<PackageStatisticResponse>> Get(CancellationToken cancellationToken)
        {
            return (await _statisticsRepository.GetAllPackageStatisticsAsync(cancellationToken))
                .OrderByDescending(p => p.Count)
                .ThenBy(p => p.Name)
                .Select(p => new PackageStatisticResponse()
                {
                    Statistic = p,
                    MoreInformation = _moreInformationRepository.Get(p.Name).ValueOrNull()
                })
                .ToArray();
        }
    }
}