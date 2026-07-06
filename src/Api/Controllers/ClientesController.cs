using Cooperativa.Api.Servicos;
using Cooperativa.Contratos;
using Microsoft.AspNetCore.Mvc;

namespace Cooperativa.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly IFluxoCobolServico fluxoCobol;

    public ClientesController(IFluxoCobolServico fluxoCobol)
    {
        this.fluxoCobol = fluxoCobol;
    }

    [HttpGet("{codigo}")]
    public async Task<ActionResult<RespostaCliente>> Consultar(
        string codigo,
        CancellationToken cancellationToken
    )
    {
        var resultado = await fluxoCobol.ProcessarAsync(
            new ClienteEntradaCobol
            {
                Operacao = OperacaoCliente.Consultar,
                Codigo = codigo
            },
            cancellationToken
        );

        return Responder(resultado);
    }

    [HttpPost]
    public async Task<ActionResult<RespostaCliente>> Cadastrar(
        CadastroCliente cadastro,
        CancellationToken cancellationToken
    )
    {
        var resultado = await fluxoCobol.ProcessarAsync(
            new ClienteEntradaCobol
            {
                Operacao = OperacaoCliente.Cadastrar,
                Nome = cadastro.Nome,
                Email = cadastro.Email,
                Telefone = cadastro.Telefone
            },
            cancellationToken
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
        var resultado = await fluxoCobol.ProcessarAsync(
            new ClienteEntradaCobol
            {
                Operacao = OperacaoCliente.Atualizar,
                Codigo = codigo,
                Email = contato.Email,
                Telefone = contato.Telefone
            },
            cancellationToken
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