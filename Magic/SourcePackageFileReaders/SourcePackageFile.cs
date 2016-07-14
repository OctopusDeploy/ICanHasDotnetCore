namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public class SourcePackageFile
    {
        public SourcePackageFile(string name, byte[] contents)
        {
            Name = name;
            Contents = contents;
        }

        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}