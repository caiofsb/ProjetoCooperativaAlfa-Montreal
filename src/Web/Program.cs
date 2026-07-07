using Cooperativa.Web.Servicos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IClientesApiServico, ClientesApiServico>(
    (serviceProvider, client) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var apiUrl = configuration["Api:BaseUrl"] ??
                     "http://localhost:5017";

        client.BaseAddress = new Uri(apiUrl);
    }
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();