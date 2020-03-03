using System;
using System.Collections.Generic;
using System.Linq;
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
        public const string CsProj = ".csproj";

        private static readonly Dictionary<string, ISourcePackagesFileReader> SourcePackagesFileReaders = new Dictionary<string, ISourcePackagesFileReader>(StringComparer.OrdinalIgnoreCase)
        {
            { PackagesConfig, new PackagesConfigReader() },
            { ProjectJson, new ProjectJsonReader() },
            { Paket, new PaketDependenciesReader() },
            { CsProj, new CsProjReader() }
        };

        public static IReadOnlyList<string> SupportedFiles => SourcePackagesFileReaders.Keys.Except(SupportedExtensions).ToArray();
        public static IReadOnlyList<string> SupportedExtensions => new[] {CsProj};

        public static Result<IReadOnlyList<string>> Read(SourcePackageFile file)
        {
            if (!SourcePackagesFileReaders.ContainsKey(file.OriginalFileName) &&
                !SourcePackagesFileReaders.ContainsKey(file.OriginalFileExtension))
            {
                Log.Warning("The filename {filename} was not recognised as a supported file format",
                    file.OriginalFileName);
                return Result.Failed<IReadOnlyList<string>>(
                    $"The filename {file.OriginalFileName} was not recognised as a supported file format. Supported types are {SupportedFiles.CommaSeperate()}.");
            }

            var sourcePackageFileReader = SourcePackagesFileReaders.FirstOrDefault(r => r.Key.Equals(file.OriginalFileName) || r.Key.Equals(file.OriginalFileExtension)).Value;

            return sourcePackageFileReader
                .ReadDependencies(file.Contents)
                .AsSuccess();
        }
    }
}