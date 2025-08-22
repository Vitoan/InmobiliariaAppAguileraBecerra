using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;
using System.Collections.Generic;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    // El controlador maneja las solicitudes relacionadas con la entidad Propietario.
    public class PropietarioController : Controller
    {
        // Instancia del repositorio para interactuar con la base de datos.
        private readonly RepositorioPropietario _repositorio;

        // Constructor que inicializa el repositorio a través de la inyección de dependencias.
        public PropietarioController(RepositorioPropietario repositorio)
        {
            _repositorio = repositorio;
        }

        // GET: Propietarios
        // Acción para mostrar la lista de todos los propietarios (Consulta).
        public IActionResult Index()
        {
            var propietarios = _repositorio.ObtenerTodos();
            return View(propietarios);
        }

        // GET: Propietarios/Crear
        // Muestra el formulario para crear un nuevo propietario (Alta).
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Propietarios/Crear
        // Procesa la información del formulario de creación.
        [HttpPost]
        [ValidateAntiForgeryToken] // Previene ataques de falsificación de solicitudes.
        public IActionResult Crear(Propietario p)
        {
            if (ModelState.IsValid) // Verifica si el modelo es válido.
            {
                _repositorio.Alta(p);
                TempData["Mensaje"] = "Propietario creado con éxito."; // Mensaje temporal para la vista.
                return RedirectToAction(nameof(Index));
            }
            return View(p); // Si el modelo no es válido, vuelve a mostrar el formulario.
        }

        // GET: Propietarios/Detalles/5
        // Muestra los detalles de un propietario específico (Consulta).
        public IActionResult Detalles(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        // GET: Propietarios/Editar/5
        // Muestra el formulario para editar un propietario.
        public IActionResult Editar(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        // POST: Propietarios/Editar/5
        // Procesa la información del formulario de edición (Modificación).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Propietario p)
        {
            if (id != p.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _repositorio.Modificacion(p); // Se usa "Modificacion" para coincidir con el repositorio
                TempData["Mensaje"] = "Propietario modificado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        // GET: Propietarios/Eliminar/5
        // Muestra la confirmación de eliminación.
        public IActionResult Eliminar(int id)
        {
            var p = _repositorio.ObtenerPorId(id);
            if (p == null)
            {
                return NotFound();
            }
            return View(p);
        }

        // POST: Propietarios/Eliminar/5
        // Procesa la eliminación del propietario (Eliminar).
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarEliminar(int id)
        {
            _repositorio.Baja(id); // Se usa "Baja" para coincidir con el repositorio
            TempData["Mensaje"] = "Propietario eliminado con éxito.";
            return RedirectToAction(nameof(Index));
        }
    }
}
