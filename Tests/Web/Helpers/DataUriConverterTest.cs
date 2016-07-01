using ICanHasDotnetCore.Web.Helpers;
using NUnit.Framework;
using FluentAssertions;
using ApprovalTests;
using ApprovalTests.Reporters;

namespace Tests.DNC.Web.Helpers
{
    [UseReporter(typeof(DiffReporter))]
    public class DataUriConverterTest
    {
        [Test]
        public void CanConvert()
        {
            var result = DataUriConverter.ConvertFrom(
                  "data:application/xml;base64,77u/PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjxwYWNrYWdlcz4NCiAgPHBhY2thZ2UgaWQ9IlNlcmlsb2ciIHZlcnNpb249IjEuNS4xNCIgdGFyZ2V0RnJhbWV3b3JrPSJuZXQ0NjEiIC8+DQo8L3BhY2thZ2VzPg==");
            Approvals.Verify(result);
        }
    }
}