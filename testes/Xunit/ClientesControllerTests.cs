using Cooperativa.Api.Controllers;
using Cooperativa.Api.Servicos;
using Cooperativa.Contratos;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Cooperativa.Testes;

public sealed class ClientesControllerTests
{
    [Fact]
    public async Task Consultar_cliente_existente_deve_retornar_ok()
    {
        var fluxo = new FluxoCobolFake(
            new ResultadoCobol
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
            }
        );

        var controller = new ClientesController(fluxo);

        var resposta = await controller.Consultar(
            "000001",
            CancellationToken.None
        );

        var ok = Assert.IsType<OkObjectResult>(resposta.Result);
        var corpo = Assert.IsType<RespostaCliente>(ok.Value);

        Assert.True(corpo.Sucesso);
        Assert.Equal("000001", corpo.Cliente?.Codigo);
        Assert.Equal("Ana Silva", corpo.Cliente?.Nome);
    }

    [Fact]
    public async Task Consultar_com_codigo_invalido_deve_retornar_bad_request()
    {
        var fluxo = new FluxoCobolFake(
            new ResultadoCobol
            {
                Sucesso = false,
                CodigoRetorno = CodigoResposta.DadosInvalidos,
                Mensagem = "Codigo nao numerico."
            }
        );

        var controller = new ClientesController(fluxo);

        var resposta = await controller.Consultar(
            "ABC123",
            CancellationToken.None
        );

        var badRequest = Assert.IsType<BadRequestObjectResult>(
            resposta.Result
        );

        var corpo = Assert.IsType<RespostaCliente>(badRequest.Value);

        Assert.False(corpo.Sucesso);
        Assert.Equal(CodigoResposta.DadosInvalidos, corpo.CodigoRetorno);
    }

    [Fact]
    public async Task Cadastrar_email_duplicado_deve_retornar_conflict()
    {
        var fluxo = new FluxoCobolFake(
            new ResultadoCobol
            {
                Sucesso = false,
                CodigoRetorno = CodigoResposta.EmailDuplicado,
                Mensagem = "Email ja cadastrado."
            }
        );

        var controller = new ClientesController(fluxo);

        var resposta = await controller.Cadastrar(
            new CadastroCliente
            {
                Nome = "Ana Silva",
                Email = "ana.silva@cooperativa.com",
                Telefone = null
            },
            CancellationToken.None
        );

        var conflict = Assert.IsType<ConflictObjectResult>(resposta.Result);
        var corpo = Assert.IsType<RespostaCliente>(conflict.Value);

        Assert.False(corpo.Sucesso);
        Assert.Equal(CodigoResposta.EmailDuplicado, corpo.CodigoRetorno);
    }

    [Fact]
    public async Task Atualizar_cliente_inexistente_deve_retornar_not_found()
    {
        var fluxo = new FluxoCobolFake(
            new ResultadoCobol
            {
                Sucesso = false,
                CodigoRetorno = CodigoResposta.NaoEncontrado,
                Mensagem = "Cliente nao encontrado."
            }
        );

        var controller = new ClientesController(fluxo);

        var resposta = await controller.AtualizarContato(
            "999999",
            new ContatoCliente
            {
                Email = "teste@cooperativa.com",
                Telefone = "11999999999"
            },
            CancellationToken.None
        );

        var notFound = Assert.IsType<NotFoundObjectResult>(resposta.Result);
        var corpo = Assert.IsType<RespostaCliente>(notFound.Value);

        Assert.False(corpo.Sucesso);
        Assert.Equal(CodigoResposta.NaoEncontrado, corpo.CodigoRetorno);
    }

    private sealed class FluxoCobolFake : IFluxoCobolServico
    {
        private readonly ResultadoCobol resultado;

        public FluxoCobolFake(ResultadoCobol resultado)
        {
            this.resultado = resultado;
        }

        public Task<ResultadoCobol> ProcessarAsync(
            ClienteEntradaCobol entrada,
            CancellationToken cancellationToken = default
        )
        {
            return Task.FromResult(resultado);
        }
    }
}