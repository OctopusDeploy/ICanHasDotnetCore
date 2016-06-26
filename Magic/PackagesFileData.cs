namespace ICanHasDotnetCore
{
    public class PackagesFileData
    {
        public PackagesFileData(string name, string contents)
        {
            Name = name;
            Contents = contents;
        }

        public string Name { get; set; }
        public string Contents { get; set; }
    }
}