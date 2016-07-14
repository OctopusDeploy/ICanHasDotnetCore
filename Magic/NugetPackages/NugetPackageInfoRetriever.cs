using System.Linq;
using System.Threading.Tasks;
using NuGet;
using Serilog;

namespace ICanHasDotnetCore.NugetPackages
{
    public class NugetPackageInfoRetriever
    {
        private readonly IPackageRepositoryWrapper _repository;

        public NugetPackageInfoRetriever(IPackageRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<NugetPackage> Retrieve(string id, bool prerelease)
        {
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
                return new NugetPackage(id, dependencies, package.IsReleaseVersion() ? SupportType.Supported : SupportType.PreRelease)
                {
                    ProjectUrl = package.ProjectUrl?.ToString()
                };
            }

            var osDeps = GetOldSkoolDependencies(package);

            return new NugetPackage(id, osDeps, SupportType.Unsupported)
            {
                ProjectUrl = package.ProjectUrl?.ToString()
            };
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