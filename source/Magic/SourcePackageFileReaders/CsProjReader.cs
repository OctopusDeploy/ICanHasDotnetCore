using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ICanHasDotnetCore.SourcePackageFileReaders.CsProj;
using Newtonsoft.Json;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class CsProjReader : ISourcePackagesFileReader
    {
        public IReadOnlyList<string> ReadDependencies(byte[] contents)
        {
            using (var ms = new MemoryStream(contents))
            using (var sr = new StreamReader(ms))
            {
                var project = (Project) new XmlSerializer(typeof(Project)).Deserialize(sr);
                return project.ItemGroups.Where(g => g.Packages.Any()).SelectMany(g => g.Packages).Select(p => p.Id).ToArray();
            }
        }
    }
}