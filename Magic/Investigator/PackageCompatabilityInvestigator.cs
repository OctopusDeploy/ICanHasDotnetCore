using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Plumbing.Extensions;
using ICanHasDotnetCore.SourcePackageFileReaders;
using Serilog;

namespace ICanHasDotnetCore.Investigator
{
    public class PackageCompatabilityInvestigator
    {

        private readonly NugetPackageInfoRetriever _nugetPackageInfoRetriever;

        private readonly ConcurrentDictionary<string, Task<PackageResult>> _results = new ConcurrentDictionary<string, Task<PackageResult>>();

        public PackageCompatabilityInvestigator(NugetPackageInfoRetriever nugetPackageInfoRetriever)
        {
            _nugetPackageInfoRetriever = nugetPackageInfoRetriever;
        }

        public static PackageCompatabilityInvestigator Create()
        {
            var repository = new PackageRepositoryWrapper();
            return new PackageCompatabilityInvestigator(
                new NugetPackageInfoRetriever(
                    repository
                )
            );
        }

        public async Task<InvestigationResult> Go(IReadOnlyList<SourcePackageFile> files)
        {
            MakeNamesUnique(files);

            var results = files.Select(Process).ToArray();
            return new InvestigationResult(await Task.WhenAll(results));
        }

        private static void MakeNamesUnique(IReadOnlyList<SourcePackageFile> files)
        {
            for (int x = 0; x < files.Count; x++)
            {
                if (files[x].Name == null)
                    files[x].Name = $"File {x + 1}";
            }

            var usedNamed = new HashSet<string>();
            foreach (var file in files)
            {
                var originalName = file.Name;
                var x = 0;
                while (usedNamed.Contains(file.Name))
                {
                    x++;
                    file.Name = $"{originalName} {x}";
                }
                usedNamed.Add(file.Name);
            }

        }

        private async Task<PackageResult> Process(SourcePackageFile file)
        {
            try
            {
                var dependencies = SourcePackageFileReader.Read(file);
                if (dependencies.WasFailure)
                    return PackageResult.Failed(file.Name, dependencies.ErrorString);

                var dependencyResults = await GetDependencyResults(dependencies.Value);
                return PackageResult.InvestigationTarget(file.Name, dependencyResults);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing supplied data file");
                return PackageResult.Failed(file.Name, "An error occured - " + ex.Message);
            }
        }

        private async Task<IReadOnlyList<PackageResult>> GetDependencyResults(IReadOnlyList<string> dependencies)
        {
            var tasks = dependencies.Select(d => _results.GetOrAdd(d, GetPackageAndDependencies));
            return await Task.WhenAll(tasks);
        }


        private async Task<PackageResult> GetPackageAndDependencies(string id)
        {
            try
            {
                var moreInformation = MoreInformationRepository.Get(id);

                var knownReplacement = KnownReplacementsRepository.Get(id);
                if (knownReplacement.Some)
                    return PackageResult.KnownReplacement(id, knownReplacement.Value);

                var package = await GetReleaseOrPrereleasePackage(id);
                var dependencyResults = await GetDependencyResults(package.Dependencies);
                return PackageResult.Success(package, dependencyResults, moreInformation);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing package {package}", id);
                return PackageResult.Failed(id, "An error occured - " + ex.Message);
            }
        }

        private async Task<NugetPackage> GetReleaseOrPrereleasePackage(string id)
        {
            var package = await _nugetPackageInfoRetriever.Retrieve(id, false);
            if (package.SupportType == SupportType.Unsupported ||
                package.SupportType == SupportType.NotFound ||
                package.SupportType == SupportType.NoDotNetLibraries)
            {
                var prerelease = await _nugetPackageInfoRetriever.Retrieve(id, true);
                if (prerelease.SupportType == SupportType.PreRelease)
                    return prerelease;
            }


            return package;
        }
    }
}