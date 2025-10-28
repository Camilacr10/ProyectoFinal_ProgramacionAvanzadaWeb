using ProyectoFinalBLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalBLL.Interfaces
{
    public interface ISolicitudService
    {
        Task<List<SolicitudDto>> GetAllAsync();
        Task<SolicitudDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(SolicitudDto dto);
        Task UpdateAsync(SolicitudDto dto);
        Task DeleteAsync(int id);
        Task<bool> SolicitudConflictiva(int idCliente);

        //Lo necesito para el tracking
        Task<CustomResponse<List<SolicitudDto>>> ObtenerTodosAsync();
        Task<CustomResponse<SolicitudDto>> ObtenerPorIdAsync(int id);
        Task<CustomResponse<SolicitudDto>> ActualizarAsync(SolicitudDto dto);
    }
}
