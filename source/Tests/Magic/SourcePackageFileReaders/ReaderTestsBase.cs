using System.Linq;
using System.Text;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic.SourcePackageFileReaders
{
    public abstract class ReaderTestsBase
    {
        protected abstract string Contents { get; }

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

        protected abstract void Execute(byte[] encodedFile);

    }
}