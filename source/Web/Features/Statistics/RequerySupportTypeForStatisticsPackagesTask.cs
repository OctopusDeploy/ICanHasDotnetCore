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
                await RunAsync(stoppingToken);
                await Task.Delay(period, stoppingToken);
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Log.Information("Requery Statistics Package Support Task Started");
                var stats = await _statisticsRepository.GetAllPackageStatisticsAsync(cancellationToken);
                var packageNames = stats.Select(s => s.Name).ToArray();

                var result = await PackageCompatabilityInvestigator.Create(new NoNugetResultCache())
                    .ProcessAsync("Requery", packageNames, cancellationToken);

                foreach (var stat in stats)
                {
                    var packageResult = result.Dependencies.FirstOrDefault(f => f.PackageName.EqualsOrdinalIgnoreCase(stat.Name));
                    await UpdatePackageAsync(packageResult, stat, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Requery Statistics Package Support Task failed");
            }
            Log.Information("Requery Statistics Package Support Task Finished in {time}", sw.Elapsed);
        }

        private async Task UpdatePackageAsync(PackageResult packageResult, PackageStatistic stat, CancellationToken cancellationToken)
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
                await _statisticsRepository.UpdateSupportTypeAsync(stat, packageResult.SupportType, cancellationToken);
            }
        }

    }
}