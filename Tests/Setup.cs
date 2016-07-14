using NUnit.Framework;
using Serilog;

namespace ICanHasDotnetCore.Tests
{
    [SetUpFixture]
    public class Setup
    {
        [SetUp]
        public void RunBeforeAnyTests()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.LiterateConsole().CreateLogger();
        }
    }
}