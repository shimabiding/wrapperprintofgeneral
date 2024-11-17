Set-StrictMode -Version 2.0

$refAssem = @(
    "System.Windows.Forms"
    "System.ComponentModel"
    "System.Drawing"
    "System.Runtime.InteropServices"
    "UIAutomationTypes"
    "UIAutomationClient"
    "WindowsBase"
)

Add-Type -Path .\ma.cs `
-OutputAssembly ma.exe `
-ReferencedAssemblies $refAssem `
 <#`
-OutputType WindowsApplication#>

Write-Host "success"

.\ma.exe
