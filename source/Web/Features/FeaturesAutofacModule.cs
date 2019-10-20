using System;
using Autofac;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Web.Configuration;
using ICanHasDotnetCore.Web.Database;
using ICanHasDotnetCore.Web.Features.result.Cache;
using ICanHasDotnetCore.Web.Features.result.GitHub;
using ICanHasDotnetCore.Web.Features.Statistics;
using Microsoft.EntityFrameworkCore;

namespace ICanHasDotnetCore.Web.Features
{
    public class FeaturesAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StatisticsRepository>().As<IStatisticsRepository>().SingleInstance();
            builder.RegisterType<GitHubScanner>().AsSelf().SingleInstance();
            builder.Register(context =>
            {
                var dbSettings = context.Resolve<IDatabaseSettings>();
                var optionsBuilder = new DbContextOptionsBuilder();
                switch (dbSettings.Provider)
                {
                    case DbProvider.Sqlite:
                        optionsBuilder.UseSqlite(dbSettings.ConnectionString);
                        break;
                    case DbProvider.SqlServer:
                        optionsBuilder.UseSqlServer(dbSettings.ConnectionString);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return optionsBuilder.Options;
            }).SingleInstance();
            builder.RegisterType<AppDbContext>().AsSelf().InstancePerDependency();
            builder.RegisterType<DbNugetResultCache>().As<INugetResultCache>().SingleInstance();
        }
    }
}