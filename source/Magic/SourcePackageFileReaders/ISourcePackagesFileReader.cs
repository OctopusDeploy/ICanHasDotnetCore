using System.Collections.Generic;

namespace ICanHasDotnetCore.SourcePackageFileReaders
{
    public interface ISourcePackagesFileReader
    {
        IReadOnlyList<string> ReadDependencies(byte[] contents);
    }
}