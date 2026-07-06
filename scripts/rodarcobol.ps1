New-Item -ItemType Directory -Force -Path runtime | Out-Null

Remove-Item runtime\PROJFINAL.exe -ErrorAction SilentlyContinue
Remove-Item runtime\saida.txt -ErrorAction SilentlyContinue
Remove-Item runtime\helper-entrada.txt -ErrorAction SilentlyContinue
Remove-Item runtime\helper-saida.txt -ErrorAction SilentlyContinue

dotnet build Cooperativa.slnx

cobc -I src\Cobol\Copy `
     -x `
     -o runtime\PROJFINAL.exe `
     src\Cobol\PROJETOFINAL.cbl

if ($LASTEXITCODE -ne 0) {
    throw "Erro ao compilar o COBOL."
}

if (!(Test-Path runtime\PROJFINAL.exe)) {
    throw "Executavel COBOL nao foi criado."
}

.\runtime\PROJFINAL.exe

if (!(Test-Path runtime\saida.txt)) {
    throw "Arquivo runtime\saida.txt nao foi criado."
}

Get-Content runtime\saida.txt -Raw