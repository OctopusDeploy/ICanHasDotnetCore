using FluentAssertions;
using ICanHasDotnetCore.SourcePackageFileReaders;

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

    }

}