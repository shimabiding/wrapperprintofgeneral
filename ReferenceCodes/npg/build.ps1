<#Add-Type -Path ".\Npgsql.dll";

$conn_str = "Server=localhost;Port=5432;User ID=postgres;Password=pass;Database=postgres";
$conn = New-Object Npgsql.NpgsqlConnection $conn_str;
$conn | Out-string
$conn.Open();
Write-Host "aj";
#>

Add-Type -Path .\Program.cs `
-OutputAssembly MyApp.exe `
-ReferencedAssemblies `
System.Windows.Forms,`
System.Drawing,`
System.Threading.Tasks,`
System.ComponentModel,`
System.Data,`
.\Npgsql.dll,`
.\Mono.Security.dll <#`
-OutputType WindowsApplication
#>