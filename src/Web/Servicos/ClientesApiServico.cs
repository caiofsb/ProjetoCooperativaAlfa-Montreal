using System.Net.Http.Json;
using Cooperativa.Contratos;

namespace Cooperativa.Web.Servicos;

public sealed class ClientesApiServico : IClientesApiServico
{
    private readonly HttpClient httpClient;

    public ClientesApiServico(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<RespostaCliente> ConsultarAsync(
        string codigo,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var resposta = await httpClient.GetAsync(
                $"api/clientes/{Uri.EscapeDataString(codigo)}",
                cancellationToken
            );

            return await LerRespostaAsync(resposta, cancellationToken);
        }
        catch
        {
            return ErroComunicacao();
        }
    }

    public async Task<RespostaCliente> CadastrarAsync(
        CadastroCliente cadastro,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var resposta = await httpClient.PostAsJsonAsync(
                "api/clientes",
                cadastro,
                cancellationToken
            );

            return await LerRespostaAsync(resposta, cancellationToken);
        }
        catch
        {
            return ErroComunicacao();
        }
    }

    public async Task<RespostaCliente> AtualizarContatoAsync(
        string codigo,
        ContatoCliente contato,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var resposta = await httpClient.PutAsJsonAsync(
                $"api/clientes/{Uri.EscapeDataString(codigo)}/contato",
                contato,
                cancellationToken
            );

            return await LerRespostaAsync(resposta, cancellationToken);
        }
        catch
        {
            return ErroComunicacao();
        }
    }

    private static async Task<RespostaCliente> LerRespostaAsync(
        HttpResponseMessage resposta,
        CancellationToken cancellationToken
    )
    {
        var corpo = await resposta.Content.ReadFromJsonAsync<RespostaCliente>(
            cancellationToken: cancellationToken
        );

        if (corpo is not null)
        {
            return corpo;
        }

        return new RespostaCliente
        {
            Sucesso = false,
            CodigoRetorno = CodigoResposta.ErroSistema,
            Mensagem = "Resposta vazia da API."
        };
    }

    private static RespostaCliente ErroComunicacao()
    {
        return new RespostaCliente
        {
            Sucesso = false,
            CodigoRetorno = CodigoResposta.ErroSistema,
            Mensagem = "Nao foi possivel comunicar com a API."
        };
    }
}