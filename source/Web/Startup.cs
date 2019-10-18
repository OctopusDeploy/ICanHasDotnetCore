using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ICanHasDotnetCore.Web.Plumbing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICanHasDotnetCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMiddleware<RedirectWwwMiddleware>()
                .UseMiddleware<RedirectHttpMiddleware>()
                .UseStaticFiles()
                .UseMvc();
        }

    }
}
