using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICanHasDotnetCore.NugetPackages;

namespace ICanHasDotnetCore
{
    public class PackageCompatabilityInvestigator
    {
        private readonly PackagesFileReader _packagesFileReader;
        private readonly NugetPackageInfoRetriever _nugetPackageInfoRetriever;
        private readonly ConcurrentDictionary<string, Task<PackageResult>> _results = new ConcurrentDictionary<string, Task<PackageResult>>();
        public PackageCompatabilityInvestigator(PackagesFileReader packagesFileReader, NugetPackageInfoRetriever nugetPackageInfoRetriever)
        {
            _packagesFileReader = packagesFileReader;
            _nugetPackageInfoRetriever = nugetPackageInfoRetriever;
        }

        public static PackageCompatabilityInvestigator Create()
        {
           var repository = new PackageRepositoryWrapper();
            return new PackageCompatabilityInvestigator(
                new PackagesFileReader(),
                new NugetPackageInfoRetriever(
                    repository
                    )
                );
        }

        public async Task<InvestigationResult> Go(IReadOnlyList<PackagesFileData> files)
        {
            for (int x = 0; x < files.Count; x++)
                if (files[x].Name == null)
                    files[x].Name = $"File {x+1}";

            var results = files.Select(Process).ToArray();
            return new InvestigationResult(await Task.WhenAll(results));
        }

        private async Task<PackageResult> Process(PackagesFileData file)
        {
            try
            {
                var dependencies = _packagesFileReader.ReadDependencies(file.Contents);
                var dependencyResults = await GetDependencyResults(dependencies);
                return PackageResult.Success(file.Name, dependencyResults, SupportType.InvestigationTarget);
            }
            catch (Exception ex)
            {
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
                var package = await _nugetPackageInfoRetriever.Retrieve(id, false);
                if (package.WasFailure)
                    return PackageResult.Failed(id, package.ErrorString);

                if (package.Value.SupportType == SupportType.Unsupported)
                {
                    var prerelease = await _nugetPackageInfoRetriever.Retrieve(id, true);
                    if (prerelease.WasSuccessful)
                        package = prerelease;
                }

                var dependencyResults = await GetDependencyResults(package.Value.Dependencies);
                return PackageResult.Success(id, dependencyResults, package.Value.SupportType);
            }
            catch (Exception ex)
            {
                return PackageResult.Failed(id, "An error occured - " + ex.Message);
            }
        }
    }
}