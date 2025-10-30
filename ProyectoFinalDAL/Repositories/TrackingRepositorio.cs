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
        // Contexto de base de datos
        private readonly SgcDbContext _db;

        // Constructor: recibe el contexto para interactuar con la base
        public TrackingRepositorio(SgcDbContext db)
        {
            _db = db;
        }

        // ===================== LISTAR TRACKING =====================
        // Devuelve todos los registros de tracking de una solicitud específica.
        // Se usa AsNoTracking() porque solo se leen los datos (no se modifican).
        public Task<List<SolicitudTracking>> ListarPorSolicitudAsync(int idSolicitud)
        {
            return _db.SolicitudTrackings
                      .Where(x => x.IdSolicitud == idSolicitud) // Filtra por ID de solicitud
                      .OrderBy(x => x.Fecha)                    // Ordena por fecha ascendente
                      .AsNoTracking()                           // Mejora rendimiento en lectura
                      .ToListAsync();                           // Convierte a lista
        }

        // ===================== AGREGAR TRACKING =====================
        // Inserta un nuevo registro de seguimiento en la base de datos.
        public async Task<bool> AgregarAsync(SolicitudTracking item)
        {
            _db.SolicitudTrackings.Add(item);  // Agrega el registro al contexto
            await _db.SaveChangesAsync();       // Guarda los cambios en base de datos
            return true;                        // Retorna true si todo sale bien
        }
    }
}