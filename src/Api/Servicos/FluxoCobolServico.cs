using System.Diagnostics;
using System.Text;
using Cooperativa.Contratos;

namespace Cooperativa.Api.Servicos;

public sealed class FluxoCobolServico : IFluxoCobolServico
{
    private const string CodigoErroSistema = "0500";
    private const int TempoLimiteSegundos = 10;

    public async Task<ResultadoCobol> ProcessarAsync(
        ClienteEntradaCobol entrada,
        CancellationToken cancellationToken = default
    )
    {
        var raizProjeto = EncontrarRaizProjeto();
        var pastaRuntime = Path.Combine(raizProjeto, "runtime");
        var arquivoEntrada = Path.Combine(pastaRuntime, "entrada.txt");
        var arquivoSaida = Path.Combine(pastaRuntime, "saida.txt");
        var executavelCobol = Path.Combine(pastaRuntime, "PROJFINAL.exe");

        Directory.CreateDirectory(pastaRuntime);

        if (!File.Exists(executavelCobol))
        {
            return ErroSistema(
                "Executavel COBOL nao encontrado em: " + executavelCobol
            );
        }

        var linhaEntrada = MontarLinhaEntrada(entrada);

        await File.WriteAllTextAsync(
            arquivoEntrada,
            linhaEntrada + Environment.NewLine,
            Encoding.ASCII,
            cancellationToken
        );

        if (File.Exists(arquivoSaida))
        {
            File.Delete(arquivoSaida);
        }

        var processo = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executavelCobol,
                WorkingDirectory = raizProjeto,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        try
        {
            processo.Start();

            using var limite = new CancellationTokenSource(
                TimeSpan.FromSeconds(TempoLimiteSegundos)
            );

            using var combinado = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                limite.Token
            );

            await processo.WaitForExitAsync(combinado.Token);
        }
        catch (OperationCanceledException)
        {
            return ErroSistema("Tempo limite ao executar COBOL.");
        }
        catch (Exception erro)
        {
            return ErroSistema("Falha ao executar COBOL: " + erro.Message);
        }

        if (processo.ExitCode != 0)
        {
            var erroCobol = await processo.StandardError.ReadToEndAsync(
                cancellationToken
            );

            return ErroSistema("COBOL retornou erro: " + erroCobol);
        }

        if (!File.Exists(arquivoSaida))
        {
            return ErroSistema(
                "Arquivo de saida do COBOL nao encontrado em: " + arquivoSaida
            );
        }

        var linhaSaida = await File.ReadAllTextAsync(
            arquivoSaida,
            Encoding.ASCII,
            cancellationToken
        );

        return InterpretarSaida(linhaSaida);
    }

    public static string MontarLinhaEntrada(ClienteEntradaCobol entrada)
    {
        return Ajustar(entrada.Operacao, 10) +
               Ajustar(entrada.Codigo, 6) +
               Ajustar(entrada.Nome, 30) +
               Ajustar(entrada.Email, 60) +
               Ajustar(entrada.Telefone ?? string.Empty, 11);
    }

public static ResultadoCobol InterpretarSaida(string conteudo)
{
    var linha = conteudo
        .Replace("\r", "")
        .Split('\n')
        .FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));

    if (string.IsNullOrWhiteSpace(linha))
    {
        return ErroSistema("Saida do COBOL vazia.");
    }

    var partes = linha.Split('|');

    if (partes.Length < 7)
    {
        return ErroSistema("Saida do COBOL invalida: " + linha);
    }

    var codigoRetorno = partes[0].Trim();
    var mensagem = partes[1].Trim();

    var cliente = new Cliente
    {
        Codigo = partes[3].Trim(),
        Nome = partes[4].Trim(),
        Email = partes[5].Trim(),
        Telefone = string.IsNullOrWhiteSpace(partes[6])
            ? null
            : partes[6].Trim()
    };

    return new ResultadoCobol
    {
        Sucesso = codigoRetorno == CodigoResposta.Sucesso,
        CodigoRetorno = codigoRetorno,
        Mensagem = mensagem,
        Cliente = cliente
    };
}

    private static string EncontrarRaizProjeto()
    {
        var pasta = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (pasta is not null)
        {
            var temSolution =
                File.Exists(Path.Combine(pasta.FullName, "Cooperativa.sln")) ||
                File.Exists(Path.Combine(pasta.FullName, "Cooperativa.slnx"));

            var temPastaSrc = Directory.Exists(
                Path.Combine(pasta.FullName, "src")
            );

            var temPastaRuntime = Directory.Exists(
                Path.Combine(pasta.FullName, "runtime")
            );

            if (temSolution && temPastaSrc && temPastaRuntime)
            {
                return pasta.FullName;
            }

            pasta = pasta.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    private static ResultadoCobol ErroSistema(string mensagem)
    {
        return new ResultadoCobol
        {
            Sucesso = false,
            CodigoRetorno = CodigoErroSistema,
            Mensagem = mensagem
        };
    }

    private static string Ajustar(string? valor, int tamanho)
    {
        var texto = valor ?? string.Empty;

        if (texto.Length > tamanho)
        {
            return texto[..tamanho];
        }

        return texto.PadRight(tamanho);
    }
}