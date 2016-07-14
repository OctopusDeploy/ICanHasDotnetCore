using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class ProjectJsonFileReader : ISourcePackagesFileReader
    {
        public IReadOnlyList<string> ReadDependencies(byte[] contents)
        {
            using (var ms = new MemoryStream(contents))
            using (var sr = new StreamReader(ms))
            {
                var model = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                var value = model["dependencies"].Value<JObject>();
                var dependencies = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(value.ToString());
                return dependencies?.Keys.ToArray() ?? Array.Empty<string>();
            }
        }
    }
}