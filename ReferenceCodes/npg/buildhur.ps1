Set-StrictMode -Version 2.0

$refAssem = @(
    "System.Windows.Forms"
    "System.ComponentModel"
    "System.Drawing"
    "System.Data"
    ".\ReferenceCodes\npg\Npgsql.dll"
    ".\ReferenceCodes\npg\Mono.Security.dll"
)

Add-Type -Path .\Program.cs `
-OutputAssembly n.exe `
-ReferencedAssemblies $refAssem
 <#`
-OutputType WindowsApplication#>
Write-Host "success"

.\n.exe
