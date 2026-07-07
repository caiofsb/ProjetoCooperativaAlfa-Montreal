using Cooperativa.Api.Servicos;
using Cooperativa.Contratos;
using Xunit;

namespace Cooperativa.Testes;

public sealed class FluxoCobolServicoTests
{
    [Fact]
    public void Montar_linha_entrada_deve_gerar_registro_fixo()
    {
        var entrada = new ClienteEntradaCobol
        {
            Operacao = OperacaoCliente.Cadastrar,
            Codigo = "",
            Nome = "Maria Oliveira",
            Email = "maria.oliveira@cooperativa.com",
            Telefone = "11977776666"
        };

        var linha = FluxoCobolServico.MontarLinhaEntrada(entrada);

        Assert.Equal(117, linha.Length);
        Assert.StartsWith("CADASTRAR ", linha);
        Assert.Contains("Maria Oliveira", linha);
        Assert.Contains("maria.oliveira@cooperativa.com", linha);
        Assert.EndsWith("11977776666", linha);
    }

    [Fact]
    public void Interpretar_saida_valida_deve_retornar_sucesso()
    {
        var saida =
            "0000|Cliente cadastrado.                                      |CADASTRAR |000004|Maria Oliveira                |maria.oliveira@cooperativa.com                              |11977776666";

        var resultado = FluxoCobolServico.InterpretarSaida(saida);

        Assert.True(resultado.Sucesso);
        Assert.Equal(CodigoResposta.Sucesso, resultado.CodigoRetorno);
        Assert.Equal("Cliente cadastrado.", resultado.Mensagem);
        Assert.NotNull(resultado.Cliente);
        Assert.Equal("000004", resultado.Cliente?.Codigo);
        Assert.Equal("Maria Oliveira", resultado.Cliente?.Nome);
        Assert.Equal("maria.oliveira@cooperativa.com", resultado.Cliente?.Email);
        Assert.Equal("11977776666", resultado.Cliente?.Telefone);
    }

    [Fact]
    public void Interpretar_saida_de_cliente_nao_encontrado_deve_retornar_erro()
    {
        var saida =
            "0404|Cliente nao encontrado.                                  |CONSULTAR |      |                              |                                                            |           ";

        var resultado = FluxoCobolServico.InterpretarSaida(saida);

        Assert.False(resultado.Sucesso);
        Assert.Equal(CodigoResposta.NaoEncontrado, resultado.CodigoRetorno);
        Assert.Equal("Cliente nao encontrado.", resultado.Mensagem);
        Assert.Null(resultado.Cliente);
    }

    [Fact]
    public void Interpretar_saida_invalida_deve_retornar_erro_sistema()
    {
        var saida = "SAIDA INVALIDA";

        var resultado = FluxoCobolServico.InterpretarSaida(saida);

        Assert.False(resultado.Sucesso);
        Assert.Equal(CodigoResposta.ErroSistema, resultado.CodigoRetorno);
        Assert.Equal("Saida do COBOL em formato invalido.", resultado.Mensagem);
        Assert.Null(resultado.Cliente);
    }
}