using System;
using System.Linq;
using System.Threading.Tasks;
using ICanHasDotnetCore.Output;
using ICanHasDotnetCore.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.result
{
    public class GetResultController : Controller
    {
        [HttpPost("/api/GetResult")]
        public async Task<GetResultResponse> Get([FromBody]GetResultRequest request)
        {
            var packagesFileDatas = request.PackageFiles.Select(p => new PackagesFileData(p.Name, DataUriConverter.ConvertFrom(p.Contents))).ToArray();
            var result = await PackageCompatabilityInvestigator.Create()
                .Go(packagesFileDatas);

            return new GetResultResponse()
            {
                Result = result.GetAllDistinctRecursive().Select(r => new PackageResult
                {
                    PackageName = r.PackageName,
                    SupportType = r.SupportType,
                    Error = r.Error,
                    WasSuccessful = r.WasSuccessful,
                    Dependencies = r.Dependencies?.Select(d => d.PackageName).ToArray()
                }).ToArray(),
                GraphViz = GraphVizOutputFormatter.Format(result),
            };
        }

    }
}