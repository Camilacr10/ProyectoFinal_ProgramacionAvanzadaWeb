using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;

namespace ProyectoFinal.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteService _service;
        public ClientesController(IClienteService service) => _service = service;

        // GET: /Clientes
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        // GET: /Clientes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        // GET: /Clientes/Create
        public IActionResult Create() => View(new ClienteDto());

        // POST: /Clientes/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _service.CreateAsync(dto);
                TempData["ok"] = "Cliente creado.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(dto.Identificacion), ex.Message);
                return View(dto);
            }
        }

        // GET: /Clientes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        // POST: /Clientes/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _service.UpdateAsync(dto);
                TempData["ok"] = "Cliente actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(dto.Identificacion), ex.Message);
                return View(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET: /Clientes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        // POST: /Clientes/DeleteConfirmed/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            TempData["ok"] = "Cliente eliminado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditModal(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null) return NotFound();
            return PartialView("_EditModal", dto); // busca Views/Clientes/_EditModal.cshtml
        }

        // POST: /Clientes/EditModal     -> guarda y responde JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(ProyectoFinalBLL.DTOs.ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Devolver el mismo parcial con errores para re-render en el modal
                return PartialView("_EditModal", dto);
            }

            try
            {
                await _service.UpdateAsync(dto);
                return Json(new { ok = true, msg = "Cliente actualizado." });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(dto.Identificacion), ex.Message);
                return PartialView("_EditModal", dto);
            }
            catch (KeyNotFoundException)
            {
                return Json(new { ok = false, msg = "Cliente no encontrado." });
            }
        }
    }
}
