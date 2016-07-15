using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public class StatisticsController : Controller
    {
        private readonly StatisticsRepository _statisticsRepository;

        public StatisticsController(StatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        [HttpGet("api/Statistics")]
        public IReadOnlyList<PackageStatisticResponse> Get()
        {
            return _statisticsRepository.GetAllPackageStatistics()
                .OrderByDescending(p => p.Count)
                .ThenBy(p => p.Name)
                .Select(p => new PackageStatisticResponse()
                {
                    Statistic = p,
                    MoreInformation = MoreInformation.Get(p.Name).ValueOrNull()
                })
                .ToArray();
        }
    }
}