using System.Linq;
using System.Text;
using FluentAssertions;
using ICanHasDotnetCore.SourcePackageFileReaders;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests.Magic.SourcePackageFileReaders
{
    [TestFixture]
    public class ProjectJsonFileReaderTest : ReaderTestsBase
    {
        protected override string Contents => @"{
  ""version"": ""1.0.0-*"",

  ""dependencies"": {
    ""Antlr"": ""3.0.11"",
    ""bootstrap"": ""3.0.11""
  },

  ""frameworks"": {
    ""net461"": {
    }
  }
}";


        protected override void Execute(byte[] encodedFile)
        {
            var result = new ProjectJsonFileReader().ReadDependencies(encodedFile);
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo("Antlr", "bootstrap");
        }

    }
}