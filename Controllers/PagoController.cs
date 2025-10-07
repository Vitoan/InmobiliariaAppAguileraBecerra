using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAppAguileraBecerra.Models;
using System;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    public class PagoController : Controller
    {
        private readonly RepositorioPago _repoPago;
        private readonly RepositorioContrato _repoContrato;

        public PagoController(RepositorioPago repoPago, RepositorioContrato repoContrato)
        {
            _repoPago = repoPago;
            _repoContrato = repoContrato;
        }

        public IActionResult Index(int contratoId)
        {
            var lista = _repoPago.ObtenerPorContrato(contratoId);
            ViewBag.Contrato = _repoContrato.ObtenerPorId(contratoId);
            return View(lista);
        }

        [Authorize]
        public IActionResult Crear(int contratoId)
        {
            ViewBag.Contrato = _repoContrato.ObtenerPorId(contratoId);
            return View(new Pago { ContratoId = contratoId, Fecha = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Crear(Pago p)
        {
            if (ModelState.IsValid)
            {
                _repoPago.Alta(p);
                TempData["Mensaje"] = "Pago registrado con éxito.";
                return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
            }
            ViewBag.Contrato = _repoContrato.ObtenerPorId(p.ContratoId);
            return View(p);
        }

        [Authorize]
        public IActionResult Editar(int id)
        {
            var p = _repoPago.ObtenerPorId(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Editar(int id, Pago p)
        {
            if (id != p.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                _repoPago.Modificacion(p);
                TempData["Mensaje"] = "Pago actualizado.";
                return RedirectToAction(nameof(Index), new { contratoId = p.ContratoId });
            }
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Anular(int id)
        {
            var pago = _repoPago.ObtenerPorId(id);
            if (pago == null) return NotFound();
            _repoPago.Baja(id);
            TempData["Mensaje"] = "Pago anulado.";
            return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
        }
    }
}
