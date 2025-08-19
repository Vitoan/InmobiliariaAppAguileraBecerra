using InmobiliariaApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// MySQL (Pomelo)
var cs = builder.Configuration.GetConnectionString("InmobiliariaContext");
builder.Services.AddDbContext<InmobiliariaContext>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
