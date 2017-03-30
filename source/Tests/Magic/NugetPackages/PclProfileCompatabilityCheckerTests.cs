using System.Collections.Generic;
using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic.NugetPackages
{
    public class PclProfileCompatabilityCheckerTests
    {

        public static IEnumerable<object[]> TestCases()
        {
            return new[]
            {
                CreateTestCase("Profile2", "net40+win8+sl4+wp7", false),
                CreateTestCase("Profile3", "net40+sl4", false),
                CreateTestCase("Profile4", "net45+sl4+win8+wp7", false),
                CreateTestCase("Profile5", "net40+win8", false),
                CreateTestCase("Profile6", "net403+win8", false),
                CreateTestCase("Profile7", "net45+win8", true),
                CreateTestCase("Profile14", "net40+sl5", false),
                CreateTestCase("Profile18", "net403+sl4", false),
                CreateTestCase("Profile19", "net403+sl5", false),
                CreateTestCase("Profile23", "net45+sl4", false),
                CreateTestCase("Profile24", "net45+sl5", false),
                CreateTestCase("Profile31", "win81+wp81", true),
                CreateTestCase("Profile32", "win81+wpa81", true),
                CreateTestCase("Profile36", "net40+sl4+win8+wp8", false),
                CreateTestCase("Profile37", "net40+sl5+win8", false),
                CreateTestCase("Profile41", "net403+sl4+win8", false),
                CreateTestCase("Profile42", "net403+sl5+win8", false),
                CreateTestCase("Profile44", "net451+win81", true),
                CreateTestCase("Profile46", "net45+sl4+win8", false),
                CreateTestCase("Profile47", "net45+sl5+win8", false),
                CreateTestCase("Profile49", "net45+wp8", true),
                CreateTestCase("Profile78", "net45+win8+wp8", true),
                CreateTestCase("Profile84", "wp81+wpa81", true),
                CreateTestCase("Profile88", "net40+sl4+win8+wp75", false),
                CreateTestCase("Profile92", "net40+win8+wpa81", false),
                CreateTestCase("Profile95", "net403+sl4+win8+wp7", false),
                CreateTestCase("Profile96", "net403+sl4+win8+wp75", false),
                CreateTestCase("Profile102", "net403+win8+wpa81", false),
                CreateTestCase("Profile104", "net45+sl4+win8+wp75", false),
                CreateTestCase("Profile111", "net45+win8+wpa81", true),
                CreateTestCase("Profile136", "net40+sl5+win8+wp8", false),
                CreateTestCase("Profile143", "net403+sl4+win8+wp8", false),
                CreateTestCase("Profile147", "net403+sl5+win8+wp8", false),
                CreateTestCase("Profile151", "net451+win81+wpa81", true),
                CreateTestCase("Profile154", "net45+sl4+win8+wp8", false),
                CreateTestCase("Profile157", "win81+wp81+wpa81", true),
                CreateTestCase("Profile158", "net45+sl5+win8+wp8", false),
                CreateTestCase("Profile225", "net40+sl5+win8+wpa81", false),
                CreateTestCase("Profile240", "net403+sl5+win8+wpa81", false),
                CreateTestCase("Profile255", "net45+sl5+win8+wpa81", false),
                CreateTestCase("Profile259", "net45+win8+wpa81+wp8", true),
                CreateTestCase("Profile328", "net40+sl5+win8+wpa81+wp8", false),
                CreateTestCase("Profile336", "net403+sl5+win8+wpa81+wp8", false),
                CreateTestCase("Profile344", "net45+sl5+win8+wpa81+wp8", false),
                CreateTestCase("Profile259 - win alias", "net45+win+wpa81+wp8", true),
                CreateTestCase("Profile259 - wpa alias", "net45+win8+wpa+wp8", true),
                CreateTestCase("Profile259 - different order", "net45+wp8+win8+wpa81", true),
            };
        }

        private static object[] CreateTestCase(string name, string profile, bool expectedResult) 
            => new object[] {name, profile, expectedResult};

        [Theory]
        [MemberData(nameof(TestCases))]
        public void PclCheckReturnsTheRightValue(string name, string profile, bool expected)
        {
            PclProfileCompatabilityChecker.Check(profile).Should().Be(expected);
        }
    }
}