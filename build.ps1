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

"Setting Version"
"Version is $version"
(Get-Content Web\Features\Version.ts).replace('0.0.0.0', $version) | Set-Content Web\Features\Version.ts

"Restoring Packages"
. dotnet restore
CheckExit

"Building Tests"
. dotnet build Tests
CheckExit

Push-Location
cd Web

"NPM Install"
. npm install
CheckExit

"Gulp"
. .\node_modules\.bin\gulp release
Pop-Location
CheckExit


"Publish Web"
. dotnet publish Web -o "$pwd\Publish\Web" -c Release
CheckExit


"Publish Console"
. dotnet publish Console -o "$pwd\Publish\Console" -c Release
CheckExit

"Zip Console" 
New-Item "$pwd\Publish\Web\wwwroot\Downloads" -ItemType Directory
[io.compression.zipfile]::CreateFromDirectory("$pwd\Publish\Console", "$pwd\Publish\Web\wwwroot\Downloads\ICanHasDotnetCore.zip")

"Pack Web"
. Tools\Octo.exe pack --id ICanHasDotnetCore.Web --version $version --basePath Publish\Web --format zip --outFolder Publish
CheckExit

"Publish Database"
. dotnet publish Database -o "$pwd\Publish\Database" -c Release
CheckExit

"Pack Database"
. Tools\Octo.exe pack --id ICanHasDotnetCore.Database --version $version --basePath Publish\Database --format zip --outFolder Publish
CheckExit
