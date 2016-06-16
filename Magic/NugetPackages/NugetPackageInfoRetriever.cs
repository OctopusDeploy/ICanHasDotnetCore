using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet;
using Serilog;

namespace ICanHasDotnetCore.Magic.NugetPackages
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

        public async Task<Result<NugetPackage>> Retrieve(string id)
        {
            if(KnownReplacements.Contains(id))
                return Result.Success(new NugetPackage(id, new string[0], SupportType.KnownReplacementAvailable));

            Log.Information("Retrieving package {id}", id);
            var package = await _repository.GetLatestPackage(id);
            if (package == null)
            {
                Log.Information("Could not find package {id}", id);
                return Result<NugetPackage>.Failed($"Could not find package {id}");
            }

            Log.Information("Found package {id}, version {version}", package.Id, package.Version);


            var coreDeps = package.DependencySets
                .Where(s => s.TargetFramework?.Identifier == ".NETStandard")
                .OrderByDescending(s => s.TargetFramework.Version)
                .FirstOrDefault();

            if (coreDeps != null)
            {
                var dependencies = GetDependencies(coreDeps);
                Log.Information("Package {id} identified as core compatible, with dependencies {dependencies}", package.Id, dependencies);
                return new NugetPackage(id, dependencies, SupportType.Supported);
            }

            var osDeps = GetOldSkoolDependencies(package);

            Log.Information("Package {id} identified as not compatible, with dependencies {dependencies}", package.Id, osDeps);
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
                .ToArray();
        }
    }
}