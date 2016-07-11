using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
        public IReadOnlyList<PackageStatistic> Get()
        {
            return _statisticsRepository.GetAllPackageStatistics()
                .OrderByDescending(p => p.Count)
                .ThenBy(p => p.Name)
                .ToArray();
        }
    }
}