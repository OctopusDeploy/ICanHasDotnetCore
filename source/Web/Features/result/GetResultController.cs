using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
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
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.Web.Features.result
{
    [SuppressMessage("ReSharper", "VSTHRD200")]
    public class GetResultController : Controller
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly GitHubScanner _gitHubScanner;
        private readonly INugetResultCache _nugetResultCache;

        public GetResultController(IStatisticsRepository statisticsRepository, GitHubScanner gitHubScanner, INugetResultCache nugetResultCache)
        {
            _statisticsRepository = statisticsRepository;
            _gitHubScanner = gitHubScanner;
            _nugetResultCache = nugetResultCache;
        }

        [HttpPost("/api/GetResult")]
        public async Task<GetResultResponse> Get([FromBody]GetResultRequest request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var packagesFileDatas = request.PackageFiles.Select(p => new SourcePackageFile(p.Name, p.OriginalFileName ?? SourcePackageFileReader.PackagesConfig, DataUriConverter.ConvertFrom(p.Contents))).ToArray();
            var result = await PackageCompatabilityInvestigator.Create(_nugetResultCache)
                .GoAsync(packagesFileDatas, cancellationToken);
            sw.Stop();
            await _statisticsRepository.AddStatisticsForResultAsync(result, cancellationToken);
            LogSummaryMessage(result, sw);
            return BuildResponse(result);
        }

        [HttpPost("/api/GetResult/GitHub")]
        public async Task<ActionResult> GitHub([FromBody]GetGitHubRequest request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var packagesFileDatas = await _gitHubScanner.ScanAsync(request.Id);

            if (packagesFileDatas.WasFailure)
            {
                return BadRequest(packagesFileDatas.ErrorString);
            }

            var result = await PackageCompatabilityInvestigator.Create(_nugetResultCache)
                .GoAsync(packagesFileDatas.Value, cancellationToken);

            sw.Stop();
            Log.Information("Generated results for GitHub repo {Repo}", request.Id);
            LogSummaryMessage(result, sw);
            LogErroredAndNotFoundPackages(request.Id, result);
            return Json(BuildResponse(result));
        }

        [HttpPost("/api/GetResult/NuGet")]
        public async Task<GetResultResponse> NuGet([FromBody]GetNuGetRequest request, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var packageResult = await PackageCompatabilityInvestigator.Create(_nugetResultCache)
                .GetPackageAndDependenciesAsync(request.PackageId, cancellationToken);
            var result = new InvestigationResult(new []{packageResult});
            sw.Stop();
            await _statisticsRepository.AddStatisticsForResultAsync(result, cancellationToken);
            LogSummaryMessage(result, sw);
            return BuildResponse(result);
        }

        [HttpPost("/api/GetResult/Demo")]
        public async Task<GetResultResponse> Demo(CancellationToken cancellationToken)
        {
            var packagesFileDatas = new[] { new SourcePackageFile("Our Project", SourcePackageFileReader.PackagesConfig, Encoding.UTF8.GetBytes(DemoPackagesConfig)) };
            var result = await PackageCompatabilityInvestigator.Create(_nugetResultCache)
                .GoAsync(packagesFileDatas, cancellationToken);

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
            foreach(var package in result.GetAllDistinctRecursive().Where(p => p.SupportType == SupportType.Error && !p.Error.Contains("A task was canceled")))
                Log.Error("Error occured with package {package} in GitHub repo {repoId}: {error}", package.PackageName, repoId, package.Error);

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
                    Error = r.Error,
                    Dependencies = r.Dependencies?.Select(d => d.PackageName).ToArray(),
                    ProjectUrl = r.ProjectUrl,
                    MoreInformation = r.MoreInformation.ValueOrNull()
                }).ToArray(),
                GraphViz = GraphVizOutputFormatter.Format(result),
                Cypher = CypherOutputFormatter.Format(result)
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