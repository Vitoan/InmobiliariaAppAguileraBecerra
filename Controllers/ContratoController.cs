using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaAppAguileraBecerra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly RepositorioContrato _repoContrato;
        private readonly RepositorioInquilino _repoInquilino;
        private readonly RepositorioInmueble _repoInmueble;
        private readonly RepositorioAuditoria _repoAuditoria;
        private readonly RepositorioPago _repoPago;

        // Opciones JSON con formato legible
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        public ContratoController(
            RepositorioContrato repoContrato,
            RepositorioInquilino repoInquilino,
            RepositorioInmueble repoInmueble,
            RepositorioAuditoria repoAuditoria,
            RepositorioPago repoPago)
        {
            _repoContrato = repoContrato;
            _repoInquilino = repoInquilino;
            _repoInmueble = repoInmueble;
            _repoAuditoria = repoAuditoria;
            _repoPago = repoPago;
        }

        // --- INDEX ---
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
            if (!ModelState.IsValid)
            {
                CargarCombos(c);
                return View(c);
            }

            // Validar superposición
            if (_repoContrato.ExisteSuperposicion(c))
            {
                TempData["Error"] = "Ya existe un contrato vigente o superpuesto para este inmueble en las fechas seleccionadas.";
                CargarCombos(c);
                return View(c);
            }

            _repoContrato.Alta(c);

            // --- AUDITORÍA: CREACIÓN ---
            var auditoria = new Auditoria
            {
                Tabla = "Contrato",
                Operacion = "Alta",
                RegistroId = c.Id,
                DatosNuevos = JsonSerializer.Serialize(c, _jsonOptions),
                Usuario = User?.Identity?.Name ?? "Sistema"
            };
            _repoAuditoria.Registrar(auditoria);

            TempData["Mensaje"] = "Contrato creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // --- EDITAR ---
        [Authorize]
        public IActionResult Editar(int id)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            CargarCombos(contrato);
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Editar(int id, Contrato c)
        {
            if (id != c.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                CargarCombos(c);
                return View(c);
            }

            if (_repoContrato.ExisteSuperposicion(c))
            {
                TempData["Error"] = "Las fechas ingresadas se superponen con otro contrato para el mismo inmueble.";
                CargarCombos(c);
                return View(c);
            }

            // Auditoría: estado anterior
            var original = _repoContrato.ObtenerPorId(id);
            var datosAnteriores = JsonSerializer.Serialize(original, _jsonOptions);

            _repoContrato.Modificacion(c);

            // Auditoría: estado nuevo
            var datosNuevos = JsonSerializer.Serialize(c, _jsonOptions);
            var auditoria = new Auditoria
            {
                Tabla = "Contrato",
                Operacion = "Modificación",
                RegistroId = c.Id,
                DatosAnteriores = datosAnteriores,
                DatosNuevos = datosNuevos,
                Usuario = User?.Identity?.Name ?? "Sistema"
            };
            _repoAuditoria.Registrar(auditoria);

            TempData["Mensaje"] = "Contrato modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // --- FINALIZAR ANTICIPADAMENTE ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizarAnticipado(int id, DateTime fechaFinAnticipada)
        {
            var contrato = _repoContrato.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            if (fechaFinAnticipada <= contrato.FechaInicio)
            {
                TempData["Error"] = "La fecha anticipada debe ser posterior al inicio del contrato.";
                return RedirectToAction("Detalles", new { id });
            }

            // Estado anterior
            var datosAnteriores = JsonSerializer.Serialize(new
            {
                contrato.Vigente,
                contrato.FechaFinAnticipada,
                contrato.Multa
            }, _jsonOptions);

            // Cálculos
            int cantidadPagos = _repoPago.ContarPorContrato(id);
            double totalDias = (contrato.FechaFin - contrato.FechaInicio).TotalDays;
            double diasCumplidos = (fechaFinAnticipada - contrato.FechaInicio).TotalDays;
            double proporcion = diasCumplidos / totalDias;
            int mesesTotales = (int)Math.Ceiling(totalDias / 30.0);
            int mesesPagados = cantidadPagos;
            int mesesAdeudados = Math.Max(0, mesesTotales - mesesPagados);
            decimal deuda = mesesAdeudados * contrato.Monto;
            decimal multa = proporcion < 0.5 ? contrato.Monto * 2 : contrato.Monto;
            decimal totalPendiente = deuda + multa;

            // Actualizar contrato
            contrato.FechaFinAnticipada = fechaFinAnticipada;
            contrato.Multa = multa;
            contrato.Vigente = false;
            _repoContrato.FinalizarAnticipado(contrato);

            // Auditoría
            var datosNuevos = JsonSerializer.Serialize(new
            {
                contrato.Vigente,
                contrato.FechaFinAnticipada,
                contrato.Multa
            }, _jsonOptions);

            var auditoria = new Auditoria
            {
                Tabla = "Contrato",
                Operacion = "Terminación Anticipada",
                RegistroId = contrato.Id,
                DatosAnteriores = datosAnteriores,
                DatosNuevos = datosNuevos,
                Usuario = User?.Identity?.Name ?? "Sistema"
            };
            _repoAuditoria.Registrar(auditoria);

            TempData["Mensaje"] = $"Contrato finalizado anticipadamente. Multa: ${multa:N2}, Deuda: ${deuda:N2}, Total a abonar: ${totalPendiente:N2}";
            return RedirectToAction("Detalles", new { id });
        }

        // --- FILTROS ---
        public IActionResult PorFecha(DateTime? fecha)
        {
            DateTime fechaConsulta = fecha ?? DateTime.Today;
            var contratos = _repoContrato.ObtenerVigentesEnFecha(fechaConsulta);
            ViewData["FechaConsulta"] = fechaConsulta.ToString("dd/MM/yyyy");
            return View("Index", contratos);
        }

        public IActionResult PorInmueble(int? inmuebleId)
        {
            var inmuebles = _repoInmueble.ObtenerTodos();
            ViewBag.Inmuebles = inmuebles;
            ViewBag.InmuebleSeleccionadoId = inmuebleId;

            List<Contrato> contratos;

            if (inmuebleId.HasValue && inmuebleId.Value > 0)
            {
                contratos = _repoContrato.ObtenerPorInmueble(inmuebleId.Value);
                var inmueble = inmuebles.FirstOrDefault(i => i.Id == inmuebleId.Value);
                ViewData["Title"] = $"Contratos del Inmueble: {inmueble?.Direccion}";
            }
            else
            {
                contratos = _repoContrato.ObtenerTodos();
                ViewData["Title"] = "Contratos por Inmueble (Todos)";
            }

            return View("PorInmueble", contratos);
        }

        [HttpGet]
        public JsonResult VerificarSuperposicion(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int id = 0)
        {
            var contratoTemp = new Contrato
            {
                Id = id,
                InmuebleId = inmuebleId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            bool existe = _repoContrato.ExisteSuperposicion(contratoTemp);
            return Json(new { existe });
        }

        // --- MÉTODO AUXILIAR ---
        private void CargarCombos(Contrato c)
        {
            ViewBag.Inquilinos = new SelectList(_repoInquilino.ObtenerTodos(), "Id", "Apellido", c.InquilinoId);
            ViewBag.Inmuebles = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion", c.InmuebleId);
        }
    }
}
