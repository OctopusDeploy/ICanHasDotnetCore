using System.Runtime.CompilerServices;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ICanHasDotnetCore.Output;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests
{
    public class EndToEndTests
    {
        [Test]
        [UseReporter(typeof(DiffReporter))]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Test()
        {
            var contents = @"<?xml version=""1.0"" encoding=""utf-8""?>
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

            var result = PackageCompatabilityInvestigator.Create()
                .Go(new[]
                {
                    new PackagesFileData("MyPackages", Encoding.UTF8.GetBytes(contents))
                })
                .Result;

            Approvals.Verify(TreeOutputFormatter.Format(result));
        }

    }
}