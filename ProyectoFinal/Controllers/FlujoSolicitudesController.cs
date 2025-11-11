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
        // Servicio de flujo
        private readonly IFlujoSolicitudesServicio _flujo;

        public FlujoSolicitudesController(IFlujoSolicitudesServicio flujo)
        {
            _flujo = flujo;
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
            var data = await _flujo.ObtenerAnalisisAsync();
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
            var data = await _flujo.ObtenerAprobacionesAsync();
            // Devuelve el formato esperado por DataTables
            return Json(new { data });
        }

        // =======================
        // CAMBIA EL ESTADO DE UNA SOLICITUD
        // (Analista o Gestor según corresponda)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(CambioEstadoDto m) 
        {
            var resp = await _flujo.CambiarEstadoAsync(m.IdSolicitud, m.NuevoEstado, m.Comentario, User);

            // Respuesta para AJAX
            if (resp.EsError)
                return Json(new { esError = true, mensaje = resp.Mensaje ?? "No se pudo actualizar" });

            return Json(new { esError = false, mensaje = resp.Mensaje ?? "Estado actualizado" });
        }

        // =======================
        // OBTIENE EL HISTORIAL (TRACKING) DE UNA SOLICITUD
        // =======================
        [HttpGet]
        public async Task<IActionResult> ObtenerTracking(int id)
        {
            var resp = await _flujo.ObtenerTrackingAsync(id);
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


        // ====== Listado ligero de clientes para el mapa JS (IdCliente -> Identificacion) ======
        [HttpGet]
        [Route("/Clientes/ObtenerClientes")]
        public async Task<IActionResult> ObtenerClientes()
        {
            var data = await _flujo.ObtenerClientesLigeroAsync();
            return Json(new { esError = false, data });
        }

        // ====== Listado ligero de usuarios para el mapa JS (Id -> UserName, Email) ======
        [HttpGet]
        [Route("/Usuarios/ObtenerUsuarios")]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var data = await _flujo.ObtenerUsuariosLigeroAsync();
            return Json(new { esError = false, data });
        }
    }
}