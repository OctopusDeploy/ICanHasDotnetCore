using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var repo = RepositoryId.Parse(repoId);
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

        private async Task<Result<SourcePackageFile[]>> ScanAsync(RepositoryId repo)
        {
            try
            {
                var contents = await GetContentsRecursiveAsync(repo);

                return contents
                    .Select(c => new SourcePackageFile(c.Path.Contains("/") ? c.Path.Substring(0, c.Path.LastIndexOf("/")) : "<root>", c.Name, Encoding.UTF8.GetBytes(c.Content)))
                    .ToArray();
            }
            catch (NotFoundException nfe) when (nfe.Message == $"repos/{repo}/commits was not found.")
            {
                return Result<SourcePackageFile[]>.Failed($"{repo} does not exist or is not publically accessible");
            }
        }

        private async Task<IReadOnlyList<RepositoryContent>> GetContentsRecursiveAsync(RepositoryId repo)
        {
            var commits = await _gitHubClient.Repository.Commit.GetAll(repo.Owner, repo.Name, new ApiOptions {PageCount = 1, PageSize = 1});
            var head = commits.First();
            var treeResponse = await _gitHubClient.Git.Tree.GetRecursive(repo.Owner, repo.Name, head.Sha);

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
                .Select(t => _gitHubClient.Repository.Content.GetAllContents(repo.Owner, repo.Name, t.Path))
                .ToArray();

            return (await Task.WhenAll(getFileTasks))
                .SelectMany(r => r)
                .ToArray();

        }

        public class RepositoryId
        {

            public RepositoryId(string owner, string name)
            {
                Owner = owner;
                Name = name;
            }
            public static Option<RepositoryId> Parse(string repoId)
            {
                var match = Regex.Match(repoId, @"^\W*([\w\-_\.]+)/([\w\-_\.]+)\W*$");
                if (match.Success)
                    return new RepositoryId(match.Groups[1].Value, match.Groups[2].Value);
                return Option<RepositoryId>.ToNone;
            }

            public string Owner { get; }
            public string Name { get; }

            public override string ToString()
            {
                return $"{Owner}/{Name}";
            }
        }
    }
}