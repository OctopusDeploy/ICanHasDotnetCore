using ICanHasDotnetCore.Plumbing.Extensions;

namespace ICanHasDotnetCore.NugetPackages
{
    public class MoreInformation
    {
        public string Id { get; }
        public string Message { get; set; }
        public string LinkText { get; set; }
        public string Url { get; set; }
        public bool StartsWith { get; set; }

        public MoreInformation(string id)
        {
            Id = id;
        }

        public bool AppliesTo(string id) => StartsWith ? id.StartsWithOrdinalIgnoreCase(Id) : id.EqualsOrdinalIgnoreCase(Id);

    }
}