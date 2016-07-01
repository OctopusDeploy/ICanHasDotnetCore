namespace ICanHasDotnetCore
{
    public class PackagesFileData
    {
        public PackagesFileData(string name, byte[] contents)
        {
            Name = name;
            Contents = contents;
        }

        public string Name { get; set; }
        public byte[] Contents { get; set; }
    }
}