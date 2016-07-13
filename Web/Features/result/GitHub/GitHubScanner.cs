using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octokit;
using Serilog;
using System.Linq;
using System.Text;
using ICanHasDotnetCore.Web.Plumbing.Extensions;
using Microsoft.Extensions.Configuration;
using Octokit.Internal;

namespace ICanHasDotnetCore.Web.Features.Result.GitHub
{
    public class GitHubScanner
    {
        private static readonly string AssemblyVersion = typeof(GitHubScanner).Assembly.GetName().Version.ToString();
        private static string _token;

        public GitHubScanner(IConfigurationRoot configuration)
        {
            _token = configuration["GitHubToken"];
        }

        public async Task<Result<PackagesFileData[]>> Scan(string repoId)
        {
            try
            {
                var match = Regex.Match(repoId, @"^\W*(\w+)/(\w+)\W*$");
                if (!match.Success)
                    return Result<PackagesFileData[]>.Failed($"{repoId} is not recognised as a GitHub repository name");

                var owner = match.Groups[1].Value;
                var name = match.Groups[2].Value;

                return await Scan(owner, name);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception scanning repository {name}", repoId);
                return Result<PackagesFileData[]>.Failed($"Something didn't quite right. The error has been logged.");
            }
        }

        private async Task<Result<PackagesFileData[]>> Scan(string owner, string name)
        {
            try
            {
                var client = GetClient();
                var contents = await GetContentsRecursive(client, owner, name);

                return contents
                    .Select(c => new PackagesFileData(c.Name, Encoding.UTF8.GetBytes(c.Content)))
                    .ToArray();
            }
            catch (NotFoundException nfe) when (nfe.Message == $"repos/{owner}/{name}/commits was not found.")
            {
                return Result<PackagesFileData[]>.Failed($"{owner}/{name} does not exist or is not publically accessible");
            }
        }

        private GitHubClient GetClient()
        {
            return new GitHubClient(
                new ProductHeaderValue("ICanHasDot.net", AssemblyVersion),
                 new InMemoryCredentialStore(_token == null ? Credentials.Anonymous : new Credentials(_token)
                )
            );
        }

        private async Task<IReadOnlyList<RepositoryContent>> GetContentsRecursive(GitHubClient client, string owner, string name)
        {
            var commits = await client.Repository.Commit.GetAll(owner, name);
            var head = commits.First();
            var treeResponse = await client.Git.Tree.GetRecursive(owner, name, head.Sha);

            if (treeResponse.Truncated)
                Log.Warning("Result truncated for {owner}/{name}", owner, name);

            var getFileTasks = treeResponse.Tree
                .Where(t => t.Type == TreeType.Blob)
                .Where(t => t.Path.EndsWith("/packages.config", StringComparison.InvariantCultureIgnoreCase))
                .Select(t => client.Repository.Content.GetAllContents(owner, name, t.Path))
                .ToArray();

            return (await Task.WhenAll(getFileTasks))
                .SelectMany(r => r)
                .ToArray();

        }
    }
}