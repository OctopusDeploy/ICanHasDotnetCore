using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace ICanHasDotnetCore.Tests
{
    public class PackagesFileReaderTests
    {
        private const string Contents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Antlr"" version=""3.4.1.9004"" targetFramework=""net461"" />
  <package id=""bootstrap"" version=""3.0.0"" targetFramework=""net461"" />
</packages>";

      

        private static void Execute(byte[] encodedFile)
        {
            var result = new PackagesFileReader().ReadDependencies(encodedFile);
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo("Antlr", "bootstrap");
        }

        [Fact]
        public void Utf8FileCanBeRead()
        {
            var encodedFile = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(Contents)).ToArray();
            Execute(encodedFile);
        }

        [Fact]
        public void Utf16FileCanBeRead()
        {
            var encodedFile = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(Contents)).ToArray();
            Execute(encodedFile);
        }

        [Fact]
        public void Utf32FileCanBeRead()
        {
            var encodedFile = Encoding.UTF32.GetPreamble().Concat(Encoding.UTF32.GetBytes(Contents)).ToArray();
            Execute(encodedFile);
        }

        [Fact]
        public void FileCanBeRead()
        {
            var encodedFile = Encoding.UTF8.GetBytes(Contents);
            Execute(encodedFile);
        }
    }
}
