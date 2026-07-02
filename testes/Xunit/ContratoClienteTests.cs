using Cooperativa.Contratos;
using Xunit;

namespace Cooperativa.Testes;

public sealed class ContratoClienteTests
{
    [Fact]
    public void Operacao_de_cadastro_deve_ter_valor_esperado()
    {
        Assert.Equal("CADASTRAR", OperacaoCliente.Cadastrar);
    }

    [Fact]
    public void Email_duplicado_deve_ter_codigo_0409()
    {
        Assert.Equal("0409", CodigoResposta.EmailDuplicado);
    }

    [Fact]
    public void Telefone_do_cadastro_deve_poder_ficar_vazio()
    {
        var cadastro = new CadastroCliente
        {
            Nome = "Ana Silva",
            Email = "ana@cooperativa.com"
        };

        Assert.Null(cadastro.Telefone);
    }
}