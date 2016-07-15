using System.Collections.Generic;
using ICanHasDotnetCore.NugetPackages;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.Knowledge
{
    public class KnowledgeController : Controller
    {

        [HttpGet("/api/Knowledge/KnownReplacements")]
        public IReadOnlyList<KnownReplacement> GetKnownReplacements()
        {
            return KnownReplacement.All;
        }

        [HttpGet("/api/Knowledge/MoreInformation")]
        public IReadOnlyList<MoreInformation> GetMoreInformation()
        {
            return MoreInformation.All;
        }
    }
}