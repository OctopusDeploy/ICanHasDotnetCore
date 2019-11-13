using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ICanHasDotnetCore.Plumbing;

namespace ICanHasDotnetCore.NugetPackages
{
    public abstract class EmbeddedResourceRepository
    {
        protected EmbeddedResourceRepository(string fileName)
        {
            using var resourceStream = typeof(KnownReplacementsRepository).Assembly.GetManifestResourceStream(fileName);
            if (resourceStream == null)
                throw new FileNotFoundException($"The embedded resource '{fileName}' was not found", fileName);
            using var reader = new StreamReader(resourceStream);
            All = JsonSerializer.Deserialize<MoreInformation[]>(reader.ReadToEnd(), new JsonSerializerOptions {PropertyNameCaseInsensitive = true})
                .OrderBy(r => r.Id)
                .ThenBy(r => r.StartsWith) // false first
                .ToArray();
        }

        public IReadOnlyList<MoreInformation> All { get; }

        public Option<MoreInformation> Get(string id) => All.FirstOrNone(k => k.AppliesTo(id));
    }
}