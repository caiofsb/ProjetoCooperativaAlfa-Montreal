using System.Diagnostics;
using System.Text;
using Cooperativa.Contratos;

namespace Cooperativa.Api.Servicos;

public sealed class FluxoCobolServico : IFluxoCobolServico
{
    public async Task<ResultadoCobol> ProcessarAsync(
        ClienteEntradaCobol entrada,
        CancellationToken cancellationToken = default
    )
    {
        var etapas = new List<string>();
        string? raizParaLog = null;

        try
        {
            var raizProjeto = EncontrarRaizProjeto();
            raizParaLog = raizProjeto;

            var runtime = Path.Combine(raizProjeto, "runtime");

            Directory.CreateDirectory(runtime);

            var entradaPath = Path.Combine(runtime, "entrada.txt");
            var saidaPath = Path.Combine(runtime, "saida.txt");
            var helperEntradaPath = Path.Combine(runtime, "helper-entrada.txt");
            var helperSaidaPath = Path.Combine(runtime, "helper-saida.txt");
            var executavelCobol = Path.Combine(runtime, "PROJFINAL.exe");

            RemoverArquivo(saidaPath);
            RemoverArquivo(helperEntradaPath);
            RemoverArquivo(helperSaidaPath);

            var operacao = entrada.Operacao?.Trim().ToUpperInvariant() ?? "";
            var codigo = entrada.Codigo?.Trim() ?? "";

            await CobolMonitor.EscreverAsync(
                raizProjeto,
                $"""
                
                ============================================================
                NOVA OPERACAO
                ============================================================
                Operacao: {operacao}
                Codigo: {codigo}
                Fluxo: Razor Pages -> API -> COBOL -> Helper -> DB2
                """,
                cancellationToken
            );

            etapas.Add("API recebeu a requisicao da tela Razor Pages.");
            etapas.Add("API montou o registro fixo de entrada para o COBOL.");

            var linhaEntrada = MontarLinhaEntrada(entrada);

            await File.WriteAllTextAsync(
                entradaPath,
                linhaEntrada + Environment.NewLine,
                Encoding.ASCII,
                cancellationToken
            );

            etapas.Add("API gravou runtime/entrada.txt.");

            await CobolMonitor.EscreverArquivoAsync(
                raizProjeto,
                "[1] Entrada enviada para o COBOL",
                entradaPath,
                cancellationToken
            );

            if (!File.Exists(executavelCobol))
            {
                await CobolMonitor.EscreverAsync(
                    raizProjeto,
                    "[ERRO] Executavel COBOL nao encontrado.",
                    cancellationToken
                );

                return new ResultadoCobol
                {
                    Sucesso = false,
                    CodigoRetorno = CodigoResposta.ErroSistema,
                    Mensagem = "Executavel COBOL nao encontrado.",
                    EtapasProcessamento = etapas
                };
            }

            await CobolMonitor.EscreverAsync(
                raizProjeto,
                "[2] Executando programa COBOL...",
                cancellationToken
            );

            etapas.Add("API executou runtime/PROJFINAL.exe.");

            var resultadoProcesso = await ExecutarCobolAsync(
                executavelCobol,
                raizProjeto,
                cancellationToken
            );

            etapas.Add(
                $"COBOL finalizou com codigo de saida {resultadoProcesso.CodigoSaida}."
            );

            await CobolMonitor.EscreverAsync(
                raizProjeto,
                $"[2] COBOL executado. Codigo de saida: {resultadoProcesso.CodigoSaida}",
                cancellationToken
            );

            if (!string.IsNullOrWhiteSpace(resultadoProcesso.SaidaPadrao))
            {
                await CobolMonitor.EscreverAsync(
                    raizProjeto,
                    "[COBOL STDOUT]",
                    cancellationToken
                );

                await CobolMonitor.EscreverAsync(
                    raizProjeto,
                    resultadoProcesso.SaidaPadrao,
                    cancellationToken
                );
            }

            if (!string.IsNullOrWhiteSpace(resultadoProcesso.ErroPadrao))
            {
                await CobolMonitor.EscreverAsync(
                    raizProjeto,
                    "[COBOL STDERR]",
                    cancellationToken
                );

                await CobolMonitor.EscreverAsync(
                    raizProjeto,
                    resultadoProcesso.ErroPadrao,
                    cancellationToken
                );
            }

            if (File.Exists(helperEntradaPath))
            {
                etapas.Add("COBOL gravou runtime/helper-entrada.txt.");
            }

            if (File.Exists(helperSaidaPath))
            {
                etapas.Add("Helper acessou o DB2 e gravou runtime/helper-saida.txt.");
            }

            await CobolMonitor.EscreverArquivoAsync(
                raizProjeto,
                "[3] Entrada enviada pelo COBOL para o Helper",
                helperEntradaPath,
                cancellationToken
            );

            await CobolMonitor.EscreverArquivoAsync(
                raizProjeto,
                "[4] Retorno do Helper/DB2",
                helperSaidaPath,
                cancellationToken
            );

            if (!File.Exists(saidaPath))
            {
                await CobolMonitor.EscreverAsync(
                    raizProjeto,
                    "[ERRO] Arquivo runtime/saida.txt nao foi gerado.",
                    cancellationToken
                );

                return new ResultadoCobol
                {
                    Sucesso = false,
                    CodigoRetorno = CodigoResposta.ErroSistema,
                    Mensagem = "Arquivo de saida do COBOL nao foi gerado.",
                    EtapasProcessamento = etapas
                };
            }

            etapas.Add("COBOL gravou runtime/saida.txt com o retorno final.");

            await CobolMonitor.EscreverArquivoAsync(
                raizProjeto,
                "[5] Saida final do COBOL para a API",
                saidaPath,
                cancellationToken
            );

            var saidaCobol = await File.ReadAllTextAsync(
                saidaPath,
                Encoding.ASCII,
                cancellationToken
            );

            etapas.Add("API interpretou o retorno final do COBOL.");

            var resultado = InterpretarSaida(saidaCobol);
            resultado.EtapasProcessamento = etapas;

            await CobolMonitor.EscreverAsync(
                raizProjeto,
                $"""
                Resultado interpretado pela API:
                Codigo: {resultado.CodigoRetorno}
                Mensagem: {resultado.Mensagem}
                Sucesso: {resultado.Sucesso}

                FIM DA OPERACAO
                ============================================================
                """,
                cancellationToken
            );

            return resultado;
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(raizParaLog))
            {
                await CobolMonitor.EscreverAsync(
                    raizParaLog,
                    """
                    [ERRO] Falha ao executar o fluxo legado.

                    FIM DA OPERACAO COM ERRO
                    ============================================================
                    """,
                    CancellationToken.None
                );
            }

            return new ResultadoCobol
            {
                Sucesso = false,
                CodigoRetorno = CodigoResposta.ErroSistema,
                Mensagem = "Falha ao executar fluxo legado.",
                EtapasProcessamento = etapas
            };
        }
    }

    private static async Task<ResultadoProcesso> ExecutarCobolAsync(
        string executavelCobol,
        string raizProjeto,
        CancellationToken cancellationToken
    )
    {
        var processo = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executavelCobol,
                WorkingDirectory = raizProjeto,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        processo.Start();

        var saidaPadraoTask = processo.StandardOutput.ReadToEndAsync();
        var erroPadraoTask = processo.StandardError.ReadToEndAsync();

        await processo.WaitForExitAsync(cancellationToken);

        var saidaPadrao = await saidaPadraoTask;
        var erroPadrao = await erroPadraoTask;

        return new ResultadoProcesso
        {
            CodigoSaida = processo.ExitCode,
            SaidaPadrao = saidaPadrao,
            ErroPadrao = erroPadrao
        };
    }

    public static string MontarLinhaEntrada(ClienteEntradaCobol entrada)
    {
        return Ajustar(entrada.Operacao?.ToUpperInvariant(), 10) +
               Ajustar(entrada.Codigo, 6) +
               Ajustar(entrada.Nome, 30) +
               Ajustar(entrada.Email, 60) +
               Ajustar(entrada.Telefone, 11);
    }

    public static ResultadoCobol InterpretarSaida(string saida)
{
    var linha = saida.TrimEnd('\r', '\n');
    var partes = linha.Split('|');

    if (partes.Length < 6)
    {
        return new ResultadoCobol
        {
            Sucesso = false,
            CodigoRetorno = CodigoResposta.ErroSistema,
            Mensagem = "Saida do COBOL em formato invalido."
        };
    }

    var codigoRetorno = partes[0].Trim();
    var mensagem = partes[1].Trim();

    string codigoCliente;
    string nome;
    string email;
    string telefone;

    if (partes.Length >= 7)
    {
        // Formato final do COBOL:
        // RETORNO|MENSAGEM|OPERACAO|CODIGO|NOME|EMAIL|TELEFONE
        codigoCliente = partes[3].Trim();
        nome = partes[4].Trim();
        email = partes[5].Trim();
        telefone = partes[6].Trim();
    }
    else
    {
        // Formato do Helper:
        // RETORNO|MENSAGEM|CODIGO|NOME|EMAIL|TELEFONE
        codigoCliente = partes[2].Trim();
        nome = partes[3].Trim();
        email = partes[4].Trim();
        telefone = partes[5].Trim();
    }

    Cliente? cliente = null;

    if (!string.IsNullOrWhiteSpace(codigoCliente))
    {
        cliente = new Cliente
        {
            Codigo = codigoCliente,
            Nome = nome,
            Email = email,
            Telefone = string.IsNullOrWhiteSpace(telefone)
                ? null
                : telefone
        };
    }

    return new ResultadoCobol
    {
        Sucesso = codigoRetorno == CodigoResposta.Sucesso,
        CodigoRetorno = codigoRetorno,
        Mensagem = mensagem,
        Cliente = cliente
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

    private static void RemoverArquivo(string caminho)
    {
        if (File.Exists(caminho))
        {
            File.Delete(caminho);
        }
    }

    private static string EncontrarRaizProjeto()
    {
        var diretorio = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (diretorio is not null)
        {
            var temSolution =
                File.Exists(Path.Combine(diretorio.FullName, "Cooperativa.sln")) ||
                File.Exists(Path.Combine(diretorio.FullName, "Cooperativa.slnx"));

            var temSrc = Directory.Exists(Path.Combine(diretorio.FullName, "src"));

            if (temSolution && temSrc)
            {
                return diretorio.FullName;
            }

            diretorio = diretorio.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    private sealed class ResultadoProcesso
    {
        public int CodigoSaida { get; set; }

        public string SaidaPadrao { get; set; } = string.Empty;

        public string ErroPadrao { get; set; } = string.Empty;
    }
}