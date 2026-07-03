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
                Telefone = args.Length > 3 ? args[3].Trim() : null
            };
        }

        if (operacao == "ATUALIZAR")
        {
            return new ComandoHelper
            {
                Operacao = operacao,
                Codigo = args.Length > 1 ? args[1].Trim() : string.Empty,
                Email = args.Length > 2 ? args[2].Trim() : string.Empty,
                Telefone = args.Length > 3 ? args[3].Trim() : null
            };
        }

        return new ComandoHelper
        {
            Operacao = operacao
        };
    }
}