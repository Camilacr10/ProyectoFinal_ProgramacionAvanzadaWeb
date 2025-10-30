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
            if (dto.IdCliente <= 0)
                ModelState.AddModelError(nameof(dto.IdCliente), "Selecciona una cédula válida.");

            if (!ModelState.IsValid)
            {
                ViewBag.Clientes = await _clientes.GetAllAsync();
                return View(dto);
            }

            try
            {
                await _service.CreateAsync(dto);
                TempData["ok"] = "La solicitud de crédito fue creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = "Hubo un error, por favor intente de nuevo.";
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Clientes = await _clientes.GetAllAsync();
                return View(dto);
            }
        }
    }
}
