using Cooperativa.Api.Servicos;
using Cooperativa.Contratos;
using Xunit;

namespace Cooperativa.Testes;

public sealed class FluxoCobolServicoTests
{
    [Fact]
    public void Montar_linha_de_entrada_deve_respeitar_tamanho_fixo()
    {
        var entrada = new ClienteEntradaCobol
        {
            Operacao = "CADASTRAR",
            Codigo = "",
            Nome = "Maria Oliveira",
            Email = "maria.oliveira@cooperativa.com",
            Telefone = ""
        };

        var linha = FluxoCobolServico.MontarLinhaEntrada(entrada);

        Assert.Equal(117, linha.Length);
        Assert.Equal("CADASTRAR ", linha[..10]);
        Assert.Equal("      ", linha.Substring(10, 6));
        Assert.Equal("Maria Oliveira", linha.Substring(16, 14));
    }

    [Fact]
    public void Interpretar_saida_valida_deve_retornar_sucesso()
    {
        var linha =
            "0000|VALIDACAO REALIZADA.|CADASTRAR |      |Maria Oliveira                |maria.oliveira@cooperativa.com                              |           ";

        var resultado = FluxoCobolServico.InterpretarSaida(linha);

        Assert.True(resultado.Sucesso);
        Assert.Equal(CodigoResposta.Sucesso, resultado.CodigoRetorno);
        Assert.Equal("Maria Oliveira", resultado.Cliente?.Nome);
        Assert.Equal("maria.oliveira@cooperativa.com", resultado.Cliente?.Email);
        Assert.Null(resultado.Cliente?.Telefone);
    }

    [Fact]
    public void Interpretar_saida_invalida_deve_retornar_erro_sistema()
    {
        var resultado = FluxoCobolServico.InterpretarSaida("SAIDA INVALIDA");

        Assert.False(resultado.Sucesso);
        Assert.Equal(CodigoResposta.ErroSistema, resultado.CodigoRetorno);
    }
}