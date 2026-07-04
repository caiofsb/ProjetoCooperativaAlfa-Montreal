using Cooperativa.Contratos;

namespace Cooperativa.Api.Servicos;

public sealed class ResultadoCobol
{
    public bool Sucesso { get; set; }

    public string CodigoRetorno { get; set; } = string.Empty;

    public string Mensagem { get; set; } = string.Empty;

    public Cliente? Cliente { get; set; }
}