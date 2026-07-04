using Cooperativa.Api.Servicos;
using Cooperativa.Helper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IFluxoCobolServico, FluxoCobolServico>();

builder.Services.AddSingleton<IClienteServico>(_ =>
{
    var usarDb2 = Environment.GetEnvironmentVariable("HELPER_USAR_DB2");
    var connectionString = Environment.GetEnvironmentVariable("DB2_CONNECTION_STRING");

    if (string.Equals(usarDb2, "true", StringComparison.OrdinalIgnoreCase) &&
        !string.IsNullOrWhiteSpace(connectionString))
    {
        return new ClienteDb2Servico(connectionString);
    }

    return new ClienteMemoriaServico();
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program
{
}