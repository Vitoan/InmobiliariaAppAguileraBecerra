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

        public IActionResult Index(int? contratoId)
        {
            IList<Pago> lista;
            if (contratoId.HasValue)
                lista = repoPago.ObtenerPorContrato(contratoId.Value);
            else
                lista = repoPago.ObtenerTodos();

            ViewBag.ContratoId = contratoId;
            ViewBag.Contrato = contratoId.HasValue ? repoContrato.ObtenerPorId(contratoId.Value) : null;
            return View(lista);
        }

        public IActionResult Crear(int contratoId)
        {
            ViewBag.Contrato = repoContrato.ObtenerPorId(contratoId);
            return View(new Pago { ContratoId = contratoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Pago pago)
        {
            if (!ModelState.IsValid)
                return View(pago);

            try
            {
                repoPago.Alta(pago);

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
                    Usuario = User?.Identity?.Name ?? "Sistema"
                };
                repoAuditoria.Registrar(auditoria);

                TempData["Mensaje"] = "Pago registrado correctamente";
                return RedirectToAction("Index", new { contratoId = pago.ContratoId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al registrar pago");
                ModelState.AddModelError("", ex.Message);
                ViewBag.Contrato = repoContrato.ObtenerPorId(pago.ContratoId);
                return View(pago);
            }
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Anular(int id)
        {
            try
            {
                var pago = repoPago.ObtenerPorId(id);
                if (pago == null)
                    return NotFound();

                var anterior = new
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

                var nuevo = new
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
                    DatosAnteriores = JsonSerializer.Serialize(anterior),
                    DatosNuevos = JsonSerializer.Serialize(nuevo),
                    Usuario = User?.Identity?.Name ?? "Sistema"
                };
                repoAuditoria.Registrar(auditoria);

                TempData["Mensaje"] = "Pago anulado correctamente";
                return RedirectToAction("Index", new { contratoId = pago.ContratoId });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al anular pago");
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
