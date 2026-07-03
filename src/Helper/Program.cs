using Cooperativa.Helper;

var comando = LeitorComando.Ler(args);
var servico = new ClienteMemoriaServico();

var resposta = comando.Operacao switch
{
    "CONSULTAR" => servico.Consultar(comando.Codigo),
    "CADASTRAR" => servico.Cadastrar(comando.Nome, comando.Email, comando.Telefone),
    "ATUALIZAR" => servico.Atualizar(comando.Codigo, comando.Email, comando.Telefone),
    _ => ResultadoHelper.DadosInvalidos("Operacao invalida.")
};

Console.WriteLine(resposta.FormatarLinha());

return resposta.Sucesso ? 0 : 1;