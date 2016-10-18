using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ICanHasDotnetCore.SourcePackageFileReaders.PackagesConfig;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class PackagesConfigReader : ISourcePackagesFileReader
    {
        public IReadOnlyList<string> ReadDependencies(byte[] contents)
        {
            using (var ms = new MemoryStream(contents))
            using (var sr = new StreamReader(ms))
            {
                var packages = (Packages) new XmlSerializer(typeof(Packages)).Deserialize(sr);
                return packages.Select(p => p.Id).ToArray();
            }
        }
    }
}