using FluentAssertions;
using NUnit;
using ICanHasDotnetCore.Magic;
using NUnit.Framework;

namespace Tests
{
    public class PackagesFileReaderTests
    {
        [Test]
        public void FileCanBeRead()
        {
            var contents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Antlr"" version=""3.4.1.9004"" targetFramework=""net461"" />
  <package id=""bootstrap"" version=""3.0.0"" targetFramework=""net461"" />
</packages>";

            var result = new PackagesFileReader().ReadDependencies(contents);
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(new[] {"Antlr", "bootstrap"});
        }
    }
}
