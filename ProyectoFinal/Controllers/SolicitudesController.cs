using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;

namespace ProyectoFinal.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ISolicitudService _service;
        private readonly IClienteService _clientes;
        private readonly IWebHostEnvironment _env;

        public SolicitudesController(
            ISolicitudService service,
            IClienteService clientes,
            IWebHostEnvironment env)
        {
            _service = service;
            _clientes = clientes;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            TempData.Remove("ok");
            TempData.Remove("error");

            var solicitudes = await _service.GetAllAsync();
            ViewBag.Clientes = await _clientes.GetAllAsync();

            return View(solicitudes);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Clientes = await _clientes.GetAllAsync();
            return View(new SolicitudDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolicitudDto dto, IFormFile? Documento)
        {
            if (!ModelState.IsValid)
                return await HandleInvalidCreateAsync(dto);

            try
            {
                var id = await _service.CreateAsync(dto, Documento, _env.WebRootPath);

                return IsAjaxRequest()
                    ? Json(new { success = true, idSolicitud = id })
                    : SuccessRedirect();
            }
            catch (Exception ex)
            {
                return await HandleCreateExceptionAsync(dto, ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["ok"] = "La solicitud fue eliminada correctamente.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["error"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo eliminar la solicitud.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IsAjaxRequest()
        {
            return string.Equals(
                Request.Headers["X-Requested-With"].ToString(),
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);
        }

        private async Task<IActionResult> HandleInvalidCreateAsync(SolicitudDto dto)
        {
            if (IsAjaxRequest())
            {
                var errors = ModelState
                    .Where(kv => kv.Value?.Errors?.Count > 0)
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                return Json(new
                {
                    success = false,
                    message = "Datos inválidos",
                    errors
                });
            }

            ViewBag.Clientes = await _clientes.GetAllAsync();
            return View(dto);
        }

        private IActionResult SuccessRedirect()
        {
            TempData["ok"] = "La solicitud de crédito fue creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> HandleCreateExceptionAsync(SolicitudDto dto, Exception ex)
        {
            if (IsAjaxRequest())
                return Json(new { success = false, message = ex.Message });

            TempData["error"] = "Hubo un error, por favor intente de nuevo.";
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Clientes = await _clientes.GetAllAsync();
            return View(dto);
        }
    }
}
