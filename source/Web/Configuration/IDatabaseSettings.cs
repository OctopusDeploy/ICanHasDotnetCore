namespace ICanHasDotnetCore.Web.Configuration
{
    public interface IDatabaseSettings
    {
        DbProvider Provider { get; }
        string ConnectionString { get; }
    }
}