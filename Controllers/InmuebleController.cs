using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaAppAguileraBecerra.Models;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
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

        // Accesibles para todos
        public IActionResult Lista(int pagina = 1)
        {
            var totalInmuebles = repositorio.ObtenerCantidad();
            var totalPaginas = (int)Math.Ceiling((double)totalInmuebles / TamañoPagina);
            var lista = repositorio.ObtenerLista(pagina, TamañoPagina);

            ViewBag.Pagina = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(lista);
        }

        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        [HttpGet]
        public ActionResult PorPropietario(int id)
        {
            var lista = repositorio.BuscarPorPropietario(id);
            return Ok(lista);
        }

        public ActionResult Ver(int id)
        {
            var entidad = id == 0 ? new Inmueble() : repositorio.ObtenerPorId(id);

            ViewBag.Propietarios = repoPropietario.ObtenerTodos()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Nombre} {p.Apellido}",
                    Selected = entidad != null && entidad.PropietarioId == p.Id
                }).ToList();

            return View(entidad);
        }

        public ActionResult Imagenes(int id, [FromServices] IRepositorioImagen repoImagen)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null) return NotFound();
            entidad.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }

        // Solo usuarios logueados
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Portada(Imagen entidad, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(entidad.InmuebleId);
                if (inmueble != null && inmueble.Portada != null)
                {
                    string rutaEliminar = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", Path.GetFileName(inmueble.Portada));
                    System.IO.File.Delete(rutaEliminar);
                }

                if (entidad.Archivo != null)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Inmuebles");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                    string fileName = "portada_" + entidad.InmuebleId + Path.GetExtension(entidad.Archivo.FileName);
                    string rutaFisicaCompleta = Path.Combine(path, fileName);

                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        entidad.Archivo.CopyTo(stream);
                    }

                    entidad.Url = Path.Combine("/Uploads/Inmuebles", fileName);
                }
                else
                {
                    entidad.Url = string.Empty;
                }

                repositorio.ModificarPortada(entidad.InmuebleId, entidad.Url);
                TempData["Mensaje"] = "Portada actualizada correctamente";
                return RedirectToAction(nameof(Index));
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
            ViewData["Propietarios"] = repoPropietario.ObtenerTodos();
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GuardarAjax(int id, Inmueble entidad)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (id == 0) id = repositorio.Alta(entidad);
                else repositorio.Modificacion(entidad);

                var res = repositorio.BuscarPorPropietario(entidad.PropietarioId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        public ActionResult Eliminar(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error")) ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        [Authorize]
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
