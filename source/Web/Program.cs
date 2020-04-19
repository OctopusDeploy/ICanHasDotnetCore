using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ICanHasDotnetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .UseSerilog((context, configuration) => configuration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                    .WriteTo.Console()
                    .WriteTo.Seq(context.Configuration["Seq:Url"], apiKey: context.Configuration["Seq:ApiKey"])
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "ICanHasDotnetCore")
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Filter.ByExcluding(c => c.Exception?.GetBaseException() is OperationCanceledException)
                )
                .Build();

            host.Run();
        }
    }
}
