using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Repositories
{
    public interface ISolicitudRepository
    {
        Task<List<Solicitud>> GetAllAsync();
        Task<Solicitud?> GetByIdAsync(int id);
        Task AddAsync(Solicitud entity);
        Task UpdateAsync(Solicitud entity);
        Task DeleteAsync(int id);
        Task<int> SaveChangesAsync();
        Task<bool> SolicitudConflictiva(int idCliente);

        //Lo necesito para el tracking
        Task<List<Solicitud>> ObtenerTodosAsync();

        Task<Solicitud?> ObtenerPorIdAsync(int id);

        Task<bool> ActualizarAsync(Solicitud entidad);

    }
}