using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests
{
    [TestFixture]
    public class ProjectJsonFileReaderTest
    {
        private const string Contents = @"{
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


        private static void Execute(byte[] encodedFile)
        {
            var result = new ProjectJsonFileReader().ReadDependencies(encodedFile);
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo("Antlr", "bootstrap");
        }

        [Test]
        public void Utf8FileCanBeRead()
        {
            var encodedFile = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(Contents)).ToArray();
            Execute(encodedFile);
        }

        [Test]
        public void Utf16FileCanBeRead()
        {
            var encodedFile = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(Contents)).ToArray();
            Execute(encodedFile);
        }

        [Test]
        public void Utf32FileCanBeRead()
        {
            var encodedFile = Encoding.UTF32.GetPreamble().Concat(Encoding.UTF32.GetBytes(Contents)).ToArray();
            Execute(encodedFile);
        }

        [Test]
        public void FileCanBeRead()
        {
            var encodedFile = Encoding.UTF8.GetBytes(Contents);
            Execute(encodedFile);
        }
    }
}