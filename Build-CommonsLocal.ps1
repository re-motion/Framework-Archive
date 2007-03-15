set-alias nant "C:\Program Files\NAnt\bin.net-2.0\nant.exe";

nant "-f:Commons.build" "-D:solution.global-dir=\Development\global" "-t:net-2.0" "-nologo" `
    "-D:build.update.assembly-info=false" `
    cleantemp `
    resources debug;

if ($LastExitCode -ne 0) 
{ 
  [System.Console]::ReadKey($false);
  throw "Build Commons has failed."; 
}

nant "-f:Commons.build" "-D:solution.global-dir=\Development\global" "-t:net-2.0" "-nologo" `
    cleantemp `
    sourcezip zip `
    securityManager-sourcezip securityManager-zip;

[System.Console]::ReadKey($false);
