$itemsToDelete = Get-ChildItem . -recurse -force -include *.suo,*.user,*.cache,*.docstates,bin,obj,build,_ReSharper.*

if ($itemsToDelete) {
    foreach ($item in $itemsToDelete) {
        Remove-Item $item.FullName -Force -Recurse -ErrorAction SilentlyContinue
        Write-Host "Deleted" $item.FullName
    }
}

Write-Host "Press any key to continue . . ."
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")