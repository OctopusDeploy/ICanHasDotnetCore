using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class PaketDependenciesReader : ISourcePackagesFileReader
    {
        public IReadOnlyList<string> ReadDependencies(byte[] contents)
        {
            using var ms = new MemoryStream(contents);
            using var sr = new StreamReader(ms);
            var str = sr.ReadToEnd();
            var matches = Regex.Matches(str, @"nuget\s+([^\s]+)");
            return matches.Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToArray();
        }
    }
}