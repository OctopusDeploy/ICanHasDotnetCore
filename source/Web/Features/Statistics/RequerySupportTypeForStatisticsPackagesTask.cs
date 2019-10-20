using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using Serilog;
using System.Linq;
using ICanHasDotnetCore.Plumbing.Extensions;
using Microsoft.Extensions.Hosting;

namespace ICanHasDotnetCore.Web.Features.Statistics
{
    public class RequerySupportTypeForStatisticsPackagesTask : BackgroundService
    {
        private readonly IStatisticsRepository _statisticsRepository;

        public RequerySupportTypeForStatisticsPackagesTask(IStatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var dueTime = TimeSpan.FromMinutes(10);
            var period = TimeSpan.FromDays(1);
            Log.Information($"Starting Requery Statistics Package Support Task in {dueTime}, then every {period}");
            await Task.Delay(dueTime, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Run();
                await Task.Delay(period, stoppingToken);
            }
        }

        public async Task Run()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Log.Information("Requery Statistics Package Support Task Started");
                var stats = _statisticsRepository.GetAllPackageStatistics();
                var packageNames = stats.Select(s => s.Name).ToArray();

                var result = await PackageCompatabilityInvestigator.Create(new NoNugetResultCache())
                    .Process("Requery", packageNames);

                foreach (var stat in stats)
                {
                    var packageResult = result.Dependencies.FirstOrDefault(f => f.PackageName.EqualsOrdinalIgnoreCase(stat.Name));
                    UpdatePackage(packageResult, stat);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Requery Statistics Package Support Task failed");
            }
            Log.Information("Requery Statistics Package Support Task Finished in {time}", sw.Elapsed);
        }

        private void UpdatePackage(PackageResult packageResult, PackageStatistic stat)
        {
            if (packageResult == null)
            {
                Log.Information("No result returned for {package}", stat.Name);
            }
            else if (!packageResult.WasSuccessful)
            {
                Log.Information("Error occured for retrieving {package}: {error}", stat.Name, packageResult.Error);
            }
            else if (stat.LatestSupportType != packageResult.SupportType && packageResult.SupportType != SupportType.Error)
            {
                Log.Information("Updating support type for {package} from {from} to {to}", stat.Name, stat.LatestSupportType,
                    packageResult.SupportType);
                _statisticsRepository.UpdateSupportTypeFor(stat, packageResult.SupportType);
            }
        }

    }
}