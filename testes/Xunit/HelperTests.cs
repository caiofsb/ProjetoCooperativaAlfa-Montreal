using Cooperativa.Contratos;
using Cooperativa.Helper;
using Xunit;

namespace Cooperativa.Testes;

public sealed class HelperTests
{
    [Fact]
    public void Consultar_cliente_existente_deve_retornar_sucesso()
    {
        var servico = new ClienteMemoriaServico();

        var resposta = servico.Consultar("000001");

        Assert.True(resposta.Sucesso);
        Assert.Equal(CodigoResposta.Sucesso, resposta.CodigoRetorno);
        Assert.Equal("Ana Silva", resposta.Cliente?.Nome);
    }

    [Fact]
    public void Consultar_cliente_inexistente_deve_retornar_0404()
    {
        var servico = new ClienteMemoriaServico();

        var resposta = servico.Consultar("999999");

        Assert.False(resposta.Sucesso);
        Assert.Equal(CodigoResposta.NaoEncontrado, resposta.CodigoRetorno);
    }

    [Fact]
    public void Cadastrar_cliente_com_email_existente_deve_retornar_0409()
    {
        var servico = new ClienteMemoriaServico();

        var resposta = servico.Cadastrar(
            "Outro Cliente",
            "ana.silva@cooperativa.com",
            null
        );

        Assert.False(resposta.Sucesso);
        Assert.Equal(CodigoResposta.EmailDuplicado, resposta.CodigoRetorno);
    }

    [Fact]
    public void Cadastrar_cliente_sem_telefone_deve_ser_permitido()
    {
        var servico = new ClienteMemoriaServico();

        var resposta = servico.Cadastrar(
            "Fernanda Rocha",
            "fernanda.rocha@cooperativa.com",
            null
        );

        Assert.True(resposta.Sucesso);
        Assert.Equal("Fernanda Rocha", resposta.Cliente?.Nome);
        Assert.Null(resposta.Cliente?.Telefone);
    }
}