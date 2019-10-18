using System;
using System.Data.SqlClient;
using System.Diagnostics;
using DbUp;
using ICanHasDotnetCore.Database.AlwaysRun;
using Microsoft.Extensions.Configuration;

namespace ICanHasDotnetCore.Database
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration["ConnectionString"];
            Console.WriteLine($"Connection String: {connectionString}");
            Console.WriteLine("Ensuring Database");
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
               .SqlDatabase(connectionString)
               .WithScriptsEmbeddedInAssembly(typeof(Program).Assembly)
               .LogToConsole()
               .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                Debugger.Break();
                return -1;
            }

            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                RemoveKnownReplacementsFromStatistics.Run(con);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
