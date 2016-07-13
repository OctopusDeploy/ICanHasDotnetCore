using ICanHasDotnetCore.Web.Helpers;
using Xunit;
using FluentAssertions;
using ApprovalTests;
using ApprovalTests.Reporters;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tests.DNC.Web.Helpers
{
    [UseReporter(typeof(DiffReporter))]
    public class DataUriConverterTest
    {
        [Fact]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CanConvert()
        {
            var result = DataUriConverter.ConvertFrom(
                  "data:application/xml;base64,77u/PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjxwYWNrYWdlcz4NCiAgPHBhY2thZ2UgaWQ9IlNlcmlsb2ciIHZlcnNpb249IjEuNS4xNCIgdGFyZ2V0RnJhbWV3b3JrPSJuZXQ0NjEiIC8+DQo8L3BhY2thZ2VzPg==");
            Approvals.Verify(Encoding.UTF8.GetString(result));
        }
    }
}