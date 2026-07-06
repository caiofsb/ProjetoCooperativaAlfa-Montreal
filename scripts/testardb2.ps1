param(
    [string]$Usuario = "db2inst1",
    [string]$Senha = "Senha123",
    [string]$Banco = "COOPDB"
)

$ErrorActionPreference = "Stop"

$env:HELPER_USAR_DB2 = "true"
$env:DB2_CONNECTION_STRING = "Driver={IBM DB2 ODBC DRIVER};Database=$Banco;Hostname=localhost;Port=50000;Protocol=TCPIP;Uid=$Usuario;Pwd=$Senha;"

Write-Host "HELPER_USAR_DB2=$env:HELPER_USAR_DB2"
Write-Host "Banco=$Banco"
Write-Host "Testando fluxo COBOL -> Helper -> ODBC -> DB2"

.\scripts\gerarentradacbl.ps1
.\scripts\rodarcobol.ps1

Write-Host ""
Write-Host "Saida final:"
Get-Content runtime\saida.txt -Raw

Write-Host ""
Write-Host "Entrada enviada ao Helper:"
Get-Content runtime\helper-entrada.txt -Raw

Write-Host ""
Write-Host "Saida recebida do Helper:"
Get-Content runtime\helper-saida.txt -Raw