using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly RepositorioPago repoPago;
        private readonly RepositorioContrato repoContrato;
        private readonly RepositorioAuditoria repoAuditoria;
        private readonly ILogger<PagoController> logger;

        public PagoController(
            RepositorioPago repoPago,
            RepositorioContrato repoContrato,
            RepositorioAuditoria repoAuditoria,
            ILogger<PagoController> logger)
        {
            this.repoPago = repoPago;
            this.repoContrato = repoContrato;
            this.repoAuditoria = repoAuditoria;
            this.logger = logger;
        }

        // ✅ LISTADO DE PAGOS
        public IActionResult Index(int? contratoId)
        {
            try
            {
                IList<Pago> lista = contratoId.HasValue
                    ? repoPago.ObtenerPorContrato(contratoId.Value)
                    : repoPago.ObtenerTodos();

                ViewBag.ContratoId = contratoId;
                ViewBag.Contrato = contratoId.HasValue ? repoContrato.ObtenerPorId(contratoId.Value) : null;

                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener pagos");
                TempData["Error"] = "Ocurrió un error al cargar los pagos.";
                return RedirectToAction("Index", "Home");
            }
        }

        // ✅ FORMULARIO DE CREACIÓN
        public IActionResult Crear(int contratoId)
        {
            try
            {
                var contrato = repoContrato.ObtenerPorId(contratoId);
                if (contrato == null)
                    return NotFound("Contrato no encontrado");

                ViewBag.Contrato = contrato;
                return View(new Pago { ContratoId = contratoId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al preparar creación de pago");
                TempData["Error"] = "No se pudo preparar la creación del pago.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ✅ POST DE CREACIÓN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Contrato = repoContrato.ObtenerPorId(pago.ContratoId);
                return View(pago);
            }

            try
            {
                repoPago.Alta(pago);

                // Auditoría
                var datosNuevos = new
                {
                    pago.Id,
                    pago.Numero,
                    pago.Fecha,
                    pago.Importe,
                    pago.Detalle,
                    pago.Anulado,
                    pago.ContratoId
                };

                var auditoria = new Auditoria
                {
                    Tabla = "Pago",
                    Operacion = "Alta",
                    RegistroId = pago.Id,
                    DatosNuevos = JsonSerializer.Serialize(datosNuevos),
                    Usuario = User?.Identity?.Name ?? "Sistema",
                    Fecha = DateTime.Now
                };

                repoAuditoria.Registrar(auditoria);

                TempData["Mensaje"] = "Pago registrado correctamente.";
                return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al registrar pago");
                ModelState.AddModelError("", "Ocurrió un error al registrar el pago: " + ex.Message);
                ViewBag.Contrato = repoContrato.ObtenerPorId(pago.ContratoId);
                return View(pago);
            }
        }

        // ✅ ANULAR PAGO (Solo administrador)
        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id)
        {
            try
            {
                var pago = repoPago.ObtenerPorId(id);
                if (pago == null)
                    return NotFound("Pago no encontrado");

                var datosAnteriores = new
                {
                    pago.Id,
                    pago.Numero,
                    pago.Fecha,
                    pago.Importe,
                    pago.Detalle,
                    pago.Anulado
                };

                pago.Anulado = true;
                repoPago.Modificacion(pago);

                var datosNuevos = new
                {
                    pago.Id,
                    pago.Numero,
                    pago.Fecha,
                    pago.Importe,
                    pago.Detalle,
                    pago.Anulado
                };

                var auditoria = new Auditoria
                {
                    Tabla = "Pago",
                    Operacion = "Anulación",
                    RegistroId = pago.Id,
                    DatosAnteriores = JsonSerializer.Serialize(datosAnteriores),
                    DatosNuevos = JsonSerializer.Serialize(datosNuevos),
                    Usuario = User?.Identity?.Name ?? "Sistema",
                    Fecha = DateTime.Now
                };

                repoAuditoria.Registrar(auditoria);

                TempData["Mensaje"] = "Pago anulado correctamente.";
                return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al anular pago");
                TempData["Error"] = "No se pudo anular el pago: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
