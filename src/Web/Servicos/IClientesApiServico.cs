using Cooperativa.Contratos;

namespace Cooperativa.Web.Servicos;

public interface IClientesApiServico
{
    Task<RespostaCliente> ConsultarAsync(
        string codigo,
        CancellationToken cancellationToken = default
    );

    Task<RespostaCliente> CadastrarAsync(
        CadastroCliente cadastro,
        CancellationToken cancellationToken = default
    );

    Task<RespostaCliente> AtualizarContatoAsync(
        string codigo,
        ContatoCliente contato,
        CancellationToken cancellationToken = default
    );
}