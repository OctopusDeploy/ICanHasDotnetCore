using System.Collections.Generic;
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.NugetPackages
{
    public class MoreInformation
    {
        public string Id { get; }
        public string Message { get; set; }
        public string LinkText { get; set; }
        public string Url { get; set; }
        public bool StartsWith { get; set; }

        private MoreInformation(string id)
        {
            Id = id;
        }

        public static readonly IReadOnlyList<MoreInformation> All = new[]
        {
            new MoreInformation("Antlr")
            {
                LinkText = "GitHub Issue tracking .NET Core support",
                Url = "https://github.com/antlr/antlrcs/issues/42"
            }
        };

        public static Option<MoreInformation> Get(string id)
        {
            return All.FirstOrNone(m => m.Id == id);
        }
    }
}