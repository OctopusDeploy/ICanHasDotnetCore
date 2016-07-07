Param(
    [Parameter(Position=1)]
    [string] $version = "0.0.0.0"
)

$pwd = pwd
Add-Type -assembly "system.io.compression.filesystem"


if((Test-Path ICanHasDotnetCore.sln) -eq $false)
{
    throw "This script has to be run from the solution directory"
}

if((Test-Path Publish) -eq $true)
{
    "Removing Publish Directory"
    Remove-Item Publish -Recurse
}

function CheckExit()
{
    "Exit Code $LastExitCode"
	if($LastExitCode -ne 0)
	{
		throw "Error occured, return code $LastExitCode"
	}
}


"Restoring Packages"
. dotnet restore
CheckExit

"Building Tests"
. dotnet build Tests
CheckExit

Push-Location
cd Web

"NPM Install"
npm install
CheckExit

"Gulp"
gulp release
Pop-Location
CheckExit

"Publish Web"
. dotnet publish Web -o "$pwd\Publish\Web" -c Release
CheckExit


"Pack Web"
. Tools\Octo.exe pack --id ICanHasDotnetCore.Web --version $version --basePath Publish\Web --format zip --outFolder Publish
CheckExit


"Publish Console"
. dotnet publish Console -o "$pwd\Publish\Console" -r active -c Release
CheckExit

"Zip Console" 
New-Item "Publish\ConsoleZip" -ItemType Directory
[io.compression.zipfile]::CreateFromDirectory("$pwd\Publish\Console", "$pwd\Publish\ConsoleZip\Console.$version.zip")

"Pack Console"
. Tools\Octo.exe pack --id ICanHasDotnetCore.Console --version $version --basePath Publish\ConsoleZip\ --format zip --outFolder Publish
CheckExit