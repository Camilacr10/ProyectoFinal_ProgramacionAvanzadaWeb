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
            private readonly ISolicitudService _sol;
            private readonly ITrackingServicio _trk;

            public FlujoSolicitudesController(ISolicitudService sol, ITrackingServicio trk)
            { _sol = sol; _trk = trk; }

            public IActionResult Analisis() => View();
            public IActionResult Aprobaciones() => View();
            public IActionResult Reporte() => View();

            [HttpGet, Authorize(Roles = "Analista,Admin")]
            public async Task<IActionResult> ObtenerAnalisis()
            {
                var all = await _sol.ObtenerTodosAsync();
                if (all.EsError) return Json(all);

                var data = (all.Data ?? new List<SolicitudDto>())
                           .Where(x => x.Estado == "Registrado" || x.Estado == "Devolucion")
                           .ToList();
                return Json(new CustomResponse<List<SolicitudDto>> { Data = data });
            }

            [HttpGet, Authorize(Roles = "Gestor,Admin")]
            public async Task<IActionResult> ObtenerAprobaciones()
            {
                var all = await _sol.ObtenerTodosAsync();
                if (all.EsError) return Json(all);

                var data = (all.Data ?? new List<SolicitudDto>())
                           .Where(x => x.Estado == "EnviadoAprobacion")
                           .ToList();
                return Json(new CustomResponse<List<SolicitudDto>> { Data = data });
            }

            public class CambioVm
            {
                public int IdSolicitud { get; set; }
                public string NuevoEstado { get; set; } = "";
                public string? Comentario { get; set; }
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CambiarEstado(CambioVm m)
            {
                var get = await _sol.ObtenerPorIdAsync(m.IdSolicitud);
                if (get.EsError || get.Data == null)
                    return Json(new CustomResponse<object> { EsError = true, Mensaje = "Solicitud no existe" });

                var s = get.Data;
                s.Estado = m.NuevoEstado;
                var upd = await _sol.ActualizarAsync(s);
                if (upd.EsError) return Json(new CustomResponse<object> { EsError = true, Mensaje = upd.Mensaje });

                var accion = m.NuevoEstado switch
                {
                    "EnviadoAprobacion" => "Enviada aprobación",
                    "Aprobado" => "Aprobada",
                    "Devolucion" => "Devolución",
                    _ => "Cambio estado"
                };

                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "n/a";
                await _trk.GuardarAsync(s.IdSolicitud, s.Estado, accion, m.Comentario, userId);

                return Json(new CustomResponse<object> { Mensaje = "Estado actualizado" });
            }

            [HttpGet]
            public async Task<IActionResult> ObtenerTracking(int id)
            {
                var resp = await _trk.ListarAsync(id);
                return Json(resp);
            }

            [HttpGet]
            public IActionResult Tracking(int id) => View(new TrackingPageVm { IdSolicitud = id });

            public class TrackingPageVm
            {
                public int IdSolicitud { get; set; }
            }
        }
    }