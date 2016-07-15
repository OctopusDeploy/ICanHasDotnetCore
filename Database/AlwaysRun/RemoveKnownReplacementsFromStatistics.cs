using System;
using ICanHasDotnetCore.NugetPackages;
using System.Data.SqlClient;

namespace ICanHasDotnetCore.Database.AlwaysRun
{
    public class RemoveKnownReplacementsFromStatistics
    {
        public static void Run(SqlConnection connection)
        {
            Console.WriteLine("Removing Known Replacements from Package Statistics");
            foreach (var replacement in KnownReplacement.All)
            {
                var sql = replacement.StartsWith
                    ? "DELETE FROM PackageStatistics WHERE Name like @name + '%'"
                    : "DELETE FROM PackageStatistics WHERE Name = @name";
                using (var cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@name", replacement.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}