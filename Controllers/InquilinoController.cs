using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize]
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

        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var i = _repositorio.ObtenerPorId(id);
            if (i == null) return NotFound();
            return View(i);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public IActionResult ConfirmarEliminar(int id)
        {
            int resultado = _repositorio.Baja(id);

            if (resultado == 0)
            {
                TempData["Error"] = "No se puede eliminar el inquilino porque tiene contratos asociados.";
            }
            else if (resultado > 0)
            {
                TempData["Success"] = "Inquilino eliminado con éxito.";
            }
            else
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el inquilino.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
