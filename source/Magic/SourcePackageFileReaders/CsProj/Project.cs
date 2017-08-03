using System.Collections.Generic;
using System.Xml.Serialization;

namespace ICanHasDotnetCore.SourcePackageFileReaders.CsProj
{
    [XmlRoot(ElementName = "Project")]
    public class Project
    {
        [XmlElementAttribute("ItemGroup")]
        public List<ItemGroup> ItemGroups { get; set; }
    }

    public class ItemGroup
    {
        [XmlElementAttribute("PackageReference")]
        public List<PackageReference> Packages { get; set;}
    }

    public class PackageReference
    {
        [XmlAttribute(AttributeName = "Include")]
        public string Id { get; set; }
    }
}