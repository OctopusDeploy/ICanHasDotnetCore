using System.Collections.Generic;
using ICanHasDotnetCore.NugetPackages;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.KnownReplacements
{
    public class KnownReplacementsController : Controller
    {

        [HttpGet("/api/KnownReplacements")]
        public IReadOnlyList<KnownReplacement> Get()
        {
            return KnownReplacement.All;
        }
    }
}