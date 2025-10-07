using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InmobiliariaAppAguileraBecerra.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InmobiliariaAppAguileraBecerra.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> logger;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly RepositorioUsuario repositorio;

        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment, RepositorioUsuario repositorio, ILogger<UsuariosController> logger)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = repositorio;
            this.logger = logger;
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Index(int pagina = 1)
        {
            var usuarios = repositorio.ObtenerTodos();
            return View(usuarios);
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var e = repositorio.ObtenerPorId(id);
            return View(e);
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: u.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                u.Clave = hashed;
                int res = repositorio.Alta(u);
                if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorio.Modificacion(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al crear el usuario");
                ViewBag.Error = ex.Message;
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
        }

        [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var email = User?.Identity?.Name;
            var u = string.IsNullOrEmpty(email) ? null : repositorio.ObtenerPorEmail(email);
            if (u == null)
                return NotFound();
            return View(u);
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = repositorio.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u)
        {
            var vista = nameof(Edit);
            try
            {
                if (!User.IsInRole("Administrador"))
                {
                    vista = nameof(Perfil);
                    var email = User?.Identity?.Name;
                    var usuarioActual = String.IsNullOrEmpty(email) ? null : repositorio.ObtenerPorEmail(email);
                    if (usuarioActual?.Id != id)
                        return RedirectToAction(nameof(Index), "Home");
                }

                repositorio.Modificacion(u);
                return RedirectToAction(vista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar el usuario");
                throw;
            }
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
            var u = repositorio.ObtenerPorId(id);
            if (u == null)
                return NotFound();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Usuario usuario)
        {
            try
            {
                var ruta = Path.Combine(environment.WebRootPath, "Uploads", $"avatar_{id}" + Path.GetExtension(usuario.Avatar));
                if (System.IO.File.Exists(ruta))
                    System.IO.File.Delete(ruta);
                repositorio.Baja(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar el usuario");
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public IActionResult Avatar()
        {
            var email = User?.Identity?.Name;
            var u = String.IsNullOrEmpty(email) ? null : repositorio.ObtenerPorEmail(email);
            if (u == null || string.IsNullOrEmpty(u.Avatar))
                return NotFound();
            string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [Authorize]
        public string AvatarBase64()
        {
            var email = User?.Identity?.Name;
            var u = String.IsNullOrEmpty(email) ? null : repositorio.ObtenerPorEmail(email);
            if (u == null || string.IsNullOrEmpty(u.Avatar))
                return "";
            string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            return Convert.ToBase64String(fileBytes);
        }

        [Authorize]
        [HttpPost("[controller]/[action]/{fileName}")]
        public IActionResult FromBase64([FromBody] string imagen, [FromRoute] string fileName)
        {
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);
            var bytes = Convert.FromBase64String(imagen);
            System.IO.File.WriteAllBytes(pathCompleto, bytes);
            return Ok();
        }

        [Authorize]
        public ActionResult Foto()
        {
            var email = User?.Identity?.Name;
            var u = String.IsNullOrEmpty(email) ? null : repositorio.ObtenerPorEmail(email);
            if (u == null || string.IsNullOrEmpty(u.Avatar))
                return NotFound();
            var stream = System.IO.File.Open(Path.Combine(environment.WebRootPath, u.Avatar.Substring(1)), FileMode.Open, FileAccess.Read);
            var ext = Path.GetExtension(u.Avatar);
            return new FileStreamResult(stream, $"image/{ext.Substring(1)}");
        }

        [Authorize]
        public ActionResult Datos()
        {
            var u = repositorio.ObtenerPorEmail(User?.Identity?.Name ?? "");
            if (u == null)
                return NotFound();
            string buffer = "Nombre;Apellido;Email" + Environment.NewLine + $"{u.Nombre};{u.Apellido};{u.Email}";
            var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(buffer));
            var res = new FileStreamResult(stream, "text/plain");
            res.FileDownloadName = "Datos.csv";
            return res;
        }

        [AllowAnonymous]
        public ActionResult LoginModal()
        {
            return PartialView("_LoginModal", new LoginView());
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : (TempData["returnUrl"] ?? "").ToString();
            if (ModelState.IsValid)
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: login.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));

                var e = repositorio.ObtenerPorEmail(login.Usuario);
                if (e == null || e.Clave != hashed)
                {
                    ModelState.AddModelError("", "El email o la clave no son correctos");
                    TempData["returnUrl"] = returnUrl;
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, e.Email),
                    new Claim("FullName", e.Nombre + " " + e.Apellido),
                    new Claim(ClaimTypes.Role, e.RolNombre)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                TempData.Remove("returnUrl");
                return Redirect(returnUrl ?? "/");
            }
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditarDatosPersonales(int id, string Nombre, string Apellido, string Email)
        {
            try
            {
                var usuario = repositorio.ObtenerPorId(id);
                if (usuario == null) return NotFound();
                usuario.Nombre = Nombre;
                usuario.Apellido = Apellido;
                usuario.Email = Email;
                repositorio.Modificacion(usuario);
                TempData["Success"] = "Datos personales actualizados correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar datos personales: {ex.Message}";
            }
            return RedirectToAction(nameof(Perfil));
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditarAvatar(int id, IFormFile Avatar, bool EliminarAvatar = false)
        {
            try
            {
                var usuario = repositorio.ObtenerPorId(id);
                if (usuario == null) return NotFound();
                if (EliminarAvatar)
                    usuario.Avatar = null;
                if (Avatar != null && Avatar.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    string fileName = "avatar_" + id + Path.GetExtension(Avatar.FileName);
                    string fullPath = Path.Combine(path, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        Avatar.CopyTo(stream);
                    }
                    usuario.Avatar = "/Uploads/" + fileName;
                }
                repositorio.Modificacion(usuario);
                TempData["Success"] = "Avatar actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar avatar: {ex.Message}";
            }
            return RedirectToAction(nameof(Perfil));
        }

        [Authorize]
        [HttpPost]
        public IActionResult CambiarClave(int id, string ClaveActual, string NuevaClave, string ConfirmarClave)
        {
            try
            {
                var usuario = repositorio.ObtenerPorId(id);
                if (usuario == null) return NotFound();
                string hashedActual = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: ClaveActual,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                if (usuario.Clave != hashedActual)
                {
                    TempData["Error"] = "La contraseña actual es incorrecta.";
                    return RedirectToAction(nameof(Perfil));
                }
                if (NuevaClave != ConfirmarClave)
                {
                    TempData["Error"] = "La nueva contraseña y su confirmación no coinciden.";
                    return RedirectToAction(nameof(Perfil));
                }
                string hashedNueva = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: NuevaClave,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                usuario.Clave = hashedNueva;
                repositorio.Modificacion(usuario);
                TempData["Success"] = "Contraseña actualizada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar contraseña: {ex.Message}";
            }
            return RedirectToAction(nameof(Perfil));
        }
    }
}
