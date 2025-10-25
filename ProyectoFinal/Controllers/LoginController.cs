using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalBLL.DTOs;
using System.Security.Claims;

namespace ProyectoFinal.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public LoginController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Index()
        {
            // Si el usuario ya está autenticado, redirige al Home
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new LoginDto());
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Index(LoginDto login)
        {
            if (!ModelState.IsValid)
                return View(login);

            // Buscar usuario por correo
            var user = await _usuarioService.GetByCorreoAsync(login.Correo);
            if (user == null || user.Clave != login.Clave)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View(login);
            }

            // Crear claims con información del usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.NombreCompleto),
                new Claim(ClaimTypes.Email, user.Correo),
                new Claim(ClaimTypes.Role, user.Rol),
                new Claim("IdUsuario", user.IdUsuario.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Iniciar sesión con cookies
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirigir automáticamente al panel principal
            return RedirectToAction("Index", "Home");
        }

        // GET: Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        // GET: Access Denied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
