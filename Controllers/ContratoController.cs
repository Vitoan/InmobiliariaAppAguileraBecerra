using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly RepositorioContrato repoContrato;
        private readonly RepositorioInquilino repoInquilino;
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioAuditoria repoAuditoria;
        private readonly ILogger<ContratoController> logger;

        public ContratoController(
            RepositorioContrato repoContrato,
            RepositorioInquilino repoInquilino,
            RepositorioInmueble repoInmueble,
            RepositorioAuditoria repoAuditoria,
            ILogger<ContratoController> logger)
        {
            this.repoContrato = repoContrato;
            this.repoInquilino = repoInquilino;
            this.repoInmueble = repoInmueble;
            this.repoAuditoria = repoAuditoria;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            var lista = repoContrato.ObtenerTodos();
            return View(lista);
        }

        public IActionResult Detalles(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            var auditorias = repoAuditoria.ObtenerPorTablaYRegistro("Contrato", id);
            ViewBag.Auditorias = auditorias;

            return View(contrato);
        }

        public IActionResult Crear()
        {
            ViewBag.Inquilinos = repoInquilino.ObtenerTodos()
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Nombre} {i.Apellido}"
                }).ToList();

            ViewBag.Inmuebles = repoInmueble.ObtenerDisponibles()
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Direccion
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos()
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = $"{i.Nombre} {i.Apellido}"
                    }).ToList();

                ViewBag.Inmuebles = repoInmueble.ObtenerDisponibles()
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Direccion
                    }).ToList();

                return View(contrato);
            }

            try
            {
                repoContrato.Alta(contrato);

                var datosNuevos = new
                {
                    contrato.Id,
                    contrato.FechaInicio,
                    contrato.FechaFin,
                    contrato.Monto,
                    contrato.InquilinoId,
                    contrato.InmuebleId,
                    contrato.Vigente
                };

                var auditoria = new Auditoria
                {
                    Tabla = "Contrato",
                    Operacion = "Alta",
                    RegistroId = contrato.Id,
                    DatosNuevos = JsonSerializer.Serialize(datosNuevos),
                    Usuario = User?.Identity?.Name ?? "Sistema"
                };
                repoAuditoria.Registrar(auditoria);

                TempData["Mensaje"] = "Contrato creado con éxito";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear contrato");
                ModelState.AddModelError("", ex.Message);

                ViewBag.Inquilinos = repoInquilino.ObtenerTodos()
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = $"{i.Nombre} {i.Apellido}"
                    }).ToList();

                ViewBag.Inmuebles = repoInmueble.ObtenerDisponibles()
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Direccion
                    }).ToList();

                return View(contrato);
            }
        }

        public IActionResult Editar(int id)
        {
            var contrato = repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            ViewBag.Inquilinos = repoInquilino.ObtenerTodos()
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Nombre} {i.Apellido}"
                }).ToList();

            ViewBag.Inmuebles = repoInmueble.ObtenerDisponibles()
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = i.Direccion
                }).ToList();

            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Inquilinos = repoInquilino.ObtenerTodos()
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = $"{i.Nombre} {i.Apellido}"
                    }).ToList();

                ViewBag.Inmuebles = repoInmueble.ObtenerDisponibles()
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Direccion
                    }).ToList();

                return View(contrato);
            }

            try
            {
                var anterior = repoContrato.ObtenerPorId(id);
                if (anterior == null)
                    return NotFound();

                repoContrato.Modificacion(contrato);

                var datosAnteriores = new
                {
                    anterior.Id,
                    anterior.FechaInicio,
                    anterior.FechaFin,
                    anterior.Monto,
                    anterior.InquilinoId,
                    anterior.InmuebleId,
                    anterior.Vigente
                };

                var datosNuevos = new
                {
                    contrato.Id,
                    contrato.FechaInicio,
                    contrato.FechaFin,
                    contrato.Monto,
                    contrato.InquilinoId,
                    contrato.InmuebleId,
                    contrato.Vigente
                };

                var auditoria = new Auditoria
                {
                    Tabla = "Contrato",
                    Operacion = "Modificación",
                    RegistroId = contrato.Id,
                    DatosAnteriores = JsonSerializer.Serialize(datosAnteriores),
                    DatosNuevos = JsonSerializer.Serialize(datosNuevos),
                    Usuario = User?.Identity?.Name ?? "Sistema"
                };
                repoAuditoria.Registrar(auditoria);

                TempData["Mensaje"] = "Contrato modificado correctamente";
                return RedirectToAction(nameof(Detalles), new { id = contrato.Id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al modificar contrato");
                ModelState.AddModelError("", ex.Message);
                return View(contrato);
            }
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id, DateTime fechaFinAnticipada, decimal multa)
        {
            try
            {
                var contrato = repoContrato.ObtenerPorId(id);
                if (contrato == null)
                    return NotFound();

                var anterior = new
                {
                    contrato.Id,
                    contrato.FechaInicio,
                    contrato.FechaFin,
                    contrato.Monto,
                    contrato.InquilinoId,
                    contrato.InmuebleId,
                    contrato.Vigente,
                    contrato.Multa,
                    contrato.FechaFinAnticipada
                };

                contrato.FechaFinAnticipada = fechaFinAnticipada;
                contrato.Multa = multa;
                contrato.Vigente = false;
                repoContrato.Modificacion(contrato);

                var nuevo = new
                {
                    contrato.Id,
                    contrato.FechaInicio,
                    contrato.FechaFin,
                    contrato.Monto,
                    contrato.InquilinoId,
                    contrato.InmuebleId,
                    contrato.Vigente,
                    contrato.Multa,
                    contrato.FechaFinAnticipada
                };

                var auditoria = new Auditoria
                {
                    Tabla = "Contrato",
                    Operacion = "Terminación",
                    RegistroId = contrato.Id,
                    DatosAnteriores = JsonSerializer.Serialize(anterior),
                    DatosNuevos = JsonSerializer.Serialize(nuevo),
                    Usuario = User?.Identity?.Name ?? "Sistema"
                };
                repoAuditoria.Registrar(auditoria);

                TempData["Mensaje"] = "Contrato finalizado correctamente";
                return RedirectToAction(nameof(Detalles), new { id = contrato.Id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al terminar contrato");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Detalles), new { id });
            }
        }
    }
}
