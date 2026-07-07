@echo off
title Monitor COBOL - Cooperativa Financeira Alfa
color 0A
cls

echo ============================================================
echo      MONITOR COBOL - COOPERATIVA FINANCEIRA ALFA
echo ============================================================
echo.
echo Esta janela acompanha as operacoes executadas pelo COBOL.
echo.
echo Para encerrar, pare a API ou feche esta janela.
echo.
echo ============================================================
echo.

powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-Content -LiteralPath 'C:\Users\santo\ProjetoFinalModernizacaoCobol-Montreal\Cooperativa\runtime\cobol-apresentacao.log' -Wait"