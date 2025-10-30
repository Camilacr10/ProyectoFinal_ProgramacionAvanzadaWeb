using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;

namespace ProyectoFinal.Controllers
{
    [Authorize]
    public class FlujoSolicitudesController : Controller
    {
        // Servicio de solicitudes
        private readonly ISolicitudService _sol;
        // Servicio de tracking (historial de movimientos)
        private readonly ITrackingServicio _trk;

        public FlujoSolicitudesController(ISolicitudService sol, ITrackingServicio trk)
        {
            _sol = sol;
            _trk = trk;
        }

        // Pantalla para análisis de solicitudes (rol Analista/Admin)
        public IActionResult Analisis() => View();

        // Pantalla para aprobaciones de solicitudes (rol Gestor/Admin)
        public IActionResult Aprobaciones() => View();

        // Pantalla para el reporte de tracking
        public IActionResult Reporte() => View();

        // =======================
        // LISTAS PARA ANALISTA
        // (Solicitudes con estado Registrado o Devolución)
        // =======================
        [HttpGet, Authorize(Roles = "Analista,Administrador")]
        public async Task<IActionResult> ObtenerAnalisis()
        {
            var list = await _sol.GetAllAsync();

            var data = (list ?? new List<SolicitudDto>())
                        .Where(x => (x.Estado ?? "").Trim() == "Registrado" || (x.Estado ?? "").Trim() == "Devolucion")
                        .ToList();

            // Devuelve el formato esperado por DataTables
            return Json(new { data });
        }

        // =======================
        // LISTAS PARA GESTOR
        // (Solicitudes en estado EnviadoAprobacion)
        // =======================
        [HttpGet, Authorize(Roles = "Gestor,Administrador")]
        public async Task<IActionResult> ObtenerAprobaciones()
        {
            var list = await _sol.GetAllAsync();

            var data = (list ?? new List<SolicitudDto>())
                        .Where(x => (x.Estado ?? "").Trim() == "EnviadoAprobacion")
                        .ToList();

            return Json(new { data });
        }

        // Modelo para cambio de estado
        public class CambioVm
        {
            public int IdSolicitud { get; set; }
            public string NuevoEstado { get; set; } = "";
            public string? Comentario { get; set; }
        }

        // =======================
        // CAMBIA EL ESTADO DE UNA SOLICITUD
        // (Analista o Gestor según corresponda)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(CambioVm m)
        {
            // Busca la solicitud
            var s = await _sol.GetByIdAsync(m.IdSolicitud);
            if (s is null)
                return Json(new { esError = true, mensaje = "Solicitud no existe" });

            // Actualiza el estado
            s.Estado = m.NuevoEstado;
            await _sol.UpdateAsync(s);

            // Determina la acción registrada en el tracking
            var accion = m.NuevoEstado switch
            {
                "EnviadoAprobacion" => "Enviada aprobación",
                "Aprobado" => "Aprobada",
                "Devolucion" => "Devolución",
                _ => "Cambio estado"
            };

            // Obtiene el usuario actual
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? (User?.Identity?.Name ?? "n/a");

            // Guarda el movimiento en el tracking
            await _trk.GuardarAsync(s.IdSolicitud, s.Estado, accion, m.Comentario, userId);

            // Respuesta para AJAX
            return Json(new { esError = false, mensaje = "Estado actualizado" });
        }

        // =======================
        // OBTIENE EL HISTORIAL (TRACKING) DE UNA SOLICITUD
        // =======================
        [HttpGet]
        public async Task<IActionResult> ObtenerTracking(int id)
        {
            var resp = await _trk.ListarAsync(id);
            // Devuelve el formato estándar { esError, data, mensaje }
            return Json(resp);
        }

        // =======================
        // VISTA DEL TRACKING INDIVIDUAL
        // =======================
        [HttpGet]
        public IActionResult Tracking(int id) => View(new TrackingPageVm { IdSolicitud = id });

        // ViewModel usado en la vista Tracking.cshtml
        public class TrackingPageVm
        {
            public int IdSolicitud { get; set; }
        }
    }
}