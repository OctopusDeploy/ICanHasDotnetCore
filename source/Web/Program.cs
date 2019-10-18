using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace ICanHasDotnetCore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .ConfigureAppConfiguration((context, configuration) =>
                {
                    configuration.SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((context, logging) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(LogEventLevel.Debug)
                        .WriteTo.Seq(context.Configuration["Seq:Url"], apiKey: context.Configuration["Seq:ApiKey"])
                        .Enrich.WithProperty("Application", "ICanHasDotnetCore")
                        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                        .CreateLogger();
                    logging.AddSerilog(Log.Logger);
                })
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            
            host.Run();
        }
    }
}
