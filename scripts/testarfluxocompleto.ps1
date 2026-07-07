param(
    [string]$Usuario = "db2inst1",
    [string]$Senha = "Senha123",
    [string]$Banco = "COOPDB"
)

$ErrorActionPreference = "Stop"

function Executar {
    param(
        [string]$Descricao,
        [scriptblock]$Comando
    )

    Write-Host ""
    Write-Host $Descricao

    & $Comando

    if ($LASTEXITCODE -ne 0) {
        throw "Falha na etapa: $Descricao"
    }
}

Write-Host "============================================"
Write-Host "TESTE COMPLETO DO PROJETO"
Write-Host "============================================"

Executar "[1] Build da solution" {
    dotnet build Cooperativa.slnx
}

Executar "[2] Testes automatizados" {
    dotnet test Cooperativa.slnx
}

Write-Host ""
Write-Host "[3] Compilando COBOL"

cobc -I src\Cobol\Copy `
     -x `
     -o runtime\PROJFINAL.exe `
     src\Cobol\PROJETOFINAL.cbl

if ($LASTEXITCODE -ne 0) {
    throw "Falha ao compilar o COBOL."
}

Write-Host ""
Write-Host "[4] Ativando DB2 no Helper"

$env:HELPER_USAR_DB2 = "true"
$env:DB2_CONNECTION_STRING = "Driver={IBM DB2 ODBC DRIVER};Database=$Banco;Hostname=localhost;Port=50000;Protocol=TCPIP;Uid=$Usuario;Pwd=$Senha;"

Write-Host "Banco=$Banco"
Write-Host "Usuario=$Usuario"

Executar "[5] Testando COBOL -> Helper -> DB2" {
    .\scripts\testardb2.ps1 -Usuario $Usuario -Senha $Senha -Banco $Banco
}

Write-Host ""
Write-Host "============================================"
Write-Host "TESTE COMPLETO FINALIZADO COM SUCESSO"
Write-Host "============================================"