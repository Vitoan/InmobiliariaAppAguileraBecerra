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

                var inmueble = repoInmueble.ObtenerPorId(contrato.InmuebleId);
                if (inmueble != null)
                {
                    inmueble.Disponible = false;
                    repoInmueble.Modificacion(inmueble);
                }

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

                var inmueble = repoInmueble.ObtenerPorId(contrato.InmuebleId);
                if (inmueble != null)
                {
                    inmueble.Disponible = true;
                    repoInmueble.Modificacion(inmueble);
                }

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

        [HttpGet]
        public IActionResult PorFecha(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var contratos = repoContrato.ObtenerTodos();

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                ViewBag.FechaInicio = fechaInicio.Value;
                ViewBag.FechaFin = fechaFin.Value;

                contratos = contratos
                    .Where(c => c.Vigente &&
                                c.FechaInicio <= fechaFin.Value &&
                                c.FechaFin >= fechaInicio.Value)
                    .ToList();
            }

            return View(contratos);
        }

        [HttpGet]
        public IActionResult PorInmueble(int? inmuebleId)
        {
            var inmuebles = repoInmueble.ObtenerTodos();
            ViewBag.Inmuebles = inmuebles;
            ViewBag.InmuebleSeleccionadoId = inmuebleId;

            IEnumerable<Contrato> contratos = repoContrato.ObtenerTodos();

            if (inmuebleId.HasValue)
                contratos = contratos.Where(c => c.InmuebleId == inmuebleId.Value);

            return View(contratos);
        }
    }
}
