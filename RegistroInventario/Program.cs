using System.Globalization;
using GoldCrew.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cultura con fecha corta estricta dd/MM/yyyy
var cultura = new CultureInfo("es-DO");
cultura.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Factory: recomendado en Blazor Server para evitar errores de concurrencia del DbContext
builder.Services.AddDbContextFactory<GoldCrewDbContext>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("GoldCrewDb")));

// Servicios de negocio (se irán registrando por módulo)
// builder.Services.AddScoped<ICategoriaService, CategoriaService>();
// builder.Services.AddScoped<IProductoService, ProductoService>();
// ...

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();