using System.Collections.Generic;
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.NugetPackages
{
    public interface IMoreInformationRepository
    {
        IReadOnlyList<MoreInformation> All { get; }
        Option<MoreInformation> Get(string id);
    }
}