using System.Collections.Generic;
using ICanHasDotnetCore.NugetPackages;
using Microsoft.AspNetCore.Mvc;

namespace ICanHasDotnetCore.Web.Features.Knowledge
{
    public class KnowledgeController : Controller
    {
        private readonly IKnownReplacementsRepository _knownReplacementsRepository;
        private readonly IMoreInformationRepository _moreInformationRepository;

        public KnowledgeController(IKnownReplacementsRepository knownReplacementsRepository, IMoreInformationRepository moreInformationRepository)
        {
            _knownReplacementsRepository = knownReplacementsRepository;
            _moreInformationRepository = moreInformationRepository;
        }

        [HttpGet("/api/Knowledge/KnownReplacements")]
        public IReadOnlyList<MoreInformation> GetKnownReplacements()
        {
            return _knownReplacementsRepository.All;
        }

        [HttpGet("/api/Knowledge/MoreInformation")]
        public IReadOnlyList<MoreInformation> GetMoreInformation()
        {
            return _moreInformationRepository.All;
        }
    }
}