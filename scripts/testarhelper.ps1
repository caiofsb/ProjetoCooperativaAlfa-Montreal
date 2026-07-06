.\scripts\gerarentradacbl.ps1
.\scripts\rodarcobol.ps1

Write-Host ""
Write-Host "Entrada enviada ao Helper:"

if (Test-Path runtime\helper-entrada.txt) {
    Get-Content runtime\helper-entrada.txt -Raw
} else {
    Write-Host "Arquivo runtime\helper-entrada.txt nao foi criado."
}

Write-Host ""
Write-Host "Saida recebida do Helper:"

if (Test-Path runtime\helper-saida.txt) {
    Get-Content runtime\helper-saida.txt -Raw
} else {
    Write-Host "Arquivo runtime\helper-saida.txt nao foi criado."
}