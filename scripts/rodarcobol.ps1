$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path runtime | Out-Null

cobc -x -o runtime\PROJFINAL.exe src\Cobol\PROJETOFINAL.cbl

.\runtime\PROJFINAL.exe

Get-Content runtime\saida.txt