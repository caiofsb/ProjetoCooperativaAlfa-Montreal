using Cooperativa.Contratos;
using Cooperativa.Helper;
using Xunit;

namespace Cooperativa.Testes;

public sealed class ResultadoHelperTests
{
    [Fact]
    public void Formatar_linha_deve_gerar_registro_fixo_para_o_cobol()
    {
        var resultado = ResultadoHelper.Ok(
            new Cliente
            {
                Codigo = "000001",
                Nome = "Ana Silva",
                Email = "ana.silva@cooperativa.com",
                Telefone = "11988887777"
            },
            "Cliente encontrado."
        );

        var linha = resultado.FormatarLinha();

        Assert.Equal(176, linha.Length);
        Assert.StartsWith("0000|", linha);
        Assert.Contains("Ana Silva", linha);
        Assert.Contains("ana.silva@cooperativa.com", linha);
    }

    [Fact]
    public void Formatar_linha_de_erro_deve_manter_tamanho_fixo()
    {
        var resultado = ResultadoHelper.NaoEncontrado();

        var linha = resultado.FormatarLinha();

        Assert.Equal(176, linha.Length);
        Assert.StartsWith("0404|", linha);
        Assert.Contains("Cliente nao encontrado.", linha);
    }
}