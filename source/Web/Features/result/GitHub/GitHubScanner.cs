using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.SourcePackageFileReaders;
using Octokit;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.result.GitHub
{
    public class GitHubScanner
    {
        private readonly IGitHubClient _gitHubClient;

        public GitHubScanner(IGitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient ?? throw new ArgumentNullException(nameof(gitHubClient));
        }

        public async Task<Result<SourcePackageFile[]>> ScanAsync(string repoId)
        {
            try
            {
                var repo = RepositoryReference.Parse(repoId);
                if (repo.None)
                    return Result<SourcePackageFile[]>.Failed($"{repoId} is not recognised as a GitHub repository name");

                return await ScanAsync(repo.Value);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception scanning repository {name}", repoId);
                return Result<SourcePackageFile[]>.Failed($"Something didn't go quite right. The error has been logged.");
            }
        }

        private async Task<Result<SourcePackageFile[]>> ScanAsync(RepositoryReference repo)
        {
            try
            {
                var contents = await GetContentsRecursiveAsync(repo);

                return contents
                    .Select(c => new SourcePackageFile(c.Path.Contains("/") ? c.Path.Substring(0, c.Path.LastIndexOf("/", StringComparison.Ordinal)) : "<root>", Path.GetFileName(c.Path), c.Content))
                    .ToArray();
            }
            catch (NotFoundException)
            {
                return Result<SourcePackageFile[]>.Failed($"{repo} does not exist or is not publicly accessible");
            }
        }

        private async Task<string> GetReferenceAsync(RepositoryReference repo)
        {
            if (repo.Reference.Some)
                return repo.Reference.Value;
            var commits = await _gitHubClient.Repository.Commit.GetAll(repo.Owner, repo.Name, new ApiOptions {PageCount = 1, PageSize = 1});
            return commits.First().Sha;
        }

        private async Task<(string Path, byte[] Content)> GetRawContentAsync(string owner, string name, string path, string reference)
        {
            var content = await _gitHubClient.Repository.Content.GetRawContentByRef(owner, name, path, reference);
            return (Path: path, Content: content);
        }

        private async Task<IReadOnlyList<(string Path, byte[] Content)>> GetContentsRecursiveAsync(RepositoryReference repo)
        {
            var reference = await GetReferenceAsync(repo);
            var treeResponse = await _gitHubClient.Git.Tree.GetRecursive(repo.Owner, repo.Name, reference);

            if (treeResponse.Truncated)
                Log.Warning("Result truncated for {repo}", repo);

            var getFileTasks = treeResponse.Tree
                .Where(t => t.Type == TreeType.Blob)
                .Where(t =>
                    SourcePackageFileReader.SupportedFiles.Any(f =>
                        t.Path.Equals(f, StringComparison.OrdinalIgnoreCase) ||
                        t.Path.EndsWith($"/{f}", StringComparison.OrdinalIgnoreCase)
                    ) ||
                    SourcePackageFileReader.SupportedExtensions.Any(e =>
                        t.Path.EndsWith(e, StringComparison.OrdinalIgnoreCase)
                    )
                )
                .Select(t => GetRawContentAsync(repo.Owner, repo.Name, t.Path, reference))
                .ToArray();

            return (await Task.WhenAll(getFileTasks))
                .ToArray();

        }

        public class RepositoryReference
        {

            public RepositoryReference(string owner, string name, Option<string> reference)
            {
                Owner = owner;
                Name = name;
                Reference = reference;
            }
            public static Option<RepositoryReference> Parse(string repoId)
            {
                var match = Regex.Match(repoId, @"^\W*([\w\-_\.]+)/([\w\-_\.]+)\W*(?:@(.*))?$");
                if (match.Success)
                {
                    var reference = match.Groups[3].Value;
                    return new RepositoryReference(match.Groups[1].Value, match.Groups[2].Value, reference != "" ? reference.Some() : reference.None());
                }
                return Option<RepositoryReference>.ToNone;
            }

            public string Owner { get; }
            public string Name { get; }
            public Option<string> Reference { get; }

            public override string ToString()
            {
                return Reference.Some ? $"{Owner}/{Name}@{Reference.Value}" : $"{Owner}/{Name}";
            }
        }
    }
}