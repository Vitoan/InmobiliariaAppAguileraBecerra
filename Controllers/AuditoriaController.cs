using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly RepositorioAuditoria _repo;

        public AuditoriaController()
        {
            _repo = new RepositorioAuditoria();
        }

        public IActionResult Index(string? tabla, string? operacion, string? usuario, DateTime? desde, DateTime? hasta)
        {
            var auditorias = _repo.ObtenerConFiltros(tabla, operacion, usuario, desde, hasta);
            ViewData["Filtros"] = new { tabla, operacion, usuario, desde, hasta };
            return View(auditorias);
        }

        public IActionResult Detalles(int id)
        {
            var auditoria = _repo.ObtenerPorId(id);
            if (auditoria == null)
            {
                return NotFound();
            }
            return View(auditoria);
        }
    }
}
