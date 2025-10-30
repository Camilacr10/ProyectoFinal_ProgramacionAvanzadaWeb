using System.Linq; // para unir errores de ModelState
using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;        // ClienteDto y CustomResponse<T>
using ProyectoFinalBLL.Interfaces;  // IClienteService

namespace ProyectoFinal.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        // ================= VISTAS (fallback) =================

        // GET: /Clientes
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        // ================== MODALES (AJAX + CustomResponse) ==================

        // CREATE
        [HttpGet]
        public IActionResult CreateModal()
            => PartialView("Create", new ClienteDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModal(ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(CustomResponse<string>.Fail(errores));
            }

            try
            {
                var id = await _service.CreateAsync(dto);
                return Json(CustomResponse<int>.Ok(id, "Cliente creado."));
            }
            catch (InvalidOperationException ex)
            {
                return Json(CustomResponse<string>.Fail(ex.Message));
            }
            catch
            {
                return Json(CustomResponse<string>.Fail("Ocurrió un error inesperado."));
            }
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> EditModal(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null) return NotFound();
            return PartialView("_EditModal", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(CustomResponse<string>.Fail(errores));
            }

            try
            {
                await _service.UpdateAsync(dto);
                return Json(CustomResponse<string>.Ok(null, "Cliente actualizado."));
            }
            catch (InvalidOperationException ex)
            {
                return Json(CustomResponse<string>.Fail(ex.Message));
            }
            catch (KeyNotFoundException)
            {
                return Json(CustomResponse<string>.Fail("Cliente no encontrado."));
            }
            catch
            {
                return Json(CustomResponse<string>.Fail("Ocurrió un error inesperado."));
            }
        }

        // DELETE
        [HttpGet]
        public async Task<IActionResult> DeleteModal(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto is null) return NotFound();
            return PartialView("Delete", dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModalConfirmed(int IdCliente)
        {
            try
            {
                await _service.DeleteAsync(IdCliente);
                return Json(CustomResponse<string>.Ok(null, "Cliente eliminado."));
            }
            catch
            {
                return Json(CustomResponse<string>.Fail("No se pudo eliminar el cliente."));
            }
        }

    }
}
