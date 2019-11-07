using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private readonly Dictionary<string, Task<PackageResult>> _results = new Dictionary<string, Task<PackageResult>>();

        private readonly SemaphoreSlim _maxParrallelism = new SemaphoreSlim(3, 3);

        public PackageCompatabilityInvestigator(NugetPackageInfoRetriever nugetPackageInfoRetriever)
        {
            _nugetPackageInfoRetriever = nugetPackageInfoRetriever;
        }

        public static PackageCompatabilityInvestigator Create(INugetResultCache nugetResultCache)
        {
            var repository = new PackageRepositoryWrapper(new NuGetSerilogLogger());
            return new PackageCompatabilityInvestigator(
                new NugetPackageInfoRetriever(
                    repository,
                    nugetResultCache
                )
            );
        }

        public async Task<InvestigationResult> GoAsync(IReadOnlyList<SourcePackageFile> files, CancellationToken cancellationToken)
        {
            files = ExcludeCsprojThatAlsoHaveAPackagesConfig(files);

            MakeNamesUnique(files);

            var results = files.Select(f => ProcessAsync(f, cancellationToken)).ToArray();
            return new InvestigationResult(await Task.WhenAll(results));
        }

        private IReadOnlyList<SourcePackageFile> ExcludeCsprojThatAlsoHaveAPackagesConfig(IReadOnlyList<SourcePackageFile> files)
        {
            var exclude = files.Where(f => f.OriginalFileExtension == ".csproj" && files.Any(o => o.Name == f.Name && o.OriginalFileName == "packages.config"));
            return files.Except(exclude).ToArray();
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

        private async Task<PackageResult> ProcessAsync(SourcePackageFile file, CancellationToken cancellationToken)
        {
            try
            {
                var dependencies = SourcePackageFileReader.Read(file);
                if (dependencies.WasFailure)
                    return PackageResult.Failed(file.Name, dependencies.ErrorString);

                return await ProcessAsync(file.Name, dependencies.Value, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing supplied data file");
                return PackageResult.Failed(file.Name, "An error occured - " + ex.Message);
            }
        }

        public async Task<PackageResult> ProcessAsync(string targetName, IReadOnlyList<string> dependencies, CancellationToken cancellationToken)
        {
            try
            {
                var dependencyResults = await GetDependencyResultsAsync(dependencies, cancellationToken);
                return PackageResult.InvestigationTarget(targetName, dependencyResults);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing supplied data file");
                return PackageResult.Failed(targetName, "An error occured - " + ex.Message);
            }
        }

        private async Task<IReadOnlyList<PackageResult>> GetDependencyResultsAsync(IReadOnlyList<string> dependencies, CancellationToken cancellationToken)
        {
            var tasks = dependencies.Select(d =>
            {
                lock (_results)
                    return _results.ContainsKey(d)
                        ? _results[d]
                        : _results[d] = GetPackageAndDependenciesAsync(d, cancellationToken);
            });
            return await Task.WhenAll(tasks);
        }


        public async Task<PackageResult> GetPackageAndDependenciesAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                var moreInformation = MoreInformationRepository.Get(id);

                var knownReplacement = KnownReplacementsRepository.Get(id);
                if (knownReplacement.Some)
                    return PackageResult.KnownReplacement(id, knownReplacement.Value);

                var package = await GetReleaseOrPrereleasePackageAsync(id, cancellationToken);
                var dependencyResults = await GetDependencyResultsAsync(package.Dependencies, cancellationToken);
                return PackageResult.Success(package, dependencyResults, moreInformation);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing package {package}", id);
                return PackageResult.Failed(id, "An error occured - " + ex.Message);
            }
        }

        private async Task<NugetPackage> GetReleaseOrPrereleasePackageAsync(string id, CancellationToken cancellationToken)
        {
            await _maxParrallelism.WaitAsync(cancellationToken);
            try
            {
                var package = await _nugetPackageInfoRetriever.RetrieveAsync(id, false, cancellationToken);
                if (package.SupportType == SupportType.Unsupported ||
                    package.SupportType == SupportType.NotFound ||
                    package.SupportType == SupportType.NoDotNetLibraries)
                {
                    return await _nugetPackageInfoRetriever.RetrieveAsync(id, true, cancellationToken);
                }
                return package;
            }
            finally
            {
                _maxParrallelism.Release();
            }
        }
    }
}