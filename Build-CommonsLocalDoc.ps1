set-alias nant "C:\Program Files\NAnt\bin.net-2.0\nant.exe";

nant "-f:Commons.build" "-D:solution.global-dir=\Development\global" "-D:build.temp.root=\Temp\CommonsLocal" "-t:net-2.0" "-nologo" `
    "-D:build.update.assembly-info=false" `
    cleantemp `
    doc-internal;

if ($LastExitCode -ne 0) 
{ 
  [System.Console]::ReadKey($false);
  throw "Build Commons has failed."; 
}

[System.Console]::ReadKey($false);
