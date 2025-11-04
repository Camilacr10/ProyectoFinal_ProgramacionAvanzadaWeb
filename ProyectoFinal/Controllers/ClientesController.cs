using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;

namespace ProyectoFinal.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ClientesController : Controller
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        // PANTALLA LISTA
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        // DETALLE
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : View(dto);
        }

        // ===== JSON estilo Flujo (listas planas / DataTables) =====
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var data = await _service.GetAllAsync();
            return Json(new { data });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerLigero()
        {
            var data = (await _service.GetAllAsync())
                .Select(c => new { c.IdCliente, c.Identificacion, c.Nombre });
            return Json(new { esError = false, data });
        }

        // ================== MODALES (AJAX + CustomResponse) ==================

        // CREATE
        [HttpGet]
        public IActionResult CreateModal()
            => PartialView("Create", new ClienteDto());

        [HttpPost]
        public async Task<IActionResult> CreateModal(ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(CustomResponse<string>.Fail(errores));
            }

            var id = await _service.CreateAsync(dto);
            return Ok(CustomResponse<int>.Ok(id, "Cliente creado."));
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
        public async Task<IActionResult> EditModal(ClienteDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(CustomResponse<string>.Fail(errores));
            }

            // Si no existe, actúa igual que en Flujo: respuesta clara sin lanzar excepción
            var existe = await _service.GetByIdAsync(dto.IdCliente);
            if (existe is null)
                return BadRequest(CustomResponse<string>.Fail("Cliente no encontrado."));

            await _service.UpdateAsync(dto);
            return Ok(CustomResponse<string>.Ok(null, "Cliente actualizado."));
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
        public async Task<IActionResult> DeleteModalConfirmed(int IdCliente)
        {
            var existe = await _service.GetByIdAsync(IdCliente);
            if (existe is null)
                return BadRequest(CustomResponse<string>.Fail("Cliente no encontrado."));

            await _service.DeleteAsync(IdCliente);
            return Ok(CustomResponse<string>.Ok(null, "Cliente eliminado."));
        }
    }
}
