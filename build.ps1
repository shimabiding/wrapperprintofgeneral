Set-StrictMode -Version 2.0

$refAssem = @(
    "System.Windows.Forms"
    "System.ComponentModel"
    "System.Drawing"
)

Add-Type .\Main.cs, .\form1.cs `
-OutputAssembly wgp.exe `
-ReferencedAssemblies $refAssem `
 <#`
-OutputType WindowsApplication#>
Write-Host "success"

.\wgp.exe
