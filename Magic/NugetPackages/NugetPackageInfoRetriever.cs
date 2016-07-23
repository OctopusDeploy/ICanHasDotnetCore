using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Plumbing.Extensions;
using NuGet;

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

        public async Task<NugetPackage> Retrieve(string id, bool includePrerelease)
        {
            var package = await _repository.GetLatestPackage(id, includePrerelease);
            return Retrieve(id, package);
        }

        public async Task<NugetPackage> Retrieve(string id, SemanticVersion version)
        {
            var package = await _repository.GetPackage(id, version);
            return Retrieve(id, package);
        }
        
        private NugetPackage Retrieve(string id, IPackage package)
        {
            if (package == null)
                return new NugetPackage(id, new string[0], SupportType.NotFound, Option<SemanticVersion>.ToNone);

            return LookForSupportedFrameworks(package)
                .IfNone(() => CheckToolAndNoDotNetLibraries(package))
                .ValueOr(() => FallbackToOldSkool(package));
        }


        private Option<NugetPackage> CheckToolAndNoDotNetLibraries(IPackage package)
        {
            var supportedFrameworks = package.GetSupportedFrameworks().ToArray();
            var hasDllReferences = package.AssemblyReferences.Any(r => r.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));

            if (supportedFrameworks.Any() || hasDllReferences)
                return Option<NugetPackage>.ToNone;

            var deps = package.DependencySets.FirstOrNone()
                .IfSome(s => GetDependencies(s).Some())
                .ValueOr(new string[0]);

            return new NugetPackage(package.Id, deps, SupportType.NoDotNetLibraries, package.Version);
        }

        private Option<NugetPackage> LookForSupportedFrameworks(IPackage package)
        {
            var supportedFramework = GetBestFramework(package);
            if (supportedFramework.None)
                return Option<NugetPackage>.ToNone;

            var depSet = package.DependencySets
                .Where(s => s.TargetFramework != null)
                .Where(s => s.TargetFramework.Identifier.EqualsOrdinalIgnoreCase(supportedFramework.Value.Identifier))
                .OrderByDescending(s => s.TargetFramework.Version)
                .FirstOrNone()
                .IfSome(s => GetDependencies(s).Some())
                .ValueOr(() => new string[0]);

            return new NugetPackage(package.Id, depSet, package.IsReleaseVersion() ? SupportType.Supported : SupportType.PreRelease, package.Version)
            {
                ProjectUrl = package.ProjectUrl?.ToString()
            };
        }

        public Option<FrameworkName> GetBestFramework(IPackage package)
        {
            var frameworks = package.DependencySets.Select(s => s.TargetFramework)
            .Union(package.DependencySets.SelectMany(s => s.SupportedFrameworks))
            .Union(package.GetSupportedFrameworks())
            .Where(f => f != null)
            .ToArray();

            foreach (var name in SupportedTargetFrameworksInOrderOfPriority)
            {
                var match = frameworks.FirstOrNone(f => f.Identifier.EqualsOrdinalIgnoreCase(name));
                if (match.Some)
                    return match;
            }

            return frameworks.FirstOrNone(f => f.IsPortableFramework() && PclProfileCompatabilityChecker.Check(f.Profile));
        }



        private NugetPackage FallbackToOldSkool(IPackage package)
        {
            var set = package.DependencySets
              .Where(s => s.TargetFramework?.Identifier == ".NETFramework")
              .OrderByDescending(s => s.TargetFramework.Version)
              .FirstOrDefault()
                             ?? package.DependencySets.FirstOrDefault();

            var deps = set == null ? new string[0] : GetDependencies(set);
            return new NugetPackage(package.Id, deps, SupportType.Unsupported, package.Version)
            {
                ProjectUrl = package.ProjectUrl?.ToString()
            };
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