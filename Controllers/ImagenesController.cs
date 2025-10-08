using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaAppAguileraBecerra.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    [Authorize]
    public class ImagenesController : Controller
    {
        private readonly IRepositorioImagen _repositorio;

        public ImagenesController(IRepositorioImagen repositorio)
        {
            _repositorio = repositorio;
        }

        [HttpPost]
        public async Task<IActionResult> Alta(int id, List<IFormFile> imagenes, [FromServices] IWebHostEnvironment environment)
        {
            if (imagenes == null || imagenes.Count == 0)
                return BadRequest("No se recibieron archivos.");

            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "uploads", "inmuebles", id.ToString());

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var listaUrls = new List<object>();

            foreach (var file in imagenes)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(path, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var imagen = new Imagen
                    {
                        InmuebleId = id,
                        Url = $"/uploads/inmuebles/{id}/{nombreArchivo}"
                    };
                    _repositorio.Agregar(imagen);

                    listaUrls.Add(new { id = imagen.Id, url = imagen.Url });
                }
            }

            return Json(listaUrls);
        }

        [HttpPost]
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var img = _repositorio.ObtenerPorId(id);
                if (img == null)
                    return NotFound();

                // Eliminar archivo físico
                var rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(rutaFisica))
                    System.IO.File.Delete(rutaFisica);

                // Eliminar de la DB
                _repositorio.Eliminar(id);

                // Devolver lista actualizada
                var listaActualizada = _repositorio.BuscarPorInmueble(img.InmuebleId)
                    .Select(i => new { id = i.Id, url = i.Url })
                    .ToList();

                return Json(listaActualizada);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
