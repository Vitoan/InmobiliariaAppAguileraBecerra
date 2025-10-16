using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaAppAguileraBecerra.Models;
using System;
using System.Collections.Generic; 
using System.Linq; 

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
                // Verificar superposición
                if (_repoContrato.ExisteSuperposicion(c))
                {
                    TempData["Error"] = "Ya existe un contrato vigente o superpuesto para este inmueble en las fechas seleccionadas.";
                    ViewBag.Inquilinos = new SelectList(_repoInquilino.ObtenerTodos(), "Id", "Apellido", c.InquilinoId);
                    ViewBag.Inmuebles = new SelectList(_repoInmueble.ObtenerTodos(), "Id", "Direccion", c.InmuebleId);
                    return View(c);
                }

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
                if (_repoContrato.ExisteSuperposicion(c))
                {
                    TempData["Error"] = "Las fechas ingresadas se superponen con otro contrato para el mismo inmueble.";
                    return View(c);
                }
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
            // Se usa fechaFinAnticipada si el campo FechaFinAnticipada del contrato es null
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
        
        // --- FILTRAR CONTRATOS POR FECHA ---
        public IActionResult PorFecha(DateTime? fecha)
        {
            // Si no se proporciona una fecha, usa la actual
            DateTime fechaConsulta = fecha ?? DateTime.Today; 
            
            var contratos = _repoContrato.ObtenerVigentesEnFecha(fechaConsulta);
            
            ViewData["FechaConsulta"] = fechaConsulta.ToString("dd/MM/yyyy");

            return View("Index", contratos); 
        }

        // ====================================================================================
        // --- MÉTODO CORREGIDO: FILTRAR CONTRATOS POR INMUEBLE ---
        // Se cambió el tipo de inmuebleId a int? y se cargan los inmuebles en el ViewBag.
        // ====================================================================================
        public IActionResult PorInmueble(int? inmuebleId) 
        {
            // 1. Obtener todos los inmuebles para el dropdown de la vista
            var inmuebles = _repoInmueble.ObtenerTodos();
            ViewBag.Inmuebles = inmuebles;
            ViewBag.InmuebleSeleccionadoId = inmuebleId; // Pasa el ID seleccionado

            List<Contrato> contratos;
            
            if (inmuebleId.HasValue && inmuebleId.Value > 0)
            {
                // 2. Si se seleccionó un inmueble, FILTRAR por ese ID.
                // ¡Asegúrate que ObtenerPorInmueble() en el Repositorio esté devolviendo datos!
                contratos = _repoContrato.ObtenerPorInmueble(inmuebleId.Value);

                // Opcional: Para mostrar en la vista la dirección del inmueble
                var inmueble = inmuebles.FirstOrDefault(i => i.Id == inmuebleId.Value);
                ViewData["Title"] = $"Contratos del Inmueble: {inmueble?.Direccion}";
            }
            else
            {
                // 3. Si no hay ID o es "Todos", mostrar todos los contratos.
                contratos = _repoContrato.ObtenerTodos(); 
                ViewData["Title"] = "Contratos por Inmueble (Todos)";
            }

            // 4. Retorna la vista PorInmueble, que tiene el formulario de filtrado.
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
    }
}