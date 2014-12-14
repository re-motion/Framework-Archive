param($installPath, $toolsPath, $package, $project)
$project = Get-Project
$p = New-Object NuGet.OptimizedZipPackage 'C:\Development\Remotion\trunk\Remotion\Remotion.Web.1.2.3.4-pre.nupkg'
$p.GetFiles() | Where-Object { $_.Path.StartsWith('res\','CurrentCultureIgnoreCase') } | { $_ }