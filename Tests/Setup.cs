using Xunit;
using Serilog;

namespace ICanHasDotnetCore.Tests
{
    public class Setup : ICollectionFixture<Setup>
    {
        public Setup()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger();
        }
    }
}