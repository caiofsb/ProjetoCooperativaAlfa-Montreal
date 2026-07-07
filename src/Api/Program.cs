using Cooperativa.Api.Servicos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IFluxoCobolServico, FluxoCobolServico>();

builder.Services.AddHostedService<MonitorCobolPromptServico>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program
{
}