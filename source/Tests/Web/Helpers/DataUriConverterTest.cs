using System.Runtime.CompilerServices;
using System.Text;
using Assent;
using ICanHasDotnetCore.Web.Helpers;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests.Web.Helpers
{
    public class DataUriConverterTest
    {
        [Test]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CanConvert()
        {
            var result = DataUriConverter.ConvertFrom(
                  "data:application/xml;base64,77u/PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjxwYWNrYWdlcz4NCiAgPHBhY2thZ2UgaWQ9IlNlcmlsb2ciIHZlcnNpb249IjEuNS4xNCIgdGFyZ2V0RnJhbWV3b3JrPSJuZXQ0NjEiIC8+DQo8L3BhY2thZ2VzPg==");
            this.Assent(Encoding.UTF8.GetString(result));
        }
    }
}