using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ICanHasDotnetCore.SourcePackageFileReaders.CsProj;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class CsProjReader : ISourcePackagesFileReader
    {
        public IReadOnlyList<string> ReadDependencies(byte[] contents)
        {
            using (var ms = new MemoryStream(contents))
            using (var sr = new StreamReader(ms))
            using(var xr = XmlReader.Create(sr))
            {
                var xs = new XmlSerializer(typeof(Project));
                if (!xs.CanDeserialize(xr)) return new string[0];

                var project = (Project)new XmlSerializer(typeof(Project)).Deserialize(xr);
                return project.ItemGroups.Where(g => g.Packages.Any()).SelectMany(g => g.Packages).Select(p => p.Id)
                    .ToArray();
            }
        }
    }
}