$currentPath = Split-Path $MyInvocation.MyCommand.Path
$msbuildFile = Join-Path $currentPath src\TeamPilgrim.msbuild
$logging = "/l:FileLogger,Microsoft.Build.Engine;verbosity=detailed;logfile=build.log"

& "$(get-content env:windir)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $msbuildFile /t:Build $logging

Write-Host "Press any key to continue . . ."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")