using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    public class ContratoController : Controller
    {
        private readonly RepositorioContrato _repositorioContrato;
        private readonly RepositorioInquilino _repositorioInquilino;
        private readonly RepositorioInmueble _repositorioInmueble;

        public ContratoController(RepositorioContrato repositorioContrato, RepositorioInquilino repositorioInquilino, RepositorioInmueble repositorioInmueble)
        {
            _repositorioContrato = repositorioContrato;
            _repositorioInquilino = repositorioInquilino;
            _repositorioInmueble = repositorioInmueble;
        }

        public IActionResult Index()
        {
            var contratos = _repositorioContrato.ObtenerTodos();
            return View(contratos);
        }

        public IActionResult Crear()
        {
            ViewBag.Inquilinos = new SelectList(_repositorioInquilino.ObtenerTodos(), "Id", "Nombre", null);
            ViewBag.Inmuebles = new SelectList(_repositorioInmueble.ObtenerTodos(), "Id", "Direccion", null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Contrato c)
        {
            if (ModelState.IsValid)
            {
                _repositorioContrato.Alta(c);
                TempData["Mensaje"] = "Contrato creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Inquilinos = new SelectList(_repositorioInquilino.ObtenerTodos(), "Id", "Nombre", c.InquilinoId);
            ViewBag.Inmuebles = new SelectList(_repositorioInmueble.ObtenerTodos(), "Id", "Direccion", c.InmuebleId);
            return View(c);
        }

        public IActionResult Detalles(int id)
        {
            var contrato = _repositorioContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        public IActionResult Editar(int id)
        {
            var contrato = _repositorioContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            ViewBag.Inquilinos = new SelectList(_repositorioInquilino.ObtenerTodos(), "Id", "Nombre", contrato.InquilinoId);
            ViewBag.Inmuebles = new SelectList(_repositorioInmueble.ObtenerTodos(), "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Contrato c)
        {
            if (id != c.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _repositorioContrato.Modificacion(c);
                TempData["Mensaje"] = "Contrato modificado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Inquilinos = new SelectList(_repositorioInquilino.ObtenerTodos(), "Id", "Nombre", c.InquilinoId);
            ViewBag.Inmuebles = new SelectList(_repositorioInmueble.ObtenerTodos(), "Id", "Direccion", c.InmuebleId);
            return View(c);
        }
    }
}