using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ICanHasDotnetCore.Magic.PackagesFile;

namespace ICanHasDotnetCore.Magic
{
    public class PackagesFileReader
    {
        public IReadOnlyList<string> ReadDependencies(string contents)
        {
            using (var tr = new StringReader(contents))
            {
                var packages = (Packages)new XmlSerializer(typeof(Packages)).Deserialize(tr);
                return packages.Select(p => p.Id).ToArray();
            }
        }
    }
}