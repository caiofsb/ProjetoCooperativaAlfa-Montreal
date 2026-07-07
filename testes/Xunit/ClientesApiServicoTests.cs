using System.Net;
using System.Net.Http.Json;
using Cooperativa.Contratos;
using Cooperativa.Web.Servicos;
using Xunit;

namespace Cooperativa.Testes;

public sealed class ClientesApiServicoTests
{
    [Fact]
    public async Task Consultar_async_deve_chamar_endpoint_da_api()
    {
        var respostaEsperada = new RespostaCliente
        {
            Sucesso = true,
            CodigoRetorno = CodigoResposta.Sucesso,
            Mensagem = "Cliente encontrado.",
            Cliente = new Cliente
            {
                Codigo = "000001",
                Nome = "Ana Silva",
                Email = "ana.silva@cooperativa.com",
                Telefone = "11988887777"
            }
        };

        var handler = new HandlerFake(respostaEsperada);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5017")
        };

        var servico = new ClientesApiServico(httpClient);

        var resultado = await servico.ConsultarAsync("000001");

        Assert.True(resultado.Sucesso);
        Assert.Equal("000001", resultado.Cliente?.Codigo);
        Assert.Equal(HttpMethod.Get, handler.MetodoRecebido);
        Assert.Equal("/api/clientes/000001", handler.CaminhoRecebido);
    }

    [Fact]
    public async Task Cadastrar_async_deve_enviar_dados_para_api()
    {
        var respostaEsperada = new RespostaCliente
        {
            Sucesso = true,
            CodigoRetorno = CodigoResposta.Sucesso,
            Mensagem = "Cliente cadastrado.",
            Cliente = new Cliente
            {
                Codigo = "000004",
                Nome = "Maria Oliveira",
                Email = "maria.oliveira@cooperativa.com",
                Telefone = "11977776666"
            }
        };

        var handler = new HandlerFake(respostaEsperada);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5017")
        };

        var servico = new ClientesApiServico(httpClient);

        var resultado = await servico.CadastrarAsync(
            new CadastroCliente
            {
                Nome = "Maria Oliveira",
                Email = "maria.oliveira@cooperativa.com",
                Telefone = "11977776666"
            }
        );

        Assert.True(resultado.Sucesso);
        Assert.Equal("000004", resultado.Cliente?.Codigo);
        Assert.Equal(HttpMethod.Post, handler.MetodoRecebido);
        Assert.Equal("/api/clientes", handler.CaminhoRecebido);
    }

    [Fact]
    public async Task Atualizar_async_deve_chamar_endpoint_de_contato()
    {
        var respostaEsperada = new RespostaCliente
        {
            Sucesso = true,
            CodigoRetorno = CodigoResposta.Sucesso,
            Mensagem = "Cliente atualizado.",
            Cliente = new Cliente
            {
                Codigo = "000001",
                Nome = "Ana Silva",
                Email = "ana.novo@cooperativa.com",
                Telefone = "11911112222"
            }
        };

        var handler = new HandlerFake(respostaEsperada);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost:5017")
        };

        var servico = new ClientesApiServico(httpClient);

        var resultado = await servico.AtualizarContatoAsync(
            "000001",
            new ContatoCliente
            {
                Email = "ana.novo@cooperativa.com",
                Telefone = "11911112222"
            }
        );

        Assert.True(resultado.Sucesso);
        Assert.Equal("ana.novo@cooperativa.com", resultado.Cliente?.Email);
        Assert.Equal(HttpMethod.Put, handler.MetodoRecebido);
        Assert.Equal("/api/clientes/000001/contato", handler.CaminhoRecebido);
    }

    private sealed class HandlerFake : HttpMessageHandler
    {
        private readonly RespostaCliente resposta;

        public HandlerFake(RespostaCliente resposta)
        {
            this.resposta = resposta;
        }

        public HttpMethod? MetodoRecebido { get; private set; }

        public string? CaminhoRecebido { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            MetodoRecebido = request.Method;
            CaminhoRecebido = request.RequestUri?.AbsolutePath;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(resposta)
            };

            return await Task.FromResult(response);
        }
    }
}