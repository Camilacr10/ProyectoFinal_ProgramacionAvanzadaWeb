using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinalBLL.DTOs;

namespace ProyectoFinalBLL.Interfaces
{
    public interface IFlujoSolicitudesServicio
    {
        // LISTAS PARA ANALISTA (Solicitudes con estado Registrado o Devolución)
        Task<List<SolicitudDto>> ObtenerAnalisisAsync();

        // LISTAS PARA GESTOR (Solicitudes en estado EnviadoAprobacion)
        Task<List<SolicitudDto>> ObtenerAprobacionesAsync();

        // CAMBIA EL ESTADO DE UNA SOLICITUD (registra tracking)
        Task<CustomResponse<object>> CambiarEstadoAsync(int idSolicitud, string nuevoEstado, string? comentario, ClaimsPrincipal user);

        // OBTIENE EL HISTORIAL (TRACKING) DE UNA SOLICITUD
        Task<CustomResponse<List<TrackingDto>>> ObtenerTrackingAsync(int idSolicitud);

        // LISTAS LIGERAS (CLIENTES Y USUARIOS)
        Task<List<ClienteLigeroDto>> ObtenerClientesLigeroAsync();
        Task<List<UsuarioLigeroDto>> ObtenerUsuariosLigeroAsync();
    }
}