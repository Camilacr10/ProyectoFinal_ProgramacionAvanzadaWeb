using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Data;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Repositories
{
    public class TrackingRepositorio : ITrackingRepositorio
    {
        private readonly SgcDbContext _db;
        public TrackingRepositorio(SgcDbContext db) { _db = db; }

        public Task<List<SolicitudTracking>> ListarPorSolicitudAsync(int idSolicitud)
        {
            return _db.SolicitudTrackings
                      .Where(x => x.IdSolicitud == idSolicitud)
                      .OrderBy(x => x.Fecha)
                      .AsNoTracking()
                      .ToListAsync();
        }

        public async Task<bool> AgregarAsync(SolicitudTracking item)
        {
            _db.SolicitudTrackings.Add(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}