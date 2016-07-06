using System.Linq;
using System.Threading.Tasks;
using NuGet;
using Serilog;

namespace ICanHasDotnetCore.NugetPackages
{
    public class NugetPackageInfoRetriever
    {
        private readonly IPackageRepositoryWrapper _repository;

        private static readonly string[] KnownReplacements = {
            "Microsoft.Bcl",
            "Microsoft.Bcl.Build",
            "Microsoft.Net.Http",
            "Microsoft.Bcl.Async"
        };

        public NugetPackageInfoRetriever(IPackageRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<NugetPackage> Retrieve(string id, bool prerelease)
        {
            if (KnownReplacements.Contains(id))
                return Result.Success(new NugetPackage(id, new string[0], SupportType.KnownReplacementAvailable));

            var package = await _repository.GetLatestPackage(id, prerelease);
            if (package == null)
                return new NugetPackage(id, new string[0], SupportType.NotFound);


            var coreDeps = package.DependencySets
                .Where(s => s.TargetFramework?.Identifier == ".NETStandard")
                .OrderByDescending(s => s.TargetFramework.Version)
                .FirstOrDefault();

            if (coreDeps != null)
            {
                var dependencies = GetDependencies(coreDeps);
                return new NugetPackage(id, dependencies, package.IsReleaseVersion() ? SupportType.Supported : SupportType.PreRelease);
            }

            var osDeps = GetOldSkoolDependencies(package);

            return new NugetPackage(id, osDeps, SupportType.Unsupported);
        }


        private static string[] GetOldSkoolDependencies(IPackage package)
        {
            var oldSkoolDeps = package.DependencySets
                .Where(s => s.TargetFramework?.Identifier == ".NETFramework")
                .OrderByDescending(s => s.TargetFramework.Version)
                .FirstOrDefault()
                               ?? package.DependencySets.FirstOrDefault();

            return oldSkoolDeps == null ? new string[0] : GetDependencies(oldSkoolDeps);
        }


        private static string[] GetDependencies(PackageDependencySet coreDeps)
        {
            return coreDeps.Dependencies
                .Select(d => d.Id)
                .Where(d => !d.StartsWith("System."))
                .Where(d => !d.StartsWith("Microsoft.NETCore."))
                .Where(d => d != "NETStandard.Library")
                .ToArray();
        }
    }
}