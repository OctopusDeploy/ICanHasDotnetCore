using FluentAssertions;
using ICanHasDotnetCore.SourcePackageFileReaders;

namespace ICanHasDotnetCore.Tests.Magic.SourcePackageFileReaders
{
    public class PackagesConfigReaderTests : ReaderTestsBase
    {
        protected override string Contents => @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Antlr"" version=""3.4.1.9004"" targetFramework=""net461"" />
  <package id=""bootstrap"" version=""3.0.0"" targetFramework=""net461"" />
</packages>";



        protected override void Execute(byte[] encodedFile)
        {
            var result = new PackagesConfigReader().ReadDependencies(encodedFile);
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo("Antlr", "bootstrap");
        }

    }
}
