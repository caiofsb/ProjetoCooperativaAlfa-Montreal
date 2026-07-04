using Cooperativa.Api.Servicos;
using Cooperativa.Contratos;
using Cooperativa.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Cooperativa.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly IFluxoCobolServico fluxoCobol;
    private readonly IClienteServico clienteServico;

    public ClientesController(
        IFluxoCobolServico fluxoCobol,
        IClienteServico clienteServico
    )
    {
        this.fluxoCobol = fluxoCobol;
        this.clienteServico = clienteServico;
    }

    [HttpGet("{codigo}")]
    public async Task<ActionResult<RespostaCliente>> Consultar(
        string codigo,
        CancellationToken cancellationToken
    )
    {
        var validacao = await fluxoCobol.ProcessarAsync(
            new ClienteEntradaCobol
            {
                Operacao = OperacaoCliente.Consultar,
                Codigo = codigo
            },
            cancellationToken
        );

        if (!validacao.Sucesso)
        {
            return Responder(validacao);
        }

        var resultado = clienteServico.Consultar(codigo);

        return Responder(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<RespostaCliente>> Cadastrar(
        CadastroCliente cadastro,
        CancellationToken cancellationToken
    )
    {
        var validacao = await fluxoCobol.ProcessarAsync(
            new ClienteEntradaCobol
            {
                Operacao = OperacaoCliente.Cadastrar,
                Nome = cadastro.Nome,
                Email = cadastro.Email,
                Telefone = cadastro.Telefone
            },
            cancellationToken
        );

        if (!validacao.Sucesso)
        {
            return Responder(validacao);
        }

        var resultado = clienteServico.Cadastrar(
            cadastro.Nome,
            cadastro.Email,
            cadastro.Telefone
        );

        return Responder(resultado);
    }

    [HttpPut("{codigo}/contato")]
    public async Task<ActionResult<RespostaCliente>> AtualizarContato(
        string codigo,
        ContatoCliente contato,
        CancellationToken cancellationToken
    )
    {
        var validacao = await fluxoCobol.ProcessarAsync(
            new ClienteEntradaCobol
            {
                Operacao = OperacaoCliente.Atualizar,
                Codigo = codigo,
                Email = contato.Email,
                Telefone = contato.Telefone
            },
            cancellationToken
        );

        if (!validacao.Sucesso)
        {
            return Responder(validacao);
        }

        var resultado = clienteServico.Atualizar(
            codigo,
            contato.Email,
            contato.Telefone
        );

        return Responder(resultado);
    }

    private ActionResult<RespostaCliente> Responder(ResultadoCobol resultado)
    {
        var resposta = new RespostaCliente
        {
            Sucesso = resultado.Sucesso,
            CodigoRetorno = resultado.CodigoRetorno,
            Mensagem = resultado.Mensagem,
            Cliente = resultado.Cliente
        };

        return GerarResposta(resposta);
    }

    private ActionResult<RespostaCliente> Responder(ResultadoHelper resultado)
    {
        var resposta = new RespostaCliente
        {
            Sucesso = resultado.Sucesso,
            CodigoRetorno = resultado.CodigoRetorno,
            Mensagem = resultado.Mensagem,
            Cliente = resultado.Cliente
        };

        return GerarResposta(resposta);
    }

    private ActionResult<RespostaCliente> GerarResposta(RespostaCliente resposta)
    {
        if (resposta.CodigoRetorno == CodigoResposta.Sucesso)
        {
            return Ok(resposta);
        }

        if (resposta.CodigoRetorno == CodigoResposta.NaoEncontrado)
        {
            return NotFound(resposta);
        }

        if (resposta.CodigoRetorno == CodigoResposta.EmailDuplicado)
        {
            return Conflict(resposta);
        }

        if (resposta.CodigoRetorno == CodigoResposta.DadosInvalidos)
        {
            return BadRequest(resposta);
        }

        return StatusCode(500, resposta);
    }
}