using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ICanHasDotnetCore.Web.Plumbing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ICanHasDotnetCore.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(LogEventLevel.Information)
                .WriteTo.Seq(Configuration["Seq:Url"], apiKey: Configuration["Seq:ApiKey"])
                .Enrich.WithProperty("Application", "ICanHasDotnetCore")
                .Enrich.WithProperty("Environment", env.EnvironmentName)
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Create the container builder.
            var builder = new ContainerBuilder();

            // Register dependencies, populate the services from
            // the collection, and build the container.
            builder.RegisterInstance(Configuration);
            builder.RegisterAssemblyModules(typeof(Startup).Assembly);
            builder.Populate(services);
            var container = builder.Build();

            // Return the IServiceProvider resolved from the container.
            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddSerilog();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMiddleware<RedirectWwwMiddleware>()
                .UseMiddleware<RedirectHttpMiddleware>()
                .UseStaticFiles()
                .UseMvc();
        }

    }
}
