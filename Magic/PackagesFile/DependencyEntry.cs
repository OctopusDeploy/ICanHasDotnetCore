using System.Collections.Generic;
using System.Xml.Serialization;

namespace ICanHasDotnetCore.Magic.PackagesFile
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