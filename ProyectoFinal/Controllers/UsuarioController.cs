using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;

namespace ProyectoFinal.Controllers
{
    [Authorize] // requiere login
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _service;
        public UsuarioController(IUsuarioService service) => _service = service;

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View(new UsuarioDto());

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.CreateAsync(dto);
            TempData["ok"] = "Usuario creado.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.UpdateAsync(dto);
            TempData["ok"] = "Usuario actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        [HttpPost, ActionName("Delete"), Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["ok"] = "Usuario eliminado.";
            return RedirectToAction(nameof(Index));
        }

        // Los usuarios normales pueden ver su propio perfil
        public async Task<IActionResult> Profile()
        {
            var correo = User.Identity?.Name;
            if (correo == null) return Unauthorized();

            var dto = await _service.GetByCorreoAsync(correo);
            return dto is null ? NotFound() : View(dto);
        }
    }
}
