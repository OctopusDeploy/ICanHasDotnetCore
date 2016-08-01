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

        public DbNugetResultCache(IConfigurationRoot configuration)
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
                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            return new NugetPackage(
                                (string)reader["Id"],
                                ((string)reader["Dependencies"]).Split('|'),
                                (SupportType)Enum.Parse(typeof(SupportType), (string)reader["LatestSupportType"]),
                                SemanticVersion.Parse((string)reader["Version"]),
                                ((string)reader["Frameworks"]).Split('|').Select(n => new FrameworkName(n)).ToArray()
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
                const string sql = "INSERT INTO dbo.NugetResultCache (Id, SupportType, Version, ProjectUrl, Dependencies) VALUES (@Id, @SupportType, @Version, @ProjectUrl, @Dependencies, @Frameworks)";
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("Id", package.Id);
                        cmd.Parameters.AddWithValue("LatestSupportType", package.SupportType.ToString());
                        cmd.Parameters.AddWithValue("Version", package.Version.Value.ToNormalizedString());
                        cmd.Parameters.AddWithValue("ProjectUrl", package.ProjectUrl);
                        cmd.Parameters.AddWithValue("Dependencies", string.Join("|", package.Dependencies));
                        cmd.Parameters.AddWithValue("Frameworks", string.Join("|", package.Frameworks.Select(f => f.FullName)));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not store {id} {version} in Nuget Package Cache", package.Id, package.Version);
            }
        }
    }
}