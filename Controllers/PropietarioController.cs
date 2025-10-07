using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario _repositorio;

        public PropietarioController(RepositorioPropietario repositorio)
        {
            _repositorio = repositorio;
        }

        public IActionResult Index()
        {
            var propietarios = _repositorio.ObtenerTodos();
            return View(propietarios);
        }

        [Authorize]
        public IActionResult Detalles(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [Authorize]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Crear(Propietario p)
        {
            if (ModelState.IsValid)
            {
                _repositorio.Alta(p);
                TempData["Mensaje"] = "Propietario creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [Authorize]
        public IActionResult Editar(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Editar(int id, Propietario p)
        {
            if (id != p.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _repositorio.Modificacion(p);
                TempData["Mensaje"] = "Propietario modificado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        [Authorize]
        public IActionResult Eliminar(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult ConfirmarEliminar(int id)
        {
            int resultado = _repositorio.Baja(id);

            if (resultado == 0)
                TempData["Error"] = "No se puede eliminar el propietario porque tiene contratos asociados.";
            else if (resultado > 0)
                TempData["Success"] = "Propietario eliminado con éxito.";
            else
                TempData["Error"] = "Ocurrió un error al intentar eliminar el propietario.";

            return RedirectToAction(nameof(Index));
        }
    }
}
