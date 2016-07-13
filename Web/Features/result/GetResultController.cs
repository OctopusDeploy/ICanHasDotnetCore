using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Output;
using ICanHasDotnetCore.Web.Features.Statistics;
using ICanHasDotnetCore.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.result
{
    public class GetResultController : Controller
    {
        private readonly StatisticsRepository _statisticsRepository;

        public GetResultController(StatisticsRepository statisticsRepository)
        {
            _statisticsRepository = statisticsRepository;
        }

        [HttpPost("/api/GetResult")]
        public async Task<GetResultResponse> Get([FromBody]GetResultRequest request)
        {
            var sw = Stopwatch.StartNew();
            var packagesFileDatas = request.PackageFiles.Select(p => new PackagesFileData(p.Name, DataUriConverter.ConvertFrom(p.Contents))).ToArray();
            var result = await PackageCompatabilityInvestigator.Create()
                .Go(packagesFileDatas);
            sw.Stop();
            await _statisticsRepository.AddStatisticsForResult(result);
            LogMessage(result, sw);
            return BuildResponse(result);
        }

        private void LogMessage(InvestigationResult result, Stopwatch sw)
        {
            var grouped = result.GetAllDistinctRecursive()
                .GroupBy(r => r.SupportType)
                .ToDictionary(g => g.Key, g => g.Count());

            int itCount;
            grouped.TryGetValue(SupportType.InvestigationTarget, out itCount);
            Log.Information("Processed packages {Count} files in {Time}ms resulting in {Total} dependencies. Breakdown: {Breakdown}.", itCount, sw.ElapsedMilliseconds, grouped.Sum(g => g.Value), grouped);
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
                    ProjectUrl = r.ProjectUrl
                }).ToArray(),
                GraphViz = GraphVizOutputFormatter.Format(result),
            };
        }


        [HttpGet("/api/GetResult/Demo")]
        public async Task<GetResultResponse> Demo()
        {
            var packagesFileDatas = new[] { new PackagesFileData("Our Project", Encoding.UTF8.GetBytes(DemoPackagesConfig)) };
            var result = await PackageCompatabilityInvestigator.Create()
                .Go(packagesFileDatas);

            return BuildResponse(result);
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