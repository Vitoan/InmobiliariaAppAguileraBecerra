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
        private readonly RepositorioPago _repositorioPago; // 👉 agregado

        public ContratoController(
            RepositorioContrato repositorioContrato,
            RepositorioInquilino repositorioInquilino,
            RepositorioInmueble repositorioInmueble,
            RepositorioPago repositorioPago) // 👉 agregado
        {
            _repositorioContrato = repositorioContrato;
            _repositorioInquilino = repositorioInquilino;
            _repositorioInmueble = repositorioInmueble;
            _repositorioPago = repositorioPago; // 👉 agregado
        }

        public IActionResult Index()
        {
            var contratos = _repositorioContrato.ObtenerTodos();
            return View(contratos);
        }

        public IActionResult Crear()
        {
            var inquilinos = _repositorioInquilino.ObtenerTodos().Select(i => new 
            {
                Id = i.Id, NombreCompleto = $"{i.Nombre} {i.Apellido}"
            }).ToList();

            ViewBag.Inquilinos = new SelectList(inquilinos, "Id", "NombreCompleto");
            ViewBag.Inmuebles = new SelectList(_repositorioInmueble.ObtenerTodos(), "Id", "Direccion");
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

            // 👉 traemos los pagos asociados
            var pagos = _repositorioPago.ObtenerPorContrato(id);
            ViewBag.Pagos = pagos;

            return View(contrato);
        }

        public IActionResult Editar(int id)
        {
            var contrato = _repositorioContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            ViewBag.Inquilinos = new SelectList(
                _repositorioInquilino.ObtenerTodos()
                    .Select(i => new { Id = i.Id, NombreCompleto = $"{i.Nombre} {i.Apellido}" }),
                "Id", "NombreCompleto", contrato.InquilinoId
            );
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
