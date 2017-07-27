using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Output;
using ICanHasDotnetCore.SourcePackageFileReaders;
using Serilog;

namespace ICanHasDotnetCore.Console
{
    class Program
    {
        private static readonly HashSet<string> ExcludeDirectories = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            "node_modules",
            "bower_components",
            "packages"
        };

        static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Warning()
                .CreateLogger();

            try
            {
                if (args.Length < 2)
                {
                    System.Console.Error.WriteLine("Usage: CanIHazDotnetCore.exe <output_directory> <dir_to_scan_1> [dir_to_scan_2] ... [dir_to_scan_n]");
                    return 1;
                }

                var directories = args.Skip(1).Select(Path.GetFullPath).ToArray();
                var packageFiles = FindFiles(directories).ToArray();
                var result = PackageCompatabilityInvestigator.Create(new NoNugetResultCache())
                    .Go(packageFiles)
                    .Result;


                WriteToOutputFiles(args[0], result);

                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
                return 2;
            }
        }

        private static void WriteToOutputFiles(string outputDirectory, InvestigationResult result)
        {
            Directory.CreateDirectory(outputDirectory);
            File.WriteAllLines(Path.Combine(outputDirectory, "Flat.txt"), new[] { FlatListingOutputFormatter.Format(result) });
            File.WriteAllLines(Path.Combine(outputDirectory, "Tree.txt"), new[] { TreeOutputFormatter.Format(result) });
            File.WriteAllLines(Path.Combine(outputDirectory, "1Level.gv"), new[] { GraphVizOutputFormatter.Format(result, 1) });
            File.WriteAllLines(Path.Combine(outputDirectory, "All.gv"), new[] { GraphVizOutputFormatter.Format(result) });
            File.WriteAllLines(Path.Combine(outputDirectory, "All.cql"), new[] { CypherOutputFormatter.Format(result) });

            foreach (var package in result.PackageConfigResults)
                File.WriteAllLines(Path.Combine(outputDirectory, package.PackageName + ".gv"), new[] { GraphVizOutputFormatter.Format(package) });

            System.Console.ForegroundColor = ConsoleColor.Magenta;
            System.Console.WriteLine($"Output written to {outputDirectory}");
            System.Console.ResetColor();
        }


        private static IEnumerable<SourcePackageFile> FindFiles(IEnumerable<string> directories)
        {
            foreach (var directory in directories.Where(d => !ExcludeDirectories.Contains(Path.GetFileName(d))))
            {
                foreach (var filename in SourcePackageFileReader.SupportedFiles)
                {
                    var file = new FileInfo(Path.Combine(directory, filename));
                    if (file.Exists)
                        yield return new SourcePackageFile(file.DirectoryName, filename, File.ReadAllBytes(file.FullName));
                }

                foreach (var fileExtension in SourcePackageFileReader.SupportedExtensions)
                {
                    foreach (var file in Directory.GetFiles(directory, $"*{fileExtension}"))
                    {
                        var f = new FileInfo(Path.Combine(directory, file));
                        if(f.Exists)
                            yield return new SourcePackageFile(f.DirectoryName, file, File.ReadAllBytes(f.FullName));
                    }
                }
               
                foreach (var packageFile in FindFiles(Directory.EnumerateDirectories(directory)))
                    yield return packageFile;
            }
        }
    }
}
