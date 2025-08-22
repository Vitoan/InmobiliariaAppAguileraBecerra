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

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        public IActionResult Detalles(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        public IActionResult Editar(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Propietario p)
        {
            if (id != p.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _repositorio.Modificacion(p);
                TempData["Mensaje"] = "Propietario modificado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        public IActionResult Eliminar(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminar(int id)
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Propietario eliminado con éxito.";
            return RedirectToAction(nameof(Index));
        }
    }
}