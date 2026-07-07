using System.Text;

namespace Cooperativa.Api.Servicos;

public static class CobolMonitor
{
    public static async Task EscreverAsync(
        string raiz,
        string texto,
        CancellationToken cancellationToken = default
    )
    {
        var runtime = Path.Combine(raiz, "runtime");
        Directory.CreateDirectory(runtime);

        var log = Path.Combine(runtime, "cobol-monitor.log");

        await File.AppendAllTextAsync(
            log,
            texto + Environment.NewLine,
            Encoding.UTF8,
            cancellationToken
        );
    }

    public static async Task EscreverArquivoAsync(
        string raiz,
        string titulo,
        string caminho,
        CancellationToken cancellationToken = default
    )
    {
        await EscreverAsync(raiz, titulo, cancellationToken);
        await EscreverAsync(raiz, "------------------------------------------------------------", cancellationToken);

        if (File.Exists(caminho))
        {
            var conteudo = await File.ReadAllTextAsync(
                caminho,
                Encoding.ASCII,
                cancellationToken
            );

            await EscreverAsync(raiz, conteudo, cancellationToken);
        }
        else
        {
            await EscreverAsync(raiz, "Arquivo nao encontrado.", cancellationToken);
        }

        await EscreverAsync(raiz, "------------------------------------------------------------", cancellationToken);
    }
}