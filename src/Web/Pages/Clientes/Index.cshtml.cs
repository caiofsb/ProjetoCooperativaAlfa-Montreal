using Cooperativa.Contratos;
using Cooperativa.Web.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cooperativa.Web.Pages.Clientes;

public sealed class IndexModel : PageModel
{
    private readonly IClientesApiServico clientesApi;

    public IndexModel(IClientesApiServico clientesApi)
    {
        this.clientesApi = clientesApi;
    }

    [BindProperty]
    public string CodigoConsulta { get; set; } = string.Empty;

    [BindProperty]
    public CadastroCliente Cadastro { get; set; } = new();

    [BindProperty]
    public string CodigoAtualizacao { get; set; } = string.Empty;

    [BindProperty]
    public ContatoCliente Contato { get; set; } = new();

    public RespostaCliente? Resultado { get; private set; }

    public string TituloResultado { get; private set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task OnPostConsultarAsync(CancellationToken cancellationToken)
    {
        TituloResultado = "Resultado da consulta";

        if (string.IsNullOrWhiteSpace(CodigoConsulta))
        {
            Resultado = CriarErro("Informe o codigo do cliente.");
            return;
        }

        Resultado = await clientesApi.ConsultarAsync(
            CodigoConsulta.Trim(),
            cancellationToken
        );
    }

    public async Task OnPostCadastrarAsync(CancellationToken cancellationToken)
    {
        TituloResultado = "Resultado do cadastro";

        Resultado = await clientesApi.CadastrarAsync(
            Cadastro,
            cancellationToken
        );
    }

    public async Task OnPostAtualizarAsync(CancellationToken cancellationToken)
    {
        TituloResultado = "Resultado da atualizacao";

        if (string.IsNullOrWhiteSpace(CodigoAtualizacao))
        {
            Resultado = CriarErro("Informe o codigo do cliente.");
            return;
        }

        Resultado = await clientesApi.AtualizarContatoAsync(
            CodigoAtualizacao.Trim(),
            Contato,
            cancellationToken
        );
    }

    private static RespostaCliente CriarErro(string mensagem)
    {
        return new RespostaCliente
        {
            Sucesso = false,
            CodigoRetorno = CodigoResposta.DadosInvalidos,
            Mensagem = mensagem
        };
    }
}