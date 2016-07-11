using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
