using Cooperativa.Api.Controllers;
using Cooperativa.Api.Servicos;
using Cooperativa.Contratos;
using Cooperativa.Helper;
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
                Mensagem = "Validacao realizada."
            }
        );

        var helper = new ClienteServicoFake(
            ResultadoHelper.Ok(
                new Cliente
                {
                    Codigo = "000001",
                    Nome = "Ana Silva",
                    Email = "ana.silva@cooperativa.com",
                    Telefone = "11988887777"
                },
                "Cliente encontrado."
            )
        );

        var controller = new ClientesController(fluxo, helper);

        var resposta = await controller.Consultar("000001", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(resposta.Result);
        var corpo = Assert.IsType<RespostaCliente>(ok.Value);

        Assert.True(corpo.Sucesso);
        Assert.Equal("000001", corpo.Cliente?.Codigo);
    }

    [Fact]
    public async Task Consultar_com_codigo_invalido_deve_retornar_bad_request()
    {
        var fluxo = new FluxoCobolFake(
            new ResultadoCobol
            {
                Sucesso = false,
                CodigoRetorno = CodigoResposta.DadosInvalidos,
                Mensagem = "Codigo deve ser numerico."
            }
        );

        var helper = new ClienteServicoFake(ResultadoHelper.NaoEncontrado());

        var controller = new ClientesController(fluxo, helper);

        var resposta = await controller.Consultar("ABC123", CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(resposta.Result);
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
                Sucesso = true,
                CodigoRetorno = CodigoResposta.Sucesso,
                Mensagem = "Validacao realizada."
            }
        );

        var helper = new ClienteServicoFake(ResultadoHelper.EmailDuplicado());

        var controller = new ClientesController(fluxo, helper);

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

    private sealed class ClienteServicoFake : IClienteServico
    {
        private readonly ResultadoHelper resultado;

        public ClienteServicoFake(ResultadoHelper resultado)
        {
            this.resultado = resultado;
        }

        public ResultadoHelper Consultar(string codigo)
        {
            return resultado;
        }

        public ResultadoHelper Cadastrar(string nome, string email, string? telefone)
        {
            return resultado;
        }

        public ResultadoHelper Atualizar(string codigo, string email, string? telefone)
        {
            return resultado;
        }
    }
}