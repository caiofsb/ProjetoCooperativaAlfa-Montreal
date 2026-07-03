namespace Cooperativa.Helper;

public interface IClienteServico
{
    ResultadoHelper Consultar(string codigo);

    ResultadoHelper Cadastrar(string nome, string email, string? telefone);

    ResultadoHelper Atualizar(string codigo, string email, string? telefone);
}