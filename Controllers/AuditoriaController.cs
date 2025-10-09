using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize(Policy = "Administrador")]
public class AuditoriaController : Controller
{
    private readonly RepositorioAuditoria repo;

    public AuditoriaController(RepositorioAuditoria repo)
    {
        this.repo = repo;
    }

    public IActionResult Index(string? tabla, string? operacion, string? usuario, DateTime? desde, DateTime? hasta)
    {
        var registros = repo.ObtenerTodos();

        if (!string.IsNullOrEmpty(tabla))
            registros = registros.Where(r => r.Tabla.Equals(tabla, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(operacion))
            registros = registros.Where(r => r.Operacion.Equals(operacion, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrEmpty(usuario))
            registros = registros.Where(r => r.Usuario != null && r.Usuario.Contains(usuario, StringComparison.OrdinalIgnoreCase)).ToList();

        if (desde != null)
            registros = registros.Where(r => r.Fecha.Date >= desde.Value.Date).ToList();

        if (hasta != null)
            registros = registros.Where(r => r.Fecha.Date <= hasta.Value.Date).ToList();

        return View(registros);
    }

    public IActionResult Detalles(int id)
    {
        var registro = repo.ObtenerPorId(id);
        if (registro == null)
            return NotFound();
        return View(registro);
    }
}

}
