using System.Collections.Generic;

namespace ICanHasDotnetCore
{
    public interface IPackagesFileReader
    {
        IReadOnlyList<string> ReadDependencies(byte[] contents);
    }
}