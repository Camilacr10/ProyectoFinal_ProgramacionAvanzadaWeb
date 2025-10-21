using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoFinal.Controllers
{    public class SolicitudesController : Controller
    {
        private readonly ISolicitudService _service;
        public SolicitudesController(ISolicitudService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var solicitudes = await _service.GetAllAsync();
            return View(solicitudes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolicitudDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                await _service.CreateAsync(dto);

                // Mensaje para el modal de éxito en Index
                TempData["ok"] = "La solicitud de crédito fue creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Mensaje de error en Create
                TempData["error"] = "Hubo un error, por favor intente de nuevo.";
                ModelState.AddModelError(string.Empty, "Hubo un error, por favor intente de nuevo.");
                return View(dto);
            }
        }
    }
}
