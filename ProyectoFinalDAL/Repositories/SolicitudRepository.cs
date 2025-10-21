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
    public class SolicitudRepository : ISolicitudRepository
    {
        private readonly SgcDbContext _ctx;
        public SolicitudRepository(SgcDbContext ctx) => _ctx = ctx;

        public Task<List<Solicitud>> GetAllAsync()
            => _ctx.Solicitudes.AsNoTracking()
            .Include(s => s.Cliente)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();

        public Task<Solicitud?> GetByIdAsync(int id)
            => _ctx.Solicitudes.FindAsync(id).AsTask();

        public async Task AddAsync(Solicitud entity)
            => await _ctx.Solicitudes.AddAsync(entity);

        public Task UpdateAsync(Solicitud entity)
        {
            _ctx.Solicitudes.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.Solicitudes.FindAsync(id);
            if (e != null) _ctx.Solicitudes.Remove(e);
        }

        public Task<int> SaveChangesAsync() => _ctx.SaveChangesAsync();

        public Task<bool> SolicitudConflictiva(int idCliente)
            => _ctx.Solicitudes.AnyAsync(s =>
                s.IdCliente == idCliente &&
                (s.Estado == "Registrado" || s.Estado == "Devolucion"));
    }
}
