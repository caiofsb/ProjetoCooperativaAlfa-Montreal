using Cooperativa.Contratos;

namespace Cooperativa.Helper;

public sealed class ResultadoHelper
{
    public bool Sucesso { get; set; }

    public string CodigoRetorno { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;

    public Cliente? Cliente { get; set; }

    public static ResultadoHelper Ok(Cliente? cliente, string mensagem)
    {
        return new ResultadoHelper
        {
            Sucesso = true,
            CodigoRetorno = CodigoResposta.Sucesso,
            Mensagem = mensagem,
            Cliente = cliente
        };
    }

    public static ResultadoHelper NaoEncontrado()
    {
        return new ResultadoHelper
        {
            Sucesso = false,
            CodigoRetorno = CodigoResposta.NaoEncontrado,
            Mensagem = "Cliente nao encontrado."
        };
    }

    public static ResultadoHelper EmailDuplicado()
    {
        return new ResultadoHelper
        {
            Sucesso = false,
            CodigoRetorno = CodigoResposta.EmailDuplicado,
            Mensagem = "Email ja cadastrado."
        };
    }

    public static ResultadoHelper DadosInvalidos(string mensagem)
    {
        return new ResultadoHelper
        {
            Sucesso = false,
            CodigoRetorno = CodigoResposta.DadosInvalidos,
            Mensagem = mensagem
        };
    }

    public string FormatarLinha()
    {
        var codigo = Cliente?.Codigo ?? string.Empty;
        var nome = Cliente?.Nome ?? string.Empty;
        var email = Cliente?.Email ?? string.Empty;
        var telefone = Cliente?.Telefone ?? string.Empty;

        return $"{CodigoRetorno}|{Mensagem}|{codigo}|{nome}|{email}|{telefone}";
    }
}