using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.SourcePackageFileReaders;
using Microsoft.Extensions.Configuration;
using Octokit;
using Octokit.Internal;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.result.GitHub
{
    public class GitHubScanner
    {
        private static readonly string AssemblyVersion = typeof(GitHubScanner).Assembly.GetName().Version.ToString();
        private static string _token;

        public GitHubScanner(IConfigurationRoot configuration)
        {
            _token = configuration["GitHubToken"];
        }

        public async Task<Result<SourcePackageFile[]>> Scan(string repoId)
        {
            try
            {
                var repo = RepositoryId.Parse(repoId);
                if (repo.None)
                    return Result<SourcePackageFile[]>.Failed($"{repoId} is not recognised as a GitHub repository name");

                return await Scan(repo.Value);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception scanning repository {name}", repoId);
                return Result<SourcePackageFile[]>.Failed($"Something didn't quite right. The error has been logged.");
            }
        }

        private async Task<Result<SourcePackageFile[]>> Scan(RepositoryId repo)
        {
            try
            {
                var client = GetClient();
                var contents = await GetContentsRecursive(client, repo);

                return contents
                    .Select(c => new SourcePackageFile(c.Path.Contains("/") ? c.Path.Substring(0, c.Path.LastIndexOf("/")) : c.Path, c.Name, Encoding.UTF8.GetBytes(c.Content)))
                    .ToArray();
            }
            catch (NotFoundException nfe) when (nfe.Message == $"repos/{repo}/commits was not found.")
            {
                return Result<SourcePackageFile[]>.Failed($"{repo} does not exist or is not publically accessible");
            }
        }

     

        private GitHubClient GetClient()
        {
            return new GitHubClient(
                new ProductHeaderValue("ICanHasDot.net", AssemblyVersion),
                 new InMemoryCredentialStore(string.IsNullOrEmpty(_token) ? Credentials.Anonymous : new Credentials(_token)
                )
            );
        }

        private async Task<IReadOnlyList<RepositoryContent>> GetContentsRecursive(GitHubClient client, RepositoryId repo)
        {
            var commits = await client.Repository.Commit.GetAll(repo.Owner, repo.Name);
            var head = commits.First();
            var treeResponse = await client.Git.Tree.GetRecursive(repo.Owner, repo.Name, head.Sha);

            if (treeResponse.Truncated)
                Log.Warning("Result truncated for {repo}", repo);

            var getFileTasks = treeResponse.Tree
                .Where(t => t.Type == TreeType.Blob)
                .Where(t => t.Path.EndsWith("/packages.config", StringComparison.InvariantCultureIgnoreCase))
                .Select(t => client.Repository.Content.GetAllContents(repo.Owner, repo.Name, t.Path))
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
                if(match.Success)
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