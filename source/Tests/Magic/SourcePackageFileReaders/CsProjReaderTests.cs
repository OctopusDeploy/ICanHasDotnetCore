using System.Text;
using FluentAssertions;
using ICanHasDotnetCore.SourcePackageFileReaders;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic.SourcePackageFileReaders
{
    public class CsProjReaderTests : ReaderTestsBase
    {
        protected override string Contents => @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net461</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include=""..\MyProject\MyProject.csproj"" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include=""Antlr"" Version=""3.0.11"" />
    <PackageReference Include=""bootstrap"" Version=""3.0.11"" />
  </ItemGroup>

  <ItemGroup Condition="" '$(TargetFramework)' == 'net461' "">
    <Reference Include=""System"" />
    <Reference Include=""Microsoft.CSharp"" />
  </ItemGroup>

</Project>
";

        protected override void Execute(byte[] encodedFile)
        {
            var result = new CsProjReader().ReadDependencies(encodedFile);
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo("Antlr", "bootstrap");
        }

        [Fact]
        void CsProjReader_HasRemoveAttribute_ReturnsValuesFromIncludeAttributesOnly()
        {
            // See https://github.com/aspnet/EntityFrameworkCore/blob/0f4340b82a66944b66a75f6e6949b473984a0ced/eng/common/internal/Tools.csproj#L10
            var result = new CsProjReader().ReadDependencies(Encoding.UTF8.GetBytes(@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <ItemGroup>
    <PackageReference Remove=""@(PackageReference)"" />
    <PackageReference Include=""First"" />
    <PackageReference Include=""Second"" />
  </ItemGroup>
</Project>
"));
            result.Should().BeEquivalentTo("First", "Second");
        }
    }

}