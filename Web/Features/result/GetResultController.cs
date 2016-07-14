using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Output;
using ICanHasDotnetCore.SourcePackageFileReaders;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using ICanHasDotnetCore.Web.Features.Statistics;
using ICanHasDotnetCore.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.result
{
    public class GetResultController : Controller
    {
        private readonly StatisticsRepository _statisticsRepository;
        private readonly GitHubScanner _gitHubScanner;

        public GetResultController(StatisticsRepository statisticsRepository, GitHubScanner gitHubScanner)
        {
            _statisticsRepository = statisticsRepository;
            _gitHubScanner = gitHubScanner;
        }

        [HttpPost("/api/GetResult")]
        public async Task<GetResultResponse> Get([FromBody]GetResultRequest request)
        {
            var sw = Stopwatch.StartNew();
            var packagesFileDatas = request.PackageFiles.Select(p => new SourcePackageFile(p.Name, SourcePackageFileReader.PackagesConfig, DataUriConverter.ConvertFrom(p.Contents))).ToArray();
            var result = await PackageCompatabilityInvestigator.Create()
                .Go(packagesFileDatas);
            sw.Stop();
            await _statisticsRepository.AddStatisticsForResult(result);
            LogSummaryMessage(result, sw);
            return BuildResponse(result);
        }

        [HttpPost("/api/GetResult/GitHub")]
        public async Task<ActionResult> GitHub([FromBody]GetGitHubRequest request)
        {
            var sw = Stopwatch.StartNew();
            var packagesFileDatas = await _gitHubScanner.Scan(request.Id);

            if (packagesFileDatas.WasFailure)
            {
                return BadRequest(packagesFileDatas.ErrorString);
            }

            var result = await PackageCompatabilityInvestigator.Create()
                .Go(packagesFileDatas.Value);

            sw.Stop();
            Log.Information("Generated results for GitHub repo {Repo}", request.Id);
            LogSummaryMessage(result, sw);
            LogErroredAndNotFoundPackages(request.Id, result);
            return Json(BuildResponse(result));
        }



        [HttpPost("/api/GetResult/Demo")]
        public async Task<GetResultResponse> Demo()
        {
            var packagesFileDatas = new[] { new SourcePackageFile("Our Project", SourcePackageFileReader.PackagesConfig, Encoding.UTF8.GetBytes(DemoPackagesConfig)) };
            var result = await PackageCompatabilityInvestigator.Create()
                .Go(packagesFileDatas);

            return BuildResponse(result);
        }



        private void LogSummaryMessage(InvestigationResult result, Stopwatch sw)
        {
            var grouped = result.GetAllDistinctRecursive()
                .GroupBy(r => r.SupportType)
                .ToDictionary(g => g.Key, g => g.Count());

            int itCount;
            grouped.TryGetValue(SupportType.InvestigationTarget, out itCount);
            Log.Information("Processed {Count} packages files in {Time}ms resulting in {Total} dependencies. Breakdown: {Breakdown}.", itCount, sw.ElapsedMilliseconds, grouped.Sum(g => g.Value), grouped);
        }

        private void LogErroredAndNotFoundPackages(string repoId, InvestigationResult result)
        {
            foreach(var package in result.GetAllDistinctRecursive().Where(p => p.SupportType == SupportType.Error))
                Log.Error("Error occured with package {package} in GitHub repo {repoId}: {error}", package.PackageName, repoId, package.Message);

            foreach (var package in result.GetAllDistinctRecursive().Where(p => p.SupportType == SupportType.NotFound))
                Log.Warning("Package {package} in GitHub repo {repoId} was not found", package.PackageName, repoId);
        }

        private static GetResultResponse BuildResponse(InvestigationResult result)
        {
            return new GetResultResponse()
            {
                Result = result.GetAllDistinctRecursive().Select(r => new PackageResult
                {
                    PackageName = r.PackageName,
                    SupportType = r.SupportType,
                    Error = r.Message,
                    Dependencies = r.Dependencies?.Select(d => d.PackageName).ToArray(),
                    ProjectUrl = r.ProjectUrl
                }).ToArray(),
                GraphViz = GraphVizOutputFormatter.Format(result),
            };
        }



        private const string DemoPackagesConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Serilog"" version=""3.5.2"" targetFramework=""net45"" />
  <package id=""Serilog.Sinks.Seq"" version=""3.5.2"" targetFramework=""net45"" />
  <package id=""Microsoft.Web.Xdt"" version=""2.1.1"" targetFramework=""net45"" />
  <package id=""FluentValidation"" version=""0.86.0"" targetFramework=""net45"" />
  <package id=""Microsoft.Net.Http"" version=""4.3.2"" targetFramework=""net45"" />
  <package id=""OurInteralSuperToolsLib"" version=""45.3.2"" targetFramework=""net45"" />
</packages>
";

    }
}