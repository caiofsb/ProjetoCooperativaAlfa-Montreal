using System.Text;

namespace Cooperativa.Helper;

public static class ArquivoHelper
{
    public static ComandoHelper LerEntrada(string caminho)
    {
        var conteudo = File.ReadAllText(caminho, Encoding.ASCII);

        var linha = conteudo
            .Replace("\r", string.Empty)
            .Split('\n')
            .FirstOrDefault(l => !string.IsNullOrWhiteSpace(l));

        if (string.IsNullOrWhiteSpace(linha))
        {
            return new ComandoHelper();
        }

        return LeitorComando.LerLinhaFixa(linha);
    }

    public static void GravarSaida(string caminho, ResultadoHelper resultado)
    {
        var pasta = Path.GetDirectoryName(caminho);

        if (!string.IsNullOrWhiteSpace(pasta))
        {
            Directory.CreateDirectory(pasta);
        }

        File.WriteAllText(
            caminho,
            resultado.FormatarLinha() + Environment.NewLine,
            Encoding.ASCII
        );
    }
}