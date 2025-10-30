using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using System.IO;

namespace ProyectoFinal.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ISolicitudService _service;
        private readonly IClienteService _clientes;
        private readonly IWebHostEnvironment _env; // <- agregado para guardar archivos

        public SolicitudesController(ISolicitudService service, IClienteService clientes, IWebHostEnvironment env)
        {
            _service = service;
            _clientes = clientes;
            _env = env; // <- asignación
        }

        public async Task<IActionResult> Index()
        {
            // Minimiza cambios: limpia mensajes genéricos para que no aparezcan banners de Clientes aquí
            TempData.Remove("ok");
            TempData.Remove("error");

            var solicitudes = await _service.GetAllAsync();
            return View(solicitudes);
        }

        // POST: /Solicitudes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["ok"] = "La solicitud fue eliminada correctamente.";
            }
            catch (KeyNotFoundException)
            {
                TempData["error"] = "La solicitud no existe o ya fue eliminada.";
            }
            catch (Exception)
            {
                TempData["error"] = "No se pudo eliminar la solicitud.";
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Create()
        {
            // Solo lectura de clientes para armar el dropdown de cédulas
            ViewBag.Clientes = await _clientes.GetAllAsync(); 
            return View(new SolicitudDto());
        }

        // POST: /Solicitudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolicitudDto dto, IFormFile? Documento)
        {
            if (dto.IdCliente <= 0)
                ModelState.AddModelError(nameof(dto.IdCliente), "Selecciona una cédula válida.");

            bool isAjax = string.Equals(
                Request.Headers["X-Requested-With"].ToString(),
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase
            );

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    var errs = ModelState
                        .Where(kv => kv.Value?.Errors?.Count > 0)
                        .ToDictionary(
                            kv => kv.Key,
                            kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new { success = false, message = "Datos inválidos", errors = errs });
                }

                ViewBag.Clientes = await _clientes.GetAllAsync();
                return View(dto);
            }

            try
            {
                if (Documento != null && Documento.Length > 0)
                {
                    string uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "solicitudes");
                    Directory.CreateDirectory(uploadsRoot);

                    var safeName = Path.GetFileName(Documento.FileName);
                    var fileName = $"{Guid.NewGuid()}_{safeName}";
                    var fullPath = Path.Combine(uploadsRoot, fileName);

                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await Documento.CopyToAsync(stream);
                    }

                    dto.DocumentoPath = $"/uploads/solicitudes/{fileName}";
                }

                var id = await _service.CreateAsync(dto);

                if (isAjax)
                    return Json(new { success = true, idSolicitud = id });

                TempData["ok"] = "La solicitud de crédito fue creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (isAjax)
                    return Json(new { success = false, message = ex.Message });

                TempData["error"] = "Hubo un error, por favor intente de nuevo.";
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Clientes = await _clientes.GetAllAsync();
                return View(dto);
            }
        }
    }
}
