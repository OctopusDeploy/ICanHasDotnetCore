using Xunit;
using Serilog;

namespace ICanHasDotnetCore.Tests
{
    public class Setup
    {
        public Setup()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger();
        }
    }


    [CollectionDefinition("")]
    public class GlobalCollection : ICollectionFixture<Setup>
    {
        
    }
}