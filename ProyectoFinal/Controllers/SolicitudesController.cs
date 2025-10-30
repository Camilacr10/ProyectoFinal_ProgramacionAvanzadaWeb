using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinal.Controllers
{    public class SolicitudesController : Controller
    {
        private readonly ISolicitudService _service;
        private readonly IClienteService _clientes;

        public SolicitudesController(ISolicitudService service, IClienteService clientes)
        {
            _service = service;
            _clientes = clientes;

        }

        public async Task<IActionResult> Index()
        {
            var solicitudes = await _service.GetAllAsync();
            return View(solicitudes);
        }

        public async Task<IActionResult> Create()
        {
            // Solo lectura de clientes para armar el dropdown de cédulas
            ViewBag.Clientes = await _clientes.GetAllAsync(); // IEnumerable<ClienteDto>
            return View(new SolicitudDto());
        }

        // POST: /Solicitudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolicitudDto dto)
        {
            // Validación mínima: IdCliente desde el dropdown + DataAnnotations
            if (dto.IdCliente <= 0)
                ModelState.AddModelError(nameof(dto.IdCliente), "Selecciona una cédula válida.");

            // ¿Es petición AJAX?
            bool isAjax = string.Equals(
                Request.Headers["X-Requested-With"].ToString(),
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase);

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    // Devuelve errores de validación
                    var errs = ModelState
                        .Where(kv => kv.Value?.Errors?.Count > 0)
                        .ToDictionary(
                            kv => kv.Key,
                            kv => kv.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return Json(new { success = false, message = "Datos inválidos", errors = errs });
                }

                // Para postback normal
                ViewBag.Clientes = await _clientes.GetAllAsync();
                return View(dto);
            }

            try
            {
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
