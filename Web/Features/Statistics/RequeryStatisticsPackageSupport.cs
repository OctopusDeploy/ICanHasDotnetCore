using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public class RequeryStatisticsPackageSupport : IStartable
    {
        private readonly StatisticsRepository _statisticsRepository;

        public RequeryStatisticsPackageSupport(StatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                Log.Information("Requery Statistics Package Support Task Init");
                await Task.Delay(TimeSpan.FromSeconds(10));
                while (true)
                {
                    try
                    {
                        await Run();
                        await Task.Delay(TimeSpan.FromDays(1));
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Requery Statistics Package Support Task failed");
                    }
                }
            });
        }

        private async Task Run()
        {
            Log.Information("Requery Statistics Package Support Task Started");
            var stats = _statisticsRepository.GetAllPackageStatistics();
            foreach (var stat in stats)
            {
                var supportType = await GetSupportTypeFor(stat);
                if (stat.LatestSupportType != supportType)
                {
                    Log.Information("Updating support type for {package} from {from} to {to}", stat.Name, stat.LatestSupportType, supportType);
                    _statisticsRepository.UpdateSupportTypeFor(stat, supportType);
                }
            }
            Log.Information("Requery Statistics Package Support Task Finished");
        }

        private async Task<SupportType> GetSupportTypeFor(PackageStatistic stat)
        {
            var package = await PackageCompatabilityInvestigator.Create().GetPackage(stat.Name, false);
            return package.SupportType;
        }
    }
}