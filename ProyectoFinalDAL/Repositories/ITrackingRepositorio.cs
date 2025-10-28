using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Repositories
{
    public interface ITrackingRepositorio
    {
        Task<List<SolicitudTracking>> ListarPorSolicitudAsync(int idSolicitud);
        Task<bool> AgregarAsync(SolicitudTracking item);
    }
}