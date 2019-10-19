namespace ICanHasDotnetCore.Web.Configuration
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public DbProvider Provider { get; set; }
        public string ConnectionString { get; set; }
    }
}