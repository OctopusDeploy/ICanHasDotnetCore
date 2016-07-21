using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Plumbing.Extensions;
using NuGet;
using Serilog;

namespace ICanHasDotnetCore.NugetPackages
{

    public class NugetPackageInfoRetriever
    {
        private readonly IPackageRepositoryWrapper _repository;

        private static readonly string[] SupportedTargetFrameworksInOrderOfPriority = new[]
        {
            ".NETStandard",
            "DNXCore",
            "ASP.NetCore",
            ".NETPlatform"
        };

        public NugetPackageInfoRetriever(IPackageRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<Result<NugetPackage>> Retrieve(string id, bool includePrerelease)
        {
            var package = await _repository.GetLatestPackage(id, includePrerelease);
            return Retrieve(id, package);
        }

        public async Task<Result<NugetPackage>> Retrieve(string id, SemanticVersion version)
        {
            var package = await _repository.GetPackage(id, version);
            return Retrieve(id, package);
        }

        public Result<NugetPackage> Retrieve(string id, IPackage package)
        {
            if (package == null)
                return new NugetPackage(id, new string[0], SupportType.NotFound);
            
            var supportedFrameworks = package.GetSupportedFrameworks().ToArray();
            var hasDllReferences = package.AssemblyReferences.Any(r => r.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));
            if (!supportedFrameworks.Any() && !hasDllReferences)
                return new NugetPackage(id, new string[0], SupportType.NonDotNet);

            var supported = supportedFrameworks
                .Any(f => SupportedTargetFrameworksInOrderOfPriority.Contains(f.Identifier));
            if (supported)
            {
                var netStdDeps = GetNetStandardDependencySet(package);
                return new NugetPackage(id, netStdDeps.ValueOrNull() ?? new string[0], package.IsReleaseVersion() ? SupportType.Supported : SupportType.PreRelease)
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


        private Option<string[]> GetNetStandardDependencySet(IPackage package)
        {
            if (package.DependencySets == null)
                return Option<string[]>.ToNone;

            var orderedDeps = package.DependencySets
                .Where(s => s.TargetFramework != null)
                .OrderByDescending(s => s.TargetFramework.Version)
                .ToArray();

            foreach (var framework in SupportedTargetFrameworksInOrderOfPriority)
            {
                var dep = orderedDeps.FirstOrNone(s => framework.EqualsOrdinalIgnoreCase(s.TargetFramework?.Identifier));
                if (dep.Some)
                    return GetDependencies(dep.Value);
            }
            return Option<string[]>.ToNone;
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