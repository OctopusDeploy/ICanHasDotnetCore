using System;
using System.Linq;
using System.Threading.Tasks;
using ICanHasDotnetCore.Output;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.result
{
    public class GetResultController : Controller
    {
        public async Task<GetResultResponse> Get(GetResultRequest request)
        {
            var result = await PackageCompatabilityInvestigator.Create()
                .Go(request.PackageFiles.Select(p => new PackagesFileData(p.Name, p.Contents)).ToArray());

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