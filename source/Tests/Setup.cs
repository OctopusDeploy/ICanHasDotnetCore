using NUnit.Framework;
using Serilog;

namespace ICanHasDotnetCore.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger();
        }
    }
}