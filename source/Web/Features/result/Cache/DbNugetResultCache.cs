using System;
using System.Data.SqlClient;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Web.Features.Statistics;
using Microsoft.Extensions.Configuration;
using NuGet;
using Serilog;
using System.Linq;
using System.Runtime.Versioning;

namespace ICanHasDotnetCore.Web.Features.result.Cache
{

    public class DbNugetResultCache : INugetResultCache
    {
        private readonly string _connectionString;

        public DbNugetResultCache(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionString"];
            if (string.IsNullOrWhiteSpace(_connectionString))
                throw new Exception("The configuration setting 'ConnectionString' is not set");
        }

        public Option<NugetPackage> Get(string id, SemanticVersion version)
        {
            try
            {


                const string sql = "SELECT * FROM dbo.NugetResultCache WHERE Id = @Id AND Version = @Version";
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("Id", id);
                        cmd.Parameters.AddWithValue("Version", version.ToNormalizedString());
                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            var dependencies = ((string)reader["Dependencies"])
                                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                            var frameworks = ((string)reader["Frameworks"])
                                .Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(n => new FrameworkName(n)).ToArray();

                            var supportType = (SupportType)Enum.Parse(typeof(SupportType), (string)reader["SupportType"]);

                            return new NugetPackage(
                                (string)reader["Id"],
                                dependencies,
                                supportType,
                                SemanticVersion.Parse((string)reader["Version"]),
                                frameworks
                            )
                            {
                                ProjectUrl = reader["ProjectUrl"] as string
                            };
                        }
                        else
                        {
                            return Option<NugetPackage>.ToNone;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not retrieve {id} {version} in Nuget Package Cache", id, version);
                return Option<NugetPackage>.ToNone;
            }
        }

        public void Store(NugetPackage package)
        {
            if (package.Version.None)
                return;
            try
            {
                const string sql = "INSERT INTO dbo.NugetResultCache (Id, SupportType, Version, ProjectUrl, Dependencies, Frameworks) VALUES (@Id, @SupportType, @Version, @ProjectUrl, @Dependencies, @Frameworks)";
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("Id", package.Id);
                        cmd.Parameters.AddWithValue("SupportType", package.SupportType.ToString());
                        cmd.Parameters.AddWithValue("Version", package.Version.Value.ToNormalizedString());
                        cmd.Parameters.AddWithValue("ProjectUrl", (object)package.ProjectUrl ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("Dependencies", string.Join("|", package.Dependencies));
                        cmd.Parameters.AddWithValue("Frameworks", string.Join("|", package.Frameworks.Select(f => f.FullName)));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not store {id} {version} in Nuget Package Cache", package.Id, package.Version.Value);
            }
        }
    }
}