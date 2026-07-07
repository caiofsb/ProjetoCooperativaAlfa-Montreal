using Cooperativa.Contratos;
using Cooperativa.Helper;
using Xunit;

namespace Cooperativa.Testes;

public sealed class HelperFluxoTests
{
    [Fact]
    public void Consultar_cliente_existente_deve_retornar_sucesso()
    {
        var servico = new ClienteMemoriaServico();

        var resultado = servico.Consultar("000001");

        Assert.True(resultado.Sucesso);
        Assert.Equal(CodigoResposta.Sucesso, resultado.CodigoRetorno);
        Assert.NotNull(resultado.Cliente);
        Assert.Equal("000001", resultado.Cliente.Codigo);
        Assert.Equal("Ana Silva", resultado.Cliente.Nome);
    }

    [Fact]
    public void Consultar_cliente_inexistente_deve_retornar_nao_encontrado()
    {
        var servico = new ClienteMemoriaServico();

        var resultado = servico.Consultar("999999");

        Assert.False(resultado.Sucesso);
        Assert.Equal(CodigoResposta.NaoEncontrado, resultado.CodigoRetorno);
        Assert.Null(resultado.Cliente);
    }

    [Fact]
    public void Cadastrar_cliente_deve_gerar_codigo_e_permitir_consulta()
    {
        var servico = new ClienteMemoriaServico();

        var cadastro = servico.Cadastrar(
            "Maria Oliveira",
            "maria.oliveira@cooperativa.com",
            "11977776666"
        );

        Assert.True(cadastro.Sucesso);
        Assert.NotNull(cadastro.Cliente);
        Assert.Equal("000004", cadastro.Cliente.Codigo);

        var consulta = servico.Consultar(cadastro.Cliente.Codigo);

        Assert.True(consulta.Sucesso);
        Assert.Equal("Maria Oliveira", consulta.Cliente?.Nome);
        Assert.Equal("maria.oliveira@cooperativa.com", consulta.Cliente?.Email);
    }

    [Fact]
    public void Cadastrar_email_duplicado_deve_retornar_conflito()
    {
        var servico = new ClienteMemoriaServico();

        var resultado = servico.Cadastrar(
            "Outra Ana",
            "ana.silva@cooperativa.com",
            "11900000000"
        );

        Assert.False(resultado.Sucesso);
        Assert.Equal(CodigoResposta.EmailDuplicado, resultado.CodigoRetorno);
    }

    [Fact]
    public void Atualizar_contato_deve_alterar_email_e_telefone()
    {
        var servico = new ClienteMemoriaServico();

        var atualizacao = servico.Atualizar(
            "000001",
            "ana.novo@cooperativa.com",
            "11911112222"
        );

        Assert.True(atualizacao.Sucesso);
        Assert.Equal("ana.novo@cooperativa.com", atualizacao.Cliente?.Email);
        Assert.Equal("11911112222", atualizacao.Cliente?.Telefone);

        var consulta = servico.Consultar("000001");

        Assert.Equal("ana.novo@cooperativa.com", consulta.Cliente?.Email);
        Assert.Equal("11911112222", consulta.Cliente?.Telefone);
    }
}