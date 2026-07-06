using Cooperativa.Helper;

var servico = CriarServico();

if (args.Length > 0 &&
    string.Equals(args[0], "ARQUIVO", StringComparison.OrdinalIgnoreCase))
{
    var entradaHelper = Path.Combine("runtime", "helper-entrada.txt");
    var saidaHelper = Path.Combine("runtime", "helper-saida.txt");

    var comandoArquivo = ArquivoHelper.LerEntrada(entradaHelper);
    var resultadoArquivo = Processar(servico, comandoArquivo);

    ArquivoHelper.GravarSaida(saidaHelper, resultadoArquivo);

    return 0;
}

var comando = LeitorComando.Ler(args);
var resposta = Processar(servico, comando);

Console.WriteLine(resposta.FormatarLinha());

return resposta.Sucesso ? 0 : 1;

static ResultadoHelper Processar(
    IClienteServico servico,
    ComandoHelper comando
)
{
    return comando.Operacao switch
    {
        "CONSULTAR" => servico.Consultar(comando.Codigo),
        "CADASTRAR" => servico.Cadastrar(
            comando.Nome,
            comando.Email,
            comando.Telefone
        ),
        "ATUALIZAR" => servico.Atualizar(
            comando.Codigo,
            comando.Email,
            comando.Telefone
        ),
        _ => ResultadoHelper.DadosInvalidos("Operacao invalida.")
    };
}

static IClienteServico CriarServico()
{
    var usarDb2 = Environment.GetEnvironmentVariable("HELPER_USAR_DB2");
    var connectionString = Environment.GetEnvironmentVariable("DB2_CONNECTION_STRING");

    if (string.Equals(usarDb2, "true", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(connectionString))
    {
        return new ClienteDb2Servico(connectionString);
    }

    return new ClienteMemoriaServico();
}