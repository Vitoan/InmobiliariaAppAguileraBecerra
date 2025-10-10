using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaAppAguileraBecerra.Models;
using System;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly RepositorioContrato _repoContrato;
        private readonly RepositorioInquilino _repoInquilino;
        private readonly RepositorioInmueble _repoInmueble;

        public ContratoController()
        {
            _repoContrato = new RepositorioContrato();
            _repoInquilino = new RepositorioInquilino();
            _repoInmueble = new RepositorioInmueble();
        }

        // --- LISTAR CONTRATOS ---
        public IActionResult Index()
        {
            var contratos = _repoContrato.ObtenerTodos();
            return View(contratos);
        }

        // --- DETALLES ---
        public IActionResult Detalles(int id)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            return View(contrato);
        }

        // --- CREAR ---
        [Authorize]
        public IActionResult Crear()
        {
            ViewBag.Inquilinos = new SelectList(_repoInquilino.ObtenerTodos(), "Id", "Apellido");
            ViewBag.Inmuebles = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Crear(Contrato c)
        {
            if (ModelState.IsValid)
            {
                _repoContrato.Alta(c);
                TempData["Mensaje"] = "Contrato creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Inquilinos = new SelectList(_repoInquilino.ObtenerTodos(), "Id", "Apellido", c.InquilinoId);
            ViewBag.Inmuebles = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion", c.InmuebleId);
            return View(c);
        }

        // --- EDITAR ---
        [Authorize]
        public IActionResult Editar(int id)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();

            ViewBag.Inquilinos = new SelectList(_repoInquilino.ObtenerTodos(), "Id", "Apellido", contrato.InquilinoId);
            ViewBag.Inmuebles = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Editar(int id, Contrato c)
        {
            if (id != c.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _repoContrato.Modificacion(c);
                TempData["Mensaje"] = "Contrato modificado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(c);
        }

        // --- ELIMINAR ---
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public IActionResult ConfirmarEliminar(int id)
        {
            _repoContrato.Baja(id);
            TempData["Mensaje"] = "Contrato eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // --- NUEVO MÉTODO: FINALIZAR ANTICIPADAMENTE ---
        [HttpPost]
[ValidateAntiForgeryToken]
public IActionResult FinalizarAnticipado(int id, DateTime fechaFinAnticipada)
{
    var contrato = _repoContrato.ObtenerPorId(id);
    if (contrato == null)
        return NotFound();

    if (fechaFinAnticipada <= contrato.FechaInicio)
    {
        TempData["Error"] = "La fecha anticipada debe ser posterior al inicio.";
        return RedirectToAction("Detalles", new { id });
    }

    // Obtener cantidad de pagos realizados
    var repoPago = new RepositorioPago();
    int cantidadPagos = repoPago.ContarPorContrato(id);

    // Calcular meses que debería haber pagado
    int mesesTotales = (int)Math.Ceiling((contrato.FechaFinAnticipada ?? fechaFinAnticipada).Subtract(contrato.FechaInicio).TotalDays / 30.0);
    int mesesPagados = cantidadPagos;
    int mesesAdeudados = Math.Max(0, mesesTotales - mesesPagados);

    // Calcular deuda
    decimal deuda = mesesAdeudados * contrato.Monto;

    // Cálculo de multa según proporción del contrato cumplido
    double totalDias = (contrato.FechaFin - contrato.FechaInicio).TotalDays;
    double diasCumplidos = (fechaFinAnticipada - contrato.FechaInicio).TotalDays;
    double proporcion = diasCumplidos / totalDias;

    decimal multa = proporcion < 0.5 ? contrato.Monto * 2 : contrato.Monto;

    // Total a pagar
    decimal totalPendiente = deuda + multa;

    // Actualizar contrato
    contrato.FechaFinAnticipada = fechaFinAnticipada;
    contrato.Multa = multa;
    contrato.Vigente = false;
    _repoContrato.FinalizarAnticipado(contrato);

    TempData["Mensaje"] = $"Contrato finalizado anticipadamente. Multa: ${multa:N2}. Deuda: ${deuda:N2}. Total a abonar: ${totalPendiente:N2}";
    return RedirectToAction("Detalles", new { id });
}

    }
}
