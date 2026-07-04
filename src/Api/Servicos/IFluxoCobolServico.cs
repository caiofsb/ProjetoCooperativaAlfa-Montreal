namespace Cooperativa.Api.Servicos;

public interface IFluxoCobolServico
{
    Task<ResultadoCobol> ProcessarAsync(
        ClienteEntradaCobol entrada,
        CancellationToken cancellationToken = default
    );
}