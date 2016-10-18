using System;
using System.Collections.Generic;
using System.Linq;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Plumbing.Extensions;
using Serilog;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class SourcePackageFileReader
    {
        public const string PackagesConfig = "packages.config";
        public const string ProjectJson = "project.json";
        public const string Paket = "paket.dependencies";

        private static readonly Dictionary<string, ISourcePackagesFileReader> SourcePackagesFileReaders = new Dictionary<string, ISourcePackagesFileReader>(StringComparer.OrdinalIgnoreCase)
        {
            { PackagesConfig, new PackagesConfigReader() },
            { ProjectJson, new ProjectJsonReader() },
            { Paket, new PaketDependenciesReader() }
        };

        public static IReadOnlyList<string> SupportedFiles => SourcePackagesFileReaders.Keys.ToArray();

        public static Result<IReadOnlyList<string>> Read(SourcePackageFile file)
        {
            if (!SourcePackagesFileReaders.ContainsKey(file.OriginalFileName))
            {
                Log.Warning("The filename {filename} was not recognised as a supported file format", file.OriginalFileName);
                return Result.Failed<IReadOnlyList<string>>($"The filename {file.OriginalFileName} was not recognised as a supported file format. Supported types are {SupportedFiles.CommaSeperate()}.");
            }
            return SourcePackagesFileReaders[file.OriginalFileName]
                .ReadDependencies(file.Contents)
                .AsSuccess();
        }
    }
}