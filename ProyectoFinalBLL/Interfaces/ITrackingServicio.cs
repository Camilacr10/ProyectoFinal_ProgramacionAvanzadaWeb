using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinalBLL.DTOs;

namespace ProyectoFinalBLL.Interfaces
{
    public interface ITrackingServicio
    {
        Task<CustomResponse<object>> GuardarAsync(int idSolicitud, string estado, string accion, string? comentario, string userId);
        Task<CustomResponse<List<TrackingDto>>> ListarAsync(int idSolicitud);
    }
}