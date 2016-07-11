. Database.exe
if($LastExitCode -ne 0)
{
	throw "Error occured, return code $LastExitCode"
}