using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assent;
using ICanHasDotnetCore.Investigator;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Output;
using ICanHasDotnetCore.SourcePackageFileReaders;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic
{
    [SuppressMessage("ReSharper", "VSTHRD200")]
    public class EndToEndTests
    {
        private const string Paket = @"source https://nuget.org/api/v2
// NuGet packages
nuget DotNetZip >= 1.9

// HTTP resources
http http://www.fssnip.net/1n decrypt.fs";

        private const string PackagesConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
<packages>
  <package id=""Autofac"" version=""3.5.2"" targetFramework=""net45"" />
  <package id=""NLog"" version=""4.0.1"" targetFramework=""net45"" />
  <package id=""Microsoft.Web.Xdt"" version=""2.1.1"" targetFramework=""net45"" />
 <package id=""Nancy"" version=""1.2.0"" targetFramework=""net45"" />
  <package id=""Nancy.Bootstrappers.Autofac"" version=""1.2.0"" targetFramework=""net45"" />
  <package id=""SharpZipLib"" version=""0.86.0"" targetFramework=""net45"" />
  <package id=""Sprache"" version=""2.0.0.46"" targetFramework=""net45"" />
  <package id=""SSH.NET"" version=""2014.4.6-beta4"" targetFramework=""net45"" />
</packages>";

        private const string ProjectJson = @"{
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

        [Fact(Skip = "Brittle, but useful when making changes, so keeping it")]
        public async Task EndToEndTest()
        {

            var result = await PackageCompatabilityInvestigator.Create(new NoNugetResultCache())
                .GoAsync(new[]
                {
                    new SourcePackageFile("PackagesConfig", SourcePackageFileReader.PackagesConfig, Encoding.UTF8.GetBytes(PackagesConfig)),
                    new SourcePackageFile("ProjectJson", SourcePackageFileReader.ProjectJson, Encoding.UTF8.GetBytes(ProjectJson)),
                    new SourcePackageFile("Paket", SourcePackageFileReader.Paket, Encoding.UTF8.GetBytes(Paket))
                }, CancellationToken.None);

            this.Assent(TreeOutputFormatter.Format(result));
        }

    }
}
