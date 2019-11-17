using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class ProjectJsonReader : ISourcePackagesFileReader
    {
        public IReadOnlyList<string> ReadDependencies(byte[] contents)
        {
            using var ms = new MemoryStream(contents);
            using var sr = new StreamReader(ms);
            var model = JsonSerializer.Deserialize<ProjectJson>(sr.ReadToEnd(), new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            return model.Dependencies?.Keys.ToArray() ?? new string[0];
        }

        private class ProjectJson
        {
            public Dictionary<string, object> Dependencies { get; set; }
        }
    }
}