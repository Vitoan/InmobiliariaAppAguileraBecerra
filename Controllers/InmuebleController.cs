using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaAppAguileraBecerra.Models;
using System.Collections.Generic;
using System.IO;
using System;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioPropietario repoPropietario;
        private const int TamañoPagina = 10;

        public InmuebleController()
        {
            this.repositorio = new RepositorioInmueble();
            this.repoPropietario = new RepositorioPropietario();
        }

        public IActionResult Lista(int pagina = 1, bool soloDisponibles = false)
        {
            IList<Inmueble> lista;
            int totalInmuebles;

            if (soloDisponibles)
            {
                lista = repositorio.ObtenerDisponiblesPaginados(pagina, TamañoPagina);
                totalInmuebles = repositorio.ObtenerCantidadDisponibles();
            }
            else
            {
                lista = repositorio.ObtenerLista(pagina, TamañoPagina);
                totalInmuebles = repositorio.ObtenerCantidad();
            }

            var totalPaginas = (int)Math.Ceiling((double)totalInmuebles / TamañoPagina);

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.SoloDisponibles = soloDisponibles;

            return View(lista);
        }

        public IActionResult Index(bool soloDisponibles = false)
        {
            IList<Inmueble> inmuebles;
            if (soloDisponibles)
            {
                inmuebles = repositorio.ObtenerDisponibles();
            }
            else
            {
                inmuebles = repositorio.ObtenerTodos();
            }

            ViewBag.SoloDisponibles = soloDisponibles;
            return View(inmuebles);
        }

        [HttpGet]
        public ActionResult PorPropietario(int? id)
        {
            var propietarios = repoPropietario.ObtenerTodos();
            ViewBag.Propietarios = propietarios;

            Propietario propietarioSeleccionado = null;
            List<Inmueble> lista = new List<Inmueble>();

            if (id.HasValue)
            {
                propietarioSeleccionado = repoPropietario.ObtenerPorId(id.Value);
                if (propietarioSeleccionado != null)
                    lista = repositorio.BuscarPorPropietario(id.Value).ToList();
            }

            ViewBag.Propietario = propietarioSeleccionado;
            return View(lista);
        }

        public ActionResult Imagenes(int id, [FromServices] IRepositorioImagen repoImagen)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            entidad.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Portada(Imagen entidad, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(entidad.InmuebleId);
                if (inmueble != null && !string.IsNullOrEmpty(inmueble.Portada))
                {
                    string rutaEliminar = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", Path.GetFileName(inmueble.Portada));
                    if (System.IO.File.Exists(rutaEliminar))
                        System.IO.File.Delete(rutaEliminar);
                }

                if (entidad.Archivo != null)
                {
                    string carpetaUploads = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles");
                    if (!Directory.Exists(carpetaUploads))
                        Directory.CreateDirectory(carpetaUploads);

                    string fileName = $"portada_{entidad.InmuebleId}{Path.GetExtension(entidad.Archivo.FileName)}";
                    string rutaFisica = Path.Combine(carpetaUploads, fileName);

                    using (var stream = new FileStream(rutaFisica, FileMode.Create))
                    {
                        entidad.Archivo.CopyTo(stream);
                    }

                    entidad.Url = $"/Uploads/Inmuebles/{fileName}";
                }
                else
                {
                    entidad.Url = string.Empty;
                }

                repositorio.ModificarPortada(entidad.InmuebleId, entidad.Url);
                TempData["Mensaje"] = "Portada actualizada correctamente";
                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
            }
        }

        [Authorize]
        public ActionResult Crear()
        {
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Id"] = entidad.Id;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                return View(entidad);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(entidad);
            }
        }

        [Authorize]
        public ActionResult Editar(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error")) ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, Inmueble entidad)
        {
            try
            {
                entidad.Id = id;
                repositorio.Modificacion(entidad);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.Error = ex.Message;
                return View(entidad);
            }
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error")) ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        [Authorize(Policy = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Inmueble entidad)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(entidad);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult CambiarEstado(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null) return NotFound();
                entidad.Habilitado = !entidad.Habilitado;
                repositorio.Modificacion(entidad);
                return Ok(entidad);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
