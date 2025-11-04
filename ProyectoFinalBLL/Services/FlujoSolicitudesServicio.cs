using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProyectoFinalBLL.DTOs;
using ProyectoFinalBLL.Interfaces;

namespace ProyectoFinalBLL.Services
{
    public class FlujoSolicitudesServicio : IFlujoSolicitudesServicio
    {
        // Servicio de solicitudes
        private readonly ISolicitudService _sol;

        // Servicio de tracking (historial de movimientos)
        private readonly ITrackingServicio _trk;

        // Mapper opcional - No se usa porque los datos que entran y salen de este servicio ya están mapeados en DTOs en ISolicitudService y ITrackingServicio
        private readonly IMapper _mapper;

        public FlujoSolicitudesServicio(ISolicitudService sol, ITrackingServicio trk, IMapper mapper)
        {
            _sol = sol;
            _trk = trk;
            _mapper = mapper;
        }

        // =======================
        // LISTAS PARA ANALISTA
        // (Solicitudes con estado Registrado o Devolución)
        // =======================
        public async Task<List<SolicitudDto>> ObtenerAnalisisAsync()
        {
            var list = await _sol.GetAllAsync();

            var data = (list ?? new List<SolicitudDto>())
                        .Where(x => (x.Estado ?? "").Trim() == "Registrado" || (x.Estado ?? "").Trim() == "Devolucion")
                        .ToList();

            return data;
        }

        // =======================
        // LISTAS PARA GESTOR
        // (Solicitudes en estado EnviadoAprobacion)
        // =======================
        public async Task<List<SolicitudDto>> ObtenerAprobacionesAsync()
        {
            var list = await _sol.GetAllAsync();

            var data = (list ?? new List<SolicitudDto>())
                        .Where(x => (x.Estado ?? "").Trim() == "EnviadoAprobacion")
                        .ToList();

            return data;
        }

        // =======================
        // CAMBIA EL ESTADO DE UNA SOLICITUD
        // (Analista o Gestor según corresponda)
        // =======================
        public async Task<CustomResponse<object>> CambiarEstadoAsync(int idSolicitud, string nuevoEstado, string? comentario, ClaimsPrincipal user)
        {
            var r = new CustomResponse<object>();

            // Busca la solicitud
            var s = await _sol.GetByIdAsync(idSolicitud);
            if (s is null)
            {
                r.EsError = true;
                r.Mensaje = "Solicitud no existe";
                return r;
            }

            // Actualiza el estado
            s.Estado = nuevoEstado;
            await _sol.UpdateAsync(s);

            // Determina la acción registrada en el tracking
            var accion = nuevoEstado switch
            {
                "EnviadoAprobacion" => "Enviada aprobación",
                "Aprobado" => "Aprobada",
                "Devolucion" => "Devolución",
                _ => "Cambio estado"
            };

            // Obtiene el usuario actual
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? (user?.Identity?.Name ?? "n/a");

            // Guarda el movimiento en el tracking
            var trkResp = await _trk.GuardarAsync(s.IdSolicitud, s.Estado, accion, comentario, userId);
            if (trkResp.EsError)
            {
                r.EsError = true;
                r.Mensaje = trkResp.Mensaje ?? "No se pudo guardar el tracking";
                return r;
            }

            // Respuesta ok
            r.Mensaje = "Estado actualizado";
            return r;
        }

        // =======================
        // OBTIENE EL HISTORIAL (TRACKING) DE UNA SOLICITUD
        // =======================
        public async Task<CustomResponse<List<TrackingDto>>> ObtenerTrackingAsync(int idSolicitud)
        {
            // Devuelve el formato estándar { esError, data, mensaje }
            var resp = await _trk.ListarAsync(idSolicitud);
            return resp;
        }
    }
}