using Cooperativa.Contratos;

namespace Cooperativa.Api.Servicos;

public sealed class ResultadoCobol
{
    public bool Sucesso { get; set; }

    public string CodigoRetorno { get; set; } = CodigoResposta.ErroSistema;

    public string Mensagem { get; set; } = string.Empty;

    public Cliente? Cliente { get; set; }

    public List<string> EtapasProcessamento { get; set; } = new();
}