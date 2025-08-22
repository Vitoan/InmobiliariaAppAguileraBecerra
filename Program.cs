using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using InmobiliariaApp.Models;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    // Este método se usa para configurar los servicios de la aplicación
    public void ConfigureServices(IServiceCollection services)
    {
        // Añade el soporte para controladores con vistas
        services.AddControllersWithViews();

        // Configura la inyección de dependencias para el repositorio.
        // Se usa el nombre de espacio completo para evitar cualquier ambigüedad.
        services.AddScoped<InmobiliariaApp.Models.RepositorioPropietario>();
    }

    // Este método se usa para configurar el pipeline de solicitudes HTTP
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        // app.UseHttpsRedirection(); // Se comenta temporalmente para evitar el error de redirección
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            // Configura el enrutamiento predeterminado para la aplicación MVC
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
