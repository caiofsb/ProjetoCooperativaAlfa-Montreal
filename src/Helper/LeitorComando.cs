namespace Cooperativa.Helper;

public static class LeitorComando
{
    public static ComandoHelper Ler(string[] args)
    {
        if (args.Length == 0)
        {
            return new ComandoHelper
            {
                Operacao = string.Empty
            };
        }

        var operacao = args[0].Trim().ToUpperInvariant();

        if (operacao == "CONSULTAR")
        {
            return new ComandoHelper
            {
                Operacao = operacao,
                Codigo = args.Length > 1 ? args[1].Trim() : string.Empty
            };
        }

        if (operacao == "CADASTRAR")
        {
            return new ComandoHelper
            {
                Operacao = operacao,
                Nome = args.Length > 1 ? args[1].Trim() : string.Empty,
                Email = args.Length > 2 ? args[2].Trim() : string.Empty,
                Telefone = args.Length > 3 ? TratarTelefone(args[3]) : null
            };
        }

        if (operacao == "ATUALIZAR")
        {
            return new ComandoHelper
            {
                Operacao = operacao,
                Codigo = args.Length > 1 ? args[1].Trim() : string.Empty,
                Email = args.Length > 2 ? args[2].Trim() : string.Empty,
                Telefone = args.Length > 3 ? TratarTelefone(args[3]) : null
            };
        }

        return new ComandoHelper
        {
            Operacao = operacao
        };
    }

    public static ComandoHelper LerLinhaFixa(string linha)
    {
        var texto = linha.PadRight(117);

        return new ComandoHelper
        {
            Operacao = Extrair(texto, 0, 10).ToUpperInvariant(),
            Codigo = Extrair(texto, 10, 6),
            Nome = Extrair(texto, 16, 30),
            Email = Extrair(texto, 46, 60),
            Telefone = TratarTelefone(Extrair(texto, 106, 11))
        };
    }

    private static string Extrair(string texto, int inicio, int tamanho)
    {
        return texto.Substring(inicio, tamanho).Trim();
    }

    private static string? TratarTelefone(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
        {
            return null;
        }

        return telefone.Trim();
    }
}