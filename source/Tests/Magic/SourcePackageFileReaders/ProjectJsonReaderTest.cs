using System.Linq;
using System.Text;
using FluentAssertions;
using ICanHasDotnetCore.SourcePackageFileReaders;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic.SourcePackageFileReaders
{
    public class ProjectJsonReaderTest : ReaderTestsBase
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
            var result = new ProjectJsonReader().ReadDependencies(encodedFile);
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo("Antlr", "bootstrap");
        }

    }
}