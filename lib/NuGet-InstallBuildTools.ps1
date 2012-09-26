$currentPath = Split-Path $MyInvocation.MyCommand.Path

& "$currentPath\nuget" update -self
& "$currentPath\nuget" install NUnit.Runners
& "$currentPath\nuget" install VersionTasks

Write-Host "Press any key to continue . . ."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")