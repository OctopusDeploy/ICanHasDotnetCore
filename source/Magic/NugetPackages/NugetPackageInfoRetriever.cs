using System;
using System.Collections.Generic;
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
        private readonly INugetResultCache _nugetResultCache;


        private static readonly string[] SupportedTargetFrameworksInOrderOfPriority = new[]
        {
            ".NETStandard",
            "DNXCore",
            "ASP.NetCore",
            ".NETPlatform"
        };

        public NugetPackageInfoRetriever(IPackageRepositoryWrapper repository, INugetResultCache nugetResultCache)
        {
            _repository = repository;
            _nugetResultCache = nugetResultCache;
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
                return new NugetPackage(id, new string[0], SupportType.NotFound, Option<SemanticVersion>.ToNone, new FrameworkName[0]);

            var cached = _nugetResultCache.Get(package.Id, package.Version);
            if (cached.Some)
                return cached.Value;

            var packageFrameworks = package.DependencySets.Select(s => s.TargetFramework)
                .Union(package.DependencySets.SelectMany(s => s.SupportedFrameworks))
                .Union(package.GetSupportedFrameworks())
                .Where(f => f != null)
                .ToArray();

            var result =  LookForSupportedFrameworks(package, packageFrameworks)
                .IfNone(() => CheckToolAndNoDotNetLibraries(package, packageFrameworks))
                .ValueOr(() => FallbackToOldSkool(package, packageFrameworks));

            _nugetResultCache.Store(result);

            return result;
        }


        private Option<NugetPackage> CheckToolAndNoDotNetLibraries(IPackage package, IReadOnlyList<FrameworkName> packageFrameworks)
        {
            var hasDllReferences = package.AssemblyReferences.Any(r => r.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));

            if (packageFrameworks.Any() || hasDllReferences)
                return Option<NugetPackage>.ToNone;

            var deps = package.DependencySets.FirstOrNone()
                .IfSome(s => GetDependencies(false, s).Some())
                .ValueOr(new string[0]);

            return new NugetPackage(package.Id, deps, SupportType.NoDotNetLibraries, package.Version, packageFrameworks);
        }

        private Option<NugetPackage> LookForSupportedFrameworks(IPackage package, IReadOnlyList<FrameworkName> packageFrameworks)
        {
            var supportedFramework = GetBestFramework(packageFrameworks);
            if (supportedFramework.None)
                return Option<NugetPackage>.ToNone;

            var isCore = SupportedTargetFrameworksInOrderOfPriority.Contains(supportedFramework.Value.Identifier, StringComparer.OrdinalIgnoreCase);

            var depSet = package.DependencySets
                .Where(s => s.TargetFramework != null)
                .Where(s => s.TargetFramework.Identifier.EqualsOrdinalIgnoreCase(supportedFramework.Value.Identifier))
                .OrderByDescending(s => s.TargetFramework.Version)
                .FirstOrNone()
                .IfNone(() => package.DependencySets.Where(s => s.TargetFramework == null).FirstOrNone())
                .IfSome(s => GetDependencies(isCore, s).Some())
                .ValueOr(() => new string[0]);

            return new NugetPackage(package.Id, depSet, package.IsReleaseVersion() ? SupportType.Supported : SupportType.PreRelease, package.Version, packageFrameworks)
            {
                ProjectUrl = package.ProjectUrl?.ToString()
            };
        }

        public Option<FrameworkName> GetBestFramework(IReadOnlyList<FrameworkName> packageFrameworks)
        {
            foreach (var name in SupportedTargetFrameworksInOrderOfPriority)
            {
                var match = packageFrameworks.FirstOrNone(f => f.Identifier.EqualsOrdinalIgnoreCase(name));
                if (match.Some)
                    return match;
            }

            return packageFrameworks.FirstOrNone(f => f.IsPortableFramework() && PclProfileCompatabilityChecker.Check(f.Profile));
        }



        private NugetPackage FallbackToOldSkool(IPackage package, IReadOnlyList<FrameworkName> supportedFrameworks)
        {
            var set = package.DependencySets
              .Where(s => s.TargetFramework?.Identifier == ".NETFramework")
              .OrderByDescending(s => s.TargetFramework.Version)
              .FirstOrDefault()
                             ?? package.DependencySets.FirstOrDefault();

            var deps = set == null ? new string[0] : GetDependencies(false, set);
            return new NugetPackage(package.Id, deps, SupportType.Unsupported, package.Version, supportedFrameworks)
            {
                ProjectUrl = package.ProjectUrl?.ToString()
            };
        }


        private static string[] GetDependencies(bool isCore, PackageDependencySet coreDeps)
        {
            return coreDeps.Dependencies
                .Select(d => d.Id)
                .Where(d => !(isCore && d.StartsWith("System.")))
                .Where(d => !d.StartsWith("Microsoft.NETCore."))
                .Where(d => d != "NETStandard.Library")
                .ToArray();
        }

    }
}