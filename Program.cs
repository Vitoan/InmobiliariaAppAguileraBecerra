using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using InmobiliariaAppAguileraBecerra.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios
builder.Services.AddControllersWithViews();

// Registrar DbContext con la conexión a tu base de datos
builder.Services.AddDbContext<InmobiliariaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar repositorios
builder.Services.AddScoped<RepositorioPropietario>();
builder.Services.AddScoped<RepositorioInquilino>();
builder.Services.AddScoped<RepositorioInmueble>();
builder.Services.AddScoped<RepositorioContrato>();
builder.Services.AddScoped<RepositorioImagen>();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // Comentado para evitar problemas, como en tu código
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
