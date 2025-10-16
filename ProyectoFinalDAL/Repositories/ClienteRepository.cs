using Microsoft.EntityFrameworkCore;
using ProyectoFinalDAL.Data;
using ProyectoFinalDAL.Entidades;

namespace ProyectoFinalDAL.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly SgcDbContext _ctx;
        public ClienteRepository(SgcDbContext ctx) => _ctx = ctx;

        public Task<List<Cliente>> GetAllAsync()
            => _ctx.Clientes.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();

        public Task<Cliente?> GetByIdAsync(int id)
            => _ctx.Clientes.FindAsync(id).AsTask();

        public Task<Cliente?> GetByIdentificacionAsync(string identificacion)
            => _ctx.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Identificacion == identificacion);

        public async Task AddAsync(Cliente entity) => await _ctx.Clientes.AddAsync(entity);

        public Task UpdateAsync(Cliente entity)
        {
            _ctx.Clientes.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.Clientes.FindAsync(id);
            if (e != null) _ctx.Clientes.Remove(e);
        }

        public Task<int> SaveChangesAsync() => _ctx.SaveChangesAsync();
    }
}
