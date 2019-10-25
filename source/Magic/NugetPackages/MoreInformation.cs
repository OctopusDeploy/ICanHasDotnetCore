using ICanHasDotnetCore.Plumbing.Extensions;

namespace ICanHasDotnetCore.NugetPackages
{
    public class MoreInformation
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string LinkText { get; set; }
        public string Url { get; set; }
        public bool StartsWith { get; set; }

        public bool AppliesTo(string id) => StartsWith ? id.StartsWithOrdinalIgnoreCase(Id) : id.EqualsOrdinalIgnoreCase(Id);

    }
}