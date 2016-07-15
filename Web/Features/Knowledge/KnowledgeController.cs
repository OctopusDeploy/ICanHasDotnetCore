using System.Collections.Generic;
using ICanHasDotnetCore.NugetPackages;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.Knowledge
{
    public class KnowledgeController : Controller
    {

        [HttpGet("/api/Knowledge/KnownReplacements")]
        public IReadOnlyList<MoreInformation> GetKnownReplacements()
        {
            return KnownReplacementsRepository.All;
        }

        [HttpGet("/api/Knowledge/MoreInformation")]
        public IReadOnlyList<MoreInformation> GetMoreInformation()
        {
            return MoreInformationRepository.All;
        }
    }
}