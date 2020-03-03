using System.Collections.Generic;
using System.Linq;

namespace ICanHasDotnetCore.Investigator
{
    public class InvestigationResult
    {
        public InvestigationResult(IReadOnlyList<PackageResult> packageConfigResults)
        {
            PackageConfigResults = packageConfigResults;
        }

        public IReadOnlyList<PackageResult> PackageConfigResults { get; }

        public IReadOnlyList<PackageResult> GetAllDistinctRecursive(int maxLevels = int.MaxValue)
        {
            return PackageConfigResults
                .Concat(PackageConfigResults.SelectMany(r => r.GetDependenciesResursive(maxLevels - 1)))
                .GroupBy(p => p.PackageName)
                .Select(g => g.First())
                .ToArray();
        }
    }
}