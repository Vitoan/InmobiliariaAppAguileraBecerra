using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino _repositorio;

        public InquilinoController(RepositorioInquilino repositorio)
        {
            _repositorio = repositorio;
        }

        public IActionResult Index()
        {
            var inquilinos = _repositorio.ObtenerTodos();
            return View(inquilinos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Inquilino i)
        {
            if (ModelState.IsValid)
            {
                _repositorio.Alta(i);
                TempData["Mensaje"] = "Inquilino creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(i);
        }

        public IActionResult Detalles(int id)
        {
            var i = _repositorio.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        public IActionResult Editar(int id)
        {
            var i = _repositorio.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Inquilino i)
        {
            if (id != i.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _repositorio.Modificacion(i);
                TempData["Mensaje"] = "Inquilino modificado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(i);
        }

        public IActionResult Eliminar(int id)
        {
            var i = _repositorio.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminar(int id)
        {
            _repositorio.Baja(id);
            TempData["Mensaje"] = "Inquilino eliminado con éxito.";
            return RedirectToAction(nameof(Index));
        }
    }
}