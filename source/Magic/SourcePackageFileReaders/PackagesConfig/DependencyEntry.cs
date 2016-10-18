using System.Collections.Generic;
using System.Xml.Serialization;

namespace ICanHasDotnetCore.SourcePackageFileReaders.PackagesConfig
{
    [XmlRoot(ElementName = "packages")]
    public class Packages : List<DependencyEntry>
    {
    }

    [XmlType("package")]
    public class DependencyEntry
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
    }
}