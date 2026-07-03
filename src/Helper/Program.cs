using Cooperativa.Helper;

var comando = LeitorComando.Ler(args);
var servico = CriarServico();

var resposta = comando.Operacao switch
{
    "CONSULTAR" => servico.Consultar(comando.Codigo),
    "CADASTRAR" => servico.Cadastrar(comando.Nome, comando.Email, comando.Telefone),
    "ATUALIZAR" => servico.Atualizar(comando.Codigo, comando.Email, comando.Telefone),
    _ => ResultadoHelper.DadosInvalidos("Operacao invalida.")
};

Console.WriteLine(resposta.FormatarLinha());

return resposta.Sucesso ? 0 : 1;

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