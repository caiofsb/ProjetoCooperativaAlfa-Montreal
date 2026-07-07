using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Cooperativa.Api.Servicos;

public sealed class MonitorCobolPromptServico : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var raiz = AcharRaiz();
        var runtime = Path.Combine(raiz, "runtime");

        Directory.CreateDirectory(runtime);

        var log = Path.Combine(runtime, "cobol-monitor.log");

        File.WriteAllText(
            log,
            """
            ============================================================
                 MONITOR COBOL - COOPERATIVA FINANCEIRA ALFA
            ============================================================

            Esta janela mostra cada operacao executada pelo COBOL.

            Fluxo:
            Razor Pages -> API .NET -> COBOL -> Helper -> ODBC -> DB2

            Aguardando operacoes...

            """,
            Encoding.UTF8
        );

        Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/k powershell -NoProfile -Command \"Get-Content -LiteralPath '{log}' -Wait\"",
            UseShellExecute = true
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static string AcharRaiz()
    {
        var pasta = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (pasta is not null)
        {
            if (File.Exists(Path.Combine(pasta.FullName, "Cooperativa.slnx")) ||
                File.Exists(Path.Combine(pasta.FullName, "Cooperativa.sln")))
            {
                return pasta.FullName;
            }

            pasta = pasta.Parent;
        }

        return Directory.GetCurrentDirectory();
    }
}